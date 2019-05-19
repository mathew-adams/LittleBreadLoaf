using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Diagnostics;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        private readonly ProductContext _context;
        public ErrorModel(ProductContext context)
        {
            _context = context;
        }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public async Task<IActionResult> OnGetAsync()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if(exceptionFeature != null)
            {
                var systemError = new SystemError()
                {
                    ErrorID = Guid.NewGuid(),
                    RequestID = RequestId,
                    Path = exceptionFeature.Path,
                    Error = exceptionFeature.Error.ToString(),
                    Occurred = DateTime.Now
                };

                _context.SystemError.Add(systemError);
                await _context.SaveChangesAsync();
                
            }
            return Page();
        }
    }
}
