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
    public class ProductImageListModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductImageListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public List<ProductImage> ProductImages { get; set; }

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

            ProductImages = await _context
                                    .ProductImage
                                    .Where(m => m.ProductID == parsedID)
                                    .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostPrimaryAsync(string ProductImageID, string ProductID)
        {
            //Sanitize inputs
            if (String.IsNullOrEmpty(ProductID)
                || !Guid.TryParse(ProductID, out Guid parsedProductID)
                || String.IsNullOrEmpty(ProductImageID)
                || !Guid.TryParse(ProductImageID, out Guid parsedProductImageID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = _context.Product.FirstOrDefault(m => m.ProductID == parsedProductID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            //TODO: Anything to do better?
            var primaryImages = _context.ProductImage.Where(m => m.ProductID == parsedProductID && m.PrimaryImage == true);

            foreach(var image in primaryImages)
            {
                image.PrimaryImage = false;
                _context.Update(image);
            }

            var productImage = await _context.ProductImage.FirstOrDefaultAsync(m => m.ProductImageID == parsedProductImageID);
            if(productImage != null)
            {
                productImage.PrimaryImage = true;
                _context.Update(productImage);
            }

            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Products/ProductImageList", new { ProductID = ProductID });
        }
    }
}