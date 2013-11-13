using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;

namespace CAPCO.Infrastructure.Domain
{
    public class Product : Entity
    {
        public Product()
        {
            RelatedSizes = new List<Product>();
            RelatedAccents = new List<Product>();
            RelatedTrims = new List<Product>();
            RelatedFinishes = new List<Product>();
            //PriceCodes = new List<ProductPriceCode>();
        }

        public string PriceCodeGroup { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string Name { get; set; }
        [DisplayName("Item Number")]
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public string Series { get; set; }
        public decimal RetailPrice { get; set; }
        [DisplayName("YouTube Url")]
        public string YouTubeUrl { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
        [DisplayName("Manufacturer Color")]
        public string ManufacturerColor { get; set; }
        [DisplayName("Manufacturer Item")]
        public string ManufacturerItem { get; set; }

        [DisplayName("Size")]
        public string SizeDescription { get; set; }
        [DisplayName("Made In")]
        public string MadeIn { get; set; }
        [DisplayName("Carton Quantity")]
        public decimal? CartonQuantity { get; set; }
        [DisplayName("Coefficient of Friction - Dry")]
        public decimal? CoefficientOfFrictionDry { get; set; }
        [DisplayName("Coefficient of Friction - Wet")]
        public decimal? CoefficientOfFrictionWet { get; set; }
        [DisplayName("Breaking Strength")]
        public string BreakingStrength { get; set; }
        [DisplayName("Water Absorption")]
        public string WaterAbsorption { get; set; }
        //[DisplayName("Chemical Resistance")]
        //public string ChemicalResistance { get; set; }
        [DisplayName("Scratch Hardiness")]
        public string ScratchHardiness { get; set; }
        [DisplayName("Units per Piece")]
        public decimal? UnitsPerPiece { get; set; }

        [DisplayName("Is Frost Resistant?")]
        public bool IsFrostResistant { get; set; }

        [DisplayName("Is Chemical Resistant?")]
        public bool IsChemicalResistant { get; set; }

        public string Section
        {
            get;
            set; 
        }

        //[DisplayName("Price Codes")]
        //public virtual ICollection<ProductPriceCode> PriceCodes { get; set; }
        
        /* Product Properties - Used primarily for search */
        //[ForeignKey("ProductGroup")]
        //public int? ProductGroupId { get; set; }
        [DisplayName("Group")]
        public ProductGroup Group { get; set; }

        //[ForeignKey("ProductStatus")]
        //public int? ProductStatusId { get; set; }
        [DisplayName("Status")]
        public ProductStatus Status { get; set; }
        [DisplayName("Status Changed On")]
        public DateTime? StatusChangedOn { get; set; }
        
        //[ForeignKey("ProductUnitOfMeasure")]
        //public int? ProductUnitOfMeasureId { get; set; }
        [DisplayName("Unit of Measure")]
        public ProductUnitOfMeasure UnitOfMeasure { get; set; }

        public ProductSeries ProductSeries { get; set; }
        public ProductUsage Usage { get; set; }

        //[ForeignKey("ProductVariation")]
        //public int? ProductVariationId { get; set; }
        [DisplayName("Variation")]
        public ProductVariation Variation { get; set; }

        //[ForeignKey("ProductCategory")]
        //public int? ProductCategoryId { get; set; }
        [DisplayName("Category")]
        public ProductCategory Category { get; set; }

        //[ForeignKey("ProductType")]
        //public int? ProductTypeId { get; set; }
        [DisplayName("Type")]
        public ProductType Type { get; set; }

        //[ForeignKey("ProductColor")]
        //public int? ProductColorId { get; set; }
        [DisplayName("Color")]
        public ProductColor Color { get; set; }

        //[ForeignKey("ProductSize")]
        //public int? ProductSizeId { get; set; }
        [DisplayName("Size Group")]
        public ProductSize Size { get; set; }

        //[ForeignKey("ProductFinish")]
        //public int? ProductFinishId { get; set; }

        [DisplayName("Finish")]
        public ProductFinish Finish { get; set; }

        [DisplayName("Parent Product")]
        public Product ParentProduct { get; set; }

        [NotMapped, DisplayName("Related Products")]
        public ICollection<Product> RelatedProducts 
        { 
            get 
            {
                var result = new List<Product>();
                result.AddRange(RelatedSizes);
                result.AddRange(RelatedAccents);
                result.AddRange(RelatedTrims);
                result.AddRange(RelatedFinishes);
                return result;
            } 
        }

        public virtual ICollection<Product> RelatedSizes { get; set; }

        public virtual ICollection<Product> RelatedAccents { get; set; }

        public virtual ICollection<Product> RelatedTrims { get; set; }

        public virtual ICollection<Product> RelatedFinishes { get; set; }
        
        
        [DisplayName("Projects")]
        public virtual ICollection<Project> ProductBundles { get; set; }

        //[DisplayName("Images")]
        //public virtual ICollection<ProductImage> ProductImages { get; set; }

        [NotMapped]
        public string DetailImageFileName
        {
            get 
            {
                return ItemNumber + "CL";
            }
        }
        
        [NotMapped]
        public string LargeImageFileName
        {
            get
            {
                return ItemNumber + "LG";
            }
        }

        [NotMapped]
        public string SmallImageFileName
        {
            get
            {
                return ItemNumber + "MU";
            }
        }

        [NotMapped]
        public string ThumbnailImageFileName
        {
            get
            {
                return ItemNumber + "MU";
            }
        }
    }
}
