using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    [Authorize]
    public class ProductAddModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }
        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //ModelState.ErrorCount
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Product.ProductID = Guid.NewGuid();
            Product.Created = DateTime.Now;
            Product.LastUpdated = DateTime.Now;

            _context.Product.Add(Product);

            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Products/ProductView", new { Product.ProductID });
        }
    }
}