using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
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

        public IActionResult OnGet(string ProductID)
        {
            if (String.IsNullOrEmpty(ProductID) || !Guid.TryParse(ProductID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            Product = _context.Product.FirstOrDefault(m => m.ProductID == parsedID);

            if (Product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            ProductImages = _context.ProductImage.Where(m => m.ProductID == parsedID).ToList();

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

            var primaryImages = _context.ProductImage.Where(m => m.ProductID == parsedProductID && m.PrimaryImage == true);

            foreach(var image in primaryImages)
            {
                image.PrimaryImage = false;
                _context.Update(image);
            }

            var productImage = _context.ProductImage.FirstOrDefault(m => m.ProductImageID == parsedProductImageID);
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