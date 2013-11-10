using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAPCO.Infrastructure.Domain;
using System.Web.Mvc;

namespace CAPCO.Models
{
    public class PrintPriceListViewModel
    {
        public PrintPriceListViewModel()
        {
            PriceListProducts = new List<Product>();
        }

        public PricePreferences PriceDisplayPreference { get; set; }
        public ICollection<Product> PriceListProducts { get; set; }
    }

    public class PriceListViewModel
    {
        /// <summary>
        /// Initializes a new instance of the PriceListViewModel class.
        /// </summary>
        public PriceListViewModel()
        {
            //PriceListProducts = new List<Product>();
            AllManufacturers = new List<Manufacturer>();
            SelectedManufacturers = new List<Manufacturer>();
            ProviderCosts = new List<ProductPriceCode>();
        }


        public Dictionary<Manufacturer, Dictionary<ProductSeries, IGrouping<ProductSeries, Product>>> GroupedProducts { get; set; }
        public PricePreferences PriceDisplayPreference { get; set; }
        public List<Product> PriceListProducts { get; set; }
        public ICollection<Manufacturer> AllManufacturers { get; set; }
        public ICollection<Manufacturer> SelectedManufacturers { get; set; }
        public string SelectedSections { get; set; }
        public string SelectedDate { get; set; }
        public IDictionary<string, string> AvailableMonths 
        {
            get
            {
                var months = new Dictionary<string, string>();
                for (int i = 0; i < 3; i++)
                {
                    var date = DateTime.Now.AddMonths(i * -1);
                    months.Add(date.ToShortDateString(), date.ToString("MMMM"));    
                }
                return months;
            }
        }
        public List<ProductPriceCode> ProviderCosts { get; set; }
        public string SearchType { get; set; }
    }
}