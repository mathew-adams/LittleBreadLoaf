using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace littlebreadloaf.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }
        
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductBadge> ProductBadge { get; set; }
        public virtual DbSet<ProductIngredient> ProductIngredient { get; set; }
        public virtual DbSet<ProductSuggestion> ProductSuggestion { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }

        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartItem> CartItem { get; set; }
        
    }
}
