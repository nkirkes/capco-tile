using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Configuration;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Data
{
    public class CAPCOContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the ThePypeContext class.
        /// </summary>
        public CAPCOContext() : base(ConfigurationManager.ConnectionStrings["CAPCO.Web"].ConnectionString)
        {
            
        }

        public DbSet<ProductGroup> ProductGroups { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductColor> ProductColors { get; set; }

        public DbSet<ProductSize> ProductSizes { get; set; }

        public DbSet<ProductFinish> ProductFinishes { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Project> ProductBundles { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        //public DbSet<ContentPage> ContentPages { get; set; }

        public DbSet<PriceCode> PriceCodes { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProductStatus> ProductStatus { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProductVariation> ProductVariations { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProductUnitOfMeasure> ProductUnitOfMeasures { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProjectComment> ProjectComments { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProjectInvitation> ProjectInvitations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasMany<ApplicationUser>(c => c.Users).WithMany();
            
            modelBuilder.Entity<Product>()
                .HasMany<Product>(x => x.RelatedSizes)
                .WithMany()
                .Map(x => x.ToTable("ProductRelatedSizes"));

            modelBuilder.Entity<Product>()
                .HasMany<Product>(x => x.RelatedAccents)
                .WithMany()
                .Map(x => x.ToTable("ProductRelatedAccents"));

            modelBuilder.Entity<Product>()
                .HasMany<Product>(x => x.RelatedTrims)
                .WithMany()
                .Map(x => x.ToTable("ProductRelatedTrims"));

            modelBuilder.Entity<Product>()
                .HasMany<Product>(x => x.RelatedFinishes)
                .WithMany()
                .Map(x => x.ToTable("ProductRelatedFinishes"));
        }

        public DbSet<CAPCO.Infrastructure.Domain.ContentSection> ContentSections { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.Manufacturer> Manufacturers { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.PickupLocation> PickupLocations { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.DiscountCode> DiscountCodes { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.StoreLocation> StoreLocations { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ContactRequest> ContactRequests { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.AccountRequest> AccountRequests { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProductUsage> ProductUsages { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.RelatedProductSize> OtherSizes { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.RelatedAccent> RelatedAccents { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.RelatedTrim> RelatedTrims { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProductPriceCode> ProductPriceCodes { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.Link> Links { get; set; }

        public DbSet<CAPCO.Infrastructure.Domain.ProductSeries> ProductSeries { get; set; }

        //public DbSet<CAPCO.Infrastructure.Domain.ProductCrossReference> ProductCrossReferences { get; set; }

    }
}