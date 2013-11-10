using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Models
{
    public class ProductSearchCriteria
    {
        public ProductSearchCriteria()
        {
            AvailableGroups = new List<ProductGroup>();
            AvailableCategories = new List<ProductCategory>();
            AvailableTypes = new List<ProductType>();
            AvailableSizes = new List<ProductSize>();
            AvailableColors = new List<ProductColor>();
            AvailableFinishes = new List<ProductFinish>();
            Products = new List<Product>();
        }

        public ProductFilters Filters { get; set; }
        public IEnumerable<Product> Products { get; set; }

        public string ItemNumber { get; set; }
        public string Series { get; set; }
        
        public int SelectedProductGroup { get; set; }
        public int SelectedFinish { get; set; }
        public int SelectedColor { get; set; }
        public int SelectedCategory { get; set; }
        public int SelectedType { get; set; }
        public int SelectedSize { get; set; }

        public List<ProductGroup> AvailableGroups { get; set; }
        public List<ProductCategory> AvailableCategories { get; set; }
        public List<ProductType> AvailableTypes { get; set; }
        public List<ProductSize> AvailableSizes { get; set; }
        public List<ProductColor> AvailableColors { get; set; }
        public List<ProductFinish> AvailableFinishes { get; set; }
        
    }
}