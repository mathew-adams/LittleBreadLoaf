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
    public class ProductIngredientAddModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductIngredientAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public String ProductID { get; set; }

        [BindProperty]
        public ProductIngredient ProductIngredient  { get; set; }

        [BindProperty]
        public string ProductName { get; set; }

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
                return new RedirectToPageResult("/Products/ProductList");
            }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.ProductID == parsedID);

            if (product == null)
            {
                return new RedirectToPageResult("/Products/ProductList");
            }
            ProductName = product.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ProductIngredient.ProductIngredientID = Guid.NewGuid();
            ProductIngredient.ProductID = parsedID;
            ProductIngredient.Created = DateTime.Now;

            _context.ProductIngredient.Add(ProductIngredient);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Products/ProductIngredientList", new { productID });
        }
    }
}