using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace littlebreadloaf.Pages.Products
{
    [Authorize]
    public class ProductBundleAddModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductBundleAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductBundle ProductBundle { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> Products { get; set; }

        [BindProperty]
        public IEnumerable<string> SelectedProducts { get; set; }

        public void OnGet()
        {
            Products = GetProducts();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            Products = GetProducts();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ProductBundle.ProductBundleID = Guid.NewGuid();
            ProductBundle.Created = DateTime.Now;

            _context.ProductBundle.Add(ProductBundle);

            if(SelectedProducts != null)
            {
                List<ProductBundleItem> items = SelectedProducts.Select(s => new ProductBundleItem()
                {
                    ProductBundleItemID = Guid.NewGuid(),
                    ProductBundleID = ProductBundle.ProductBundleID,
                    ProductID = Guid.Parse(s)
                }).ToList();

                _context.ProductBundleItem.AddRange(items);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Products/ProductBundleList");
        }

        private IEnumerable<SelectListItem> GetProducts()
        {
            var products = _context.Product.AsNoTracking().Select(s => new SelectListItem()
            {
                Text = s.Name,
                Value = s.ProductID.ToString(),
                Selected = false
            });
            products = products.OrderBy(o => o.Text);
            return products;
        }

    }
}