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
    public class ProductImageEditModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductImageEditModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductImage ProductImage { get; set; }

        [BindProperty]
        public string ProductName { get; set; }

        public async Task<IActionResult> OnGetAsync(string productImageID)
        {
            if (String.IsNullOrEmpty(productImageID) || !Guid.TryParse(productImageID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            ProductImage = await _context.ProductImage.FirstOrDefaultAsync(m => m.ProductImageID == parsedID);

            if (ProductImage == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == ProductImage.ProductID); //TODO: Can use a where clause and an anonymouse select for just the name
            if(product != null)
            {
                ProductName = product.Name;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
          
            var product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == ProductImage.ProductID);

            if (product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            ProductName = product.Name;
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ProductImage.Update(ProductImage);

            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Products/ProductImageList", new { ProductImage.ProductID });
        }
    }
}