using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Http;

namespace littlebreadloaf.Pages.Products
{
    public class ProductPreOrderModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductPreOrderModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<Product> Products { get; set; }

        [BindProperty]
        public List<ProductBadge> ProductBadges { get; set; }

        [BindProperty]
        public List<ProductImage> ProductImages { get; set; }

        [BindProperty]
        public List<ProductOrderOutage> ProductOrderOutages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Source { get; set; }

        [BindProperty]
        public bool IsPreOrder { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Source))
                return new RedirectToPageResult("/Products/ProductList");

            if(! await _context.PreOrderSource.AnyAsync(a => a.Source == Source))
            {
                return new RedirectToPageResult("/Products/ProductList");
            }

            var cookieOptions = new CookieOptions
            {
                IsEssential = true //Prevents the "accept cookies" dialog from preventing this cookie from being sent
            };

            HttpContext.Response.Cookies.Append(CartHelper.PreOrderCookie, Source, cookieOptions);

            IsPreOrder = true;

            Products = await _context
                            .Product
                            .AsNoTracking()
                            .ToListAsync();
            Products = Products.OrderBy(o => o.SortOrder).ToList();
            ProductBadges = await _context.ProductBadge.AsNoTracking().ToListAsync();
            ProductImages = await _context.ProductImage.AsNoTracking().Where(m => m.PrimaryImage == true).ToListAsync();
            ProductOrderOutages = await _context.ProductOrderOutage.AsNoTracking().ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCartAddAsync(string productID)
        {
            if (String.IsNullOrEmpty(productID) || !Guid.TryParse(productID, out Guid parsedID))
            {
                return new RedirectResult("/Products/ProductPreOrder");
            }

            return await CartHelper.AddToCart(_context,
                                              parsedID,
                                              User,
                                              HttpContext);
        }

    }
}