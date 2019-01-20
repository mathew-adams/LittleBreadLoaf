using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    public class ProductReportModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductReportModel(ProductContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(string productID)
        {

            var test = _context.Product
                       .Join(_context.ProductIngredient,
                            p => p.ProductID,
                            pi => pi.ProductID,
                            (p, pi) => new { p.Name, p.Price, pi.Description })
                       .Select(s => new { s.Name, s.Price, s.Description})
                       .ToList();

            foreach(var ent in test)
            {

               // string hhh = "";

            }
            return Page();
        }
    }
}