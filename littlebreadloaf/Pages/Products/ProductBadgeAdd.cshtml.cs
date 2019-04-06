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
    public class ProductBadgeAddModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductBadgeAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string ProductID { get; set; }

        [BindProperty]
        public string ProductName { get; set; }

        [BindProperty]
        public ProductBadge ProductBadge { get; set; }

        public async Task<IActionResult> OnGetAsync(string productID)
        {
            if (String.IsNullOrEmpty(productID) || !Guid.TryParse(productID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            ProductID = productID;

            var product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedID);

            if (product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }
            ProductName = product.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string productID)
        {
            if (String.IsNullOrEmpty(productID) || !Guid.TryParse(productID, out Guid parsedID))
            {
                return new RedirectToPageResult("/Products/ProductBadgeList");
            }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedID);

            if (product == null)
            {
                return new RedirectToPageResult("/Products/ProductBadgeList");
            }

            ProductName = product.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ProductBadge.ProductBadgeID = Guid.NewGuid();
            ProductBadge.ProductID = parsedID;

            _context.ProductBadge.Add(ProductBadge);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Products/ProductBadgeList", new { productID });
        }
    }
}