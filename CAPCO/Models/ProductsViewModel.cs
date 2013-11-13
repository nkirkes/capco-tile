using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAPCO.Infrastructure.Domain;
using PagedList;

namespace CAPCO.Models
{
    public class PagedProductsList
    {
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public List<Product> Products { get; set; }
        public int Count { get; set; }
    }

    public class ProductsViewModel
    {
        public static int PageSize = 24;
        public ProductsViewModel()
        {
            AllProducts = new List<Product>().AsQueryable();
        }

        public ProductFilters Filters { get; set; }
        //public IPagedList<Product> Products { get; set; }
        public List<Product> Products { get; set; }
        public IQueryable<Product> AllProducts { get; set; }
        public List<ProductGroup> AvailableGroups { get; set; }
        public int ProductsInGroups { get; set; }
        public List<ProductCategory> AvailableCategories { get; set; }
        public int ProductsInCategories { get; set; }
        public List<ProductType> AvailableTypes { get; set; }
        public int ProductsInTypes { get; set; }
        public List<ProductSize> AvailableSizes { get; set; }
        public int ProductsInSizes { get; set; }
        public List<ProductColor> AvailableColors { get; set; }
        public int ProductsInColors { get; set; }
        public List<ProductFinish> AvailableFinishes { get; set; }
        public int ProductsInFinishes { get; set; }
        public bool HasFilters 
        { 
            get 
            {
                return Filters != null && (
                    Filters.CategoryId.HasValue ||
                    Filters.ColorId > 0 ||
                    Filters.FinishId > 0 ||
                    Filters.GroupId > 0 ||
                    Filters.SizeId > 0 ||
                    Filters.TypeId > 0 ||
                    !String.IsNullOrWhiteSpace(Filters.Series) ||
                    !String.IsNullOrWhiteSpace(Filters.ItemNumber));
            } 
        }
        public int? Page { get; set; }
        public int TotalCount { get; set; }
    }
}