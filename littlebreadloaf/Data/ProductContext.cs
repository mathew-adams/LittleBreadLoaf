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
        public virtual DbSet<ProductOrder> ProductOrder { get; set; }
        public virtual DbSet<ProductOrderOutage> ProductOrderOutage { get; set; }
        public virtual DbSet<ProductBundle> ProductBundle { get; set; }
        public virtual DbSet<ProductBundleItem> ProductBundleItem { get; set; }

        public virtual DbSet<littlebreadloaf.Data.Cart> Cart { get; set; }
        public virtual DbSet<CartItem> CartItem { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceTransaction> InvoiceTransaction { get; set; }
        public virtual DbSet<NzAddressDeliverable> NzAddressDeliverable { get; set; }

        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<SourceToTag> SourceToTag { get; set; }
        public virtual DbSet<BlogCategory> BlogCategory { get; set; }
        public virtual DbSet<CategoryToBlog> CategoryToBlog { get; set; }
        public virtual DbSet<BlogImage> BlogImage { get; set; }

        public virtual DbSet<UserProfile> UserProfile { get; set; }

        public virtual DbSet<BusinessSettings> BusinessSettings { get; set; }

        public virtual DbSet<littlebreadloaf.ConfigurationProvider.LittleBreadLoafSystem> LittleBreadLoafSystem { get; set; }
        public virtual DbSet<SystemError> SystemError { get; set; }

        public virtual DbSet<PreOrderSource> PreOrderSource { get; set; }
    }
}
