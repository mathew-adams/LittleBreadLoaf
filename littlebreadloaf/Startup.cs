using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using littlebreadloaf.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using System.IO;
using Microsoft.Extensions.Primitives;

namespace littlebreadloaf
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            });

            services.AddDbContext<ProductContext>
            (
                options => options.UseMySql(Configuration["ConnectionStrings:DefaultConnection"])
            );
            services.AddDbContext<ApplicationDbContext>
            (
                options => options.UseMySql(Configuration["ConnectionStrings:DefaultConnection"])
            );
            services.AddDefaultIdentity<IdentityUser>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseExceptionHandler("/Error");
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            //app.UseHttpsRedirection();

            app.UseMiddleware<ResponseCompressionQualityMiddleware>(new Dictionary<string, double>
            {
                { "br", 1.0 },
                { "gzip", 0.9 }
            });
            app.UseResponseCompression(); //Must come before UseStaticFiles

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24; //24 hours
                    ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, $"public, max-age={durationInSeconds}");
                }
            });
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}

public class BrotliCompressionProvider : ICompressionProvider
{
    public string EncodingName => "br";
    public bool SupportsFlush => true;
    public Stream CreateStream(Stream outputStream)
    {
        return new BrotliStream(outputStream, CompressionMode.Compress);
    }
}

public class ResponseCompressionQualityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDictionary<string, double> _encodingQuality;
    public ResponseCompressionQualityMiddleware(RequestDelegate next, IDictionary<string, double> encodingQuality)
    {
        _next = next;
        _encodingQuality = encodingQuality;
    }
    public async Task Invoke(HttpContext context)
    {
        StringValues encodings = context.Request.Headers[HeaderNames.AcceptEncoding];
        IList<StringWithQualityHeaderValue> encodingsList;
        if (!StringValues.IsNullOrEmpty(encodings)
            && StringWithQualityHeaderValue.TryParseList(encodings, out encodingsList)
            && (encodingsList != null) && (encodingsList.Count > 0))
        {
            string[] encodingsWithQuality = new string[encodingsList.Count];
            for (int encodingIndex = 0; encodingIndex < encodingsList.Count; encodingIndex++)
            {
                // If there is any quality value provided don't change anything
                if (encodingsList[encodingIndex].Quality.HasValue)
                {
                    encodingsWithQuality = null;
                    break;
                }
                else
                {
                    string encodingValue = encodingsList[encodingIndex].Value.Value;
                    encodingsWithQuality[encodingIndex] = (new StringWithQualityHeaderValue(encodingValue,
                        _encodingQuality.ContainsKey(encodingValue) ? _encodingQuality[encodingValue] : 0.1)).ToString();
                }
            }
            if (encodingsWithQuality != null)
                context.Request.Headers[HeaderNames.AcceptEncoding] = new StringValues(encodingsWithQuality);
        }
        await _next(context);
    }
}