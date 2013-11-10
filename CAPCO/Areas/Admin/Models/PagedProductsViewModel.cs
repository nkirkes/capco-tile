using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Areas.Admin.Models
{
    public class PagedViewModel<T>
    {
        public int? Page { get; set; }
        public IPagedList<T> Entities { get; set; }
        public int TotalCount { get; set; }
        public string Criteria { get; set; }
    }

    public class PagedPriceCodesViewModel
    {
        public int? Page { get; set; }
        public IPagedList<ProductPriceCode> PagedPriceCodes { get; set; }
        public int TotalCount { get; set; }
    }

    public class PagedProductsViewModel
    {
        public int? Page { get; set; }
        public IPagedList<Product> PagedProducts { get; set; }
        public int ProductsCount { get; set; }
        public string Criteria { get; set; }
    }
}