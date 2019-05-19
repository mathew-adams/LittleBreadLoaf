using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Products
{
    public class ProductBundleView
    {
        public Product Product { get; set; }
        public List<ProductBadge> ProductBadges { get; set; }
        public ProductImage ProductImage { get; set; }
        public bool Outage { get; set; }
    }

    public class ProductBundleViewModel : PageModel
    {
        private readonly ProductContext _context;
        public ProductBundleViewModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string ProductBundleID { get; set; }

        [BindProperty]
        public ProductBundle ProductBundle { get; set; }

        [BindProperty]
        public List<ProductBundleView> Products { get; set; }

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

            var products = await _context
                                .Product
                                .Join(_context.ProductBundleItem,
                                      p => p.ProductID, pbi => pbi.ProductID, (p, pbi) => new { p.ProductID, p.Name, p.Price, p.Unit, p.Description, pbi.ProductBundleID })
                                .Where(w => w.ProductBundleID == parsedID)
                                .Select(s => new Product()
                                {
                                    ProductID = s.ProductID,
                                    Name = s.Name,
                                    Description = s.Description,
                                    Unit = s.Unit,
                                    Price = s.Price
                                }).ToListAsync();

            var images = await _context
                                .ProductImage
                                .Join(_context.ProductBundleItem,
                                    pi => pi.ProductID, pbi => pbi.ProductID, (pi, pbi) => new { pi.ProductImageID, pi.Title, pi.FileLocation, pi.ProductID, pi.PrimaryImage, pbi.ProductBundleID })
                                .Where(w => w.ProductBundleID == parsedID && w.PrimaryImage == true)
                                .Select(s => new ProductImage()
                                {
                                    ProductImageID = s.ProductImageID,
                                    ProductID = s.ProductID,
                                    FileLocation = s.FileLocation,
                                    Title = s.Title
                                }).ToListAsync();

            var badges = await _context
                                .ProductBadge
                                .Join(_context.ProductBundleItem,
                                    pb => pb.ProductID, pbi => pbi.ProductID, (pb, pbi) => new { pb.ProductID, pb.ProductBadgeID, pb.Title, pb.Description, pbi.ProductBundleID })
                                .Where(w => w.ProductBundleID == parsedID)
                                .Select(s => new ProductBadge()
                                {
                                    ProductBadgeID = s.ProductBadgeID,
                                    ProductID = s.ProductID,
                                    Title = s.Title,
                                    Description = s.Description

                                }).ToListAsync();

            var outages = await _context.ProductOrderOutage.AsNoTracking().ToListAsync();
            bool outage = outages.Count > 0 && outages.Any(a => a.Start <= DateTime.Now && a.Stop >= DateTime.Now);

            Products = products.Select(s => new ProductBundleView()
            {
                Product = products.FirstOrDefault(f => f.ProductID == s.ProductID),
                ProductBadges = badges.Where(w => w.ProductID == s.ProductID).ToList(),
                ProductImage = images.Where(w => w.ProductID == s.ProductID).FirstOrDefault(),
                Outage = outage
            }).ToList();

            return Page();
        }
    }
}