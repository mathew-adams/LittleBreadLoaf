using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;


namespace littlebreadloaf.Services
{
    public class RenderViewComponentService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IViewComponentHelper _viewComponentHelper;

        public RenderViewComponentService(
            IServiceProvider serviceProvider,
            ITempDataProvider tempDataProvider,
            IViewComponentHelper viewComponentHelper
        )
        {
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
            _viewComponentHelper = viewComponentHelper;
        }

        public async Task<string> RenderViewComponentToStringAsync<TViewComponent>(object args)
            where TViewComponent : ViewComponent
        {
            var viewContext = GetFakeViewContext();
            (_viewComponentHelper as IViewContextAware).Contextualize(viewContext);

            var htmlContent = await _viewComponentHelper.InvokeAsync<TViewComponent>(args);
            var html = "";
            using (var stringWriter = new StringWriter())
            {
                htmlContent.WriteTo(stringWriter, HtmlEncoder.Default);
                html = stringWriter.ToString();
            }
                
            return html;
        }

        private ViewContext GetFakeViewContext(ActionContext actionContext = null, TextWriter writer = null)
        {
            if(actionContext == null)
            {
                actionContext = GetFakeActionContext();
            }
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            var tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                NullView.Instance,
                viewData,
                tempData,
                writer ?? TextWriter.Null,
                new HtmlHelperOptions());

            return viewContext;
        }

        private ActionContext GetFakeActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider,
            };

            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();

            return new ActionContext(httpContext, routeData, actionDescriptor);
        }

        private class NullView : IView
        {
            public static readonly NullView Instance = new NullView();
            public string Path => string.Empty;
            public Task RenderAsync(ViewContext context)
            {
                if (context == null) { throw new ArgumentNullException(nameof(context)); }
                return Task.CompletedTask;
            }
        }
    }
}
