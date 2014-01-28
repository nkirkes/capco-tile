using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Configuration;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Data
{
    public interface IDataContext : IDisposable
    {
        DbSet<ProductGroup> ProductGroups { get; set; }

        DbSet<ProductType> ProductTypes { get; set; }

        DbSet<ProductCategory> ProductCategories { get; set; }

        DbSet<ProductColor> ProductColors { get; set; }

        DbSet<ProductSize> ProductSizes { get; set; }

        DbSet<ProductFinish> ProductFinishes { get; set; }

        DbSet<Product> Products { get; set; }

        DbSet<Project> ProductBundles { get; set; }

        DbSet<Notification> Notifications { get; set; }

        DbSet<ApplicationUser> ApplicationUsers { get; set; }

        DbSet<PriceCode> PriceCodes { get; set; }

        DbSet<ProductStatus> ProductStatus { get; set; }

        DbSet<ProductVariation> ProductVariations { get; set; }

        DbSet<ProductUnitOfMeasure> ProductUnitOfMeasures { get; set; }

        DbSet<ProductImage> ProductImages { get; set; }

        DbSet<ProjectComment> ProjectComments { get; set; }

        DbSet<ProjectInvitation> ProjectInvitations { get; set; }

        DbSet<SliderImage> SliderImages { get; set; }

        DbSet<ContentSection> ContentSections { get; set; }

        DbSet<Manufacturer> Manufacturers { get; set; }

        DbSet<PickupLocation> PickupLocations { get; set; }

        DbSet<DiscountCode> DiscountCodes { get; set; }

        DbSet<StoreLocation> StoreLocations { get; set; }

        DbSet<ContactRequest> ContactRequests { get; set; }

        DbSet<AccountRequest> AccountRequests { get; set; }

        DbSet<ProductUsage> ProductUsages { get; set; }

        DbSet<RelatedProductSize> OtherSizes { get; set; }

        DbSet<RelatedAccent> RelatedAccents { get; set; }

        DbSet<RelatedTrim> RelatedTrims { get; set; }

        DbSet<ProductPriceCode> ProductPriceCodes { get; set; }

        DbSet<Link> Links { get; set; }

        DbSet<ProductSeries> ProductSeries { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);
        int SaveChanges();
    }

    public class CAPCOContext : DbContext, IDataContext
    {
        /// <summary>
        /// Initializes a new instance of the data context class.
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
        
        public DbSet<PriceCode> PriceCodes { get; set; }

        public DbSet<ProductStatus> ProductStatus { get; set; }

        public DbSet<ProductVariation> ProductVariations { get; set; }

        public DbSet<ProductUnitOfMeasure> ProductUnitOfMeasures { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<ProjectComment> ProjectComments { get; set; }

        public DbSet<ProjectInvitation> ProjectInvitations { get; set; }

        public DbSet<SliderImage> SliderImages { get; set; }

        public DbSet<ContentSection> ContentSections { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<PickupLocation> PickupLocations { get; set; }

        public DbSet<DiscountCode> DiscountCodes { get; set; }

        public DbSet<StoreLocation> StoreLocations { get; set; }

        public DbSet<ContactRequest> ContactRequests { get; set; }

        public DbSet<AccountRequest> AccountRequests { get; set; }

        public DbSet<ProductUsage> ProductUsages { get; set; }

        public DbSet<RelatedProductSize> OtherSizes { get; set; }

        public DbSet<RelatedAccent> RelatedAccents { get; set; }

        public DbSet<RelatedTrim> RelatedTrims { get; set; }

        public DbSet<ProductPriceCode> ProductPriceCodes { get; set; }

        public DbSet<Link> Links { get; set; }

        public DbSet<ProductSeries> ProductSeries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasMany<ApplicationUser>(c => c.Users).WithMany().Map(x => x.ToTable("ProjectApplicationUsers"));

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
    }
}