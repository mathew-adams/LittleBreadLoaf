using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    [Authorize]
    public class ProductEditModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductEditModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(string productID)
        {
            if (String.IsNullOrEmpty(productID) || !Guid.TryParse(productID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //ModelState.ErrorCount
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Product.LastUpdated = DateTime.Now;
            _context.Product.Update(Product);

            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Products/ProductView", new { Product.ProductID });
        }

    }
}