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
    public class ProductBundleEditModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductBundleEditModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string ProductBundleID { get; set; }

        [BindProperty]
        public ProductBundle ProductBundle { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> Products { get; set; }

        [BindProperty]
        public IEnumerable<string> SelectedProducts { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(ProductBundleID) || !Guid.TryParse(ProductBundleID, out Guid parsedID))
            {
                return new RedirectResult("/Products/ProductBundleList");
            }

            ProductBundle = await _context.ProductBundle.FirstOrDefaultAsync(m => m.ProductBundleID == parsedID);
            if (ProductBundle == null)
            {
                return new RedirectResult("/Products/ProductBundleList");
            }

            Products = await GetProducts();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Products = await GetProducts();

            if(!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(ProductBundleID) || !Guid.TryParse(ProductBundleID, out Guid parsedID))
            {
                return new RedirectResult("/Products/ProductBundleList");
            }

            _context.ProductBundle.Update(ProductBundle);
            _context.ProductBundleItem.RemoveRange(_context.ProductBundleItem.Where(w => w.ProductBundleID == parsedID));

            if (SelectedProducts != null && SelectedProducts.Count() > 0)
            {
                _context.ProductBundleItem.AddRange(SelectedProducts
                                                    .Select(s => new ProductBundleItem()
                                                                {
                                                                    ProductBundleItemID = Guid.NewGuid(),
                                                                    ProductBundleID = parsedID,
                                                                    ProductID = Guid.Parse(s)
                                                                }));
            }

            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Products/ProductBundleView", new { ProductBundle.ProductBundleID });
        }

        private async Task<IEnumerable<SelectListItem>> GetProducts()
        {
            var products = await _context.Product.GroupJoin(_context.ProductBundleItem,
                                                            p => p.ProductID,
                                                            pbi => pbi.ProductID, (p, pbi) => new { Product = p, ProductBundleItem = pbi })
                                                .SelectMany(xy => xy.ProductBundleItem
                                                                    .Where(w => w.ProductBundleID == ProductBundle.ProductBundleID)
                                                                    .DefaultIfEmpty(),
                                                                    (p, pbi) => new { p.Product, ProductBundleItem = pbi })
                                                .Select(s => new SelectListItem
                                                {
                                                    Text = s.Product.Name,
                                                    Value = s.Product.ProductID.ToString(),
                                                    Selected = s.Product.ProductID == s.ProductBundleItem.ProductID
                                                })
                                                .ToListAsync();
            return products.OrderBy(o => o.Value);
            //var products = _context.Product.AsNoTracking().Select(s => new SelectListItem()
            //{
            //    Text = s.Name,
            //    Value = s.ProductID.ToString(),
            //    Selected = false
            //});
            //products = products.OrderBy(o => o.Text);
            //return products;
        }

    }
}