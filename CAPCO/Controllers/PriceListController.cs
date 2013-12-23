using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CAPCO.Infrastructure.Domain;
using CAPCO.Models;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Specifications;
using System.IO;
using CAPCO.Infrastructure.Security;

namespace CAPCO.Controllers
{
    [CapcoAuthorizationAttribute(Roles = "ServiceProviders,Administrators")]
    public class PriceListController : ApplicationController
    {
        private readonly IRepository<Product> _ProductRepository;
        private readonly IRepository<Manufacturer> _MfgRepo;
        private readonly IRepository<ProductPriceCode> _ProductPriceCodeRepo;
        /// <summary>
        /// Initializes a new instance of the PriceListController class.
        /// </summary>
        public PriceListController(IRepository<Product> productRepository, IRepository<Manufacturer> mfgRepo, IRepository<ProductPriceCode> productPriceCodeRepo)
        {
            _ProductPriceCodeRepo = productPriceCodeRepo;
            _MfgRepo = mfgRepo;
            _ProductRepository = productRepository;            
        }

        public ActionResult Index()
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            var model = new PriceListViewModel();
            model.PriceDisplayPreference = (PricePreferences)Enum.Parse(typeof(PricePreferences), CurrentUser.PricePreference);
            model.AllManufacturers = _MfgRepo.All.ToList();
            model.SearchType = "all";
            ViewBag.CurrentUser = CurrentUser;
            return View(model);
        }

        public ActionResult All()
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            var model = new PriceListViewModel();
            model.PriceDisplayPreference = (PricePreferences)Enum.Parse(typeof(PricePreferences), CurrentUser.PricePreference);
            var query = _ProductRepository.AllIncluding(x => x.Manufacturer, x => x.ProductSeries, x => x.Usage, x => x.Category, x => x.Color, x => x.Finish, x => x.Group, x => x.Size, x => x.Status, x => x.Type, x => x.UnitOfMeasure, x => x.Variation);
            model.ProviderCosts = new List<ProductPriceCode>();
            var reqPriceCodes = query.Select(x => x.PriceCodeGroup).Distinct();
            model.ProviderCosts = (from ppc in _ProductPriceCodeRepo.All where (reqPriceCodes.Contains(ppc.PriceGroup) && (ppc.PriceCode == CurrentUser.PriceCode || ppc.PriceCode == CurrentUser.RetailCode)) select ppc).ToList();
                        
            model.AllManufacturers = _MfgRepo.All.ToList();
            model.SearchType = "all";
            model.PriceListProducts = query.ToList();
            ViewBag.CurrentUser = CurrentUser;
            return View("index", model);
        }

        
        public ActionResult BySection()
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            var model = new PriceListViewModel();
            model.PriceDisplayPreference = (PricePreferences)Enum.Parse(typeof(PricePreferences), CurrentUser.PricePreference);
            model.AllManufacturers = _MfgRepo.All.ToList();
            model.SearchType = "section";
            ViewBag.CurrentUser = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult BySection(PriceListViewModel model)
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            model.PriceDisplayPreference = (PricePreferences)Enum.Parse(typeof(PricePreferences), CurrentUser.PricePreference);
            try
            {
                model.SelectedManufacturers = new List<Manufacturer>();
                if (Request["SelectedSections"] != null)
                {
                    var selectedSections = Request["SelectedSections"].Split(',');
                    foreach (string section in selectedSections)
                    {
                        var mfg = _MfgRepo.All.FirstOrDefault(x => x.Section == section);
                        if (mfg != null)
                        {
                            model.SelectedManufacturers.Add(mfg);
                        }
                    }
                    if (model.SelectedManufacturers.Any())
                    {
                        var query = _ProductRepository.AllIncluding(x => x.Manufacturer, x => x.ProductSeries, x => x.Usage, x => x.Category, x => x.Color, x => x.Finish, x => x.Group, x => x.Size, x => x.Status, x => x.Type, x => x.UnitOfMeasure, x => x.Variation);
                        model.PriceListProducts = query.Where(u => selectedSections.Contains(u.Section)).ToList();
                        
                        model.ProviderCosts = new List<ProductPriceCode>();
                        var reqPriceCodes = model.PriceListProducts.Select(x => x.PriceCodeGroup).Distinct();
                        model.ProviderCosts = (from ppc in _ProductPriceCodeRepo.All where (reqPriceCodes.Contains(ppc.PriceGroup) && (ppc.PriceCode == CurrentUser.PriceCode || ppc.PriceCode == CurrentUser.RetailCode)) select ppc).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            model.SelectedSections = Request["SelectedSections"];
            
            model.AllManufacturers = _MfgRepo.All.ToList();
            model.SearchType = "section";
            ViewBag.CurrentUser = CurrentUser;
            return View(model);
        }

        public ActionResult ByUpdates()
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            var model = new PriceListViewModel();
            model.PriceDisplayPreference = (PricePreferences)Enum.Parse(typeof(PricePreferences), CurrentUser.PricePreference);
            model.AllManufacturers = _MfgRepo.All.ToList();
            model.SearchType = "updates";
            ViewBag.CurrentUser = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult ByUpdates(PriceListViewModel model)
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");
            model.PriceDisplayPreference = (PricePreferences)Enum.Parse(typeof(PricePreferences), CurrentUser.PricePreference);
            try
            {
                if (Request["SelectedMonth"] != null)
                {
                    var selectedMonth = DateTime.Parse(Request["SelectedMonth"]);
                    var startDate = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
                    var query = _ProductRepository
                        .AllIncluding(x => x.Manufacturer, x => x.ProductSeries, x => x.Usage, x => x.Category, x => x.Color, x => x.Finish, x => x.Group, x => x.Size, x => x.Status, x => x.Type, x => x.UnitOfMeasure, x => x.Variation)
                        .Where(x => x.LastModifiedOn >= startDate);
                    
                    model.ProviderCosts = new List<ProductPriceCode>();
                    var reqPriceCodes = query.Select(x => x.PriceCodeGroup).Distinct();

                    model.ProviderCosts = (from ppc in _ProductPriceCodeRepo.All where (reqPriceCodes.Contains(ppc.PriceGroup) && (ppc.PriceCode == CurrentUser.PriceCode || ppc.PriceCode == CurrentUser.RetailCode)) select ppc).ToList();
                    model.PriceListProducts = query.ToList();
                }                
            }
            catch (Exception ex)
            {

            }


            model.SelectedDate = Request["SelectedMonth"];
            
            model.AllManufacturers = _MfgRepo.All.ToList();
            model.SearchType = "updates";
            ViewBag.CurrentUser = CurrentUser;
            return View(model);
        }

        public ActionResult Print(string type, string criteria = "")
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            PriceListViewModel model = InitializeGridView(type, criteria);
            model.SearchType = type;
            model.SelectedSections = criteria;
            return View(model);
        }

        public ActionResult Data(string type, string criteria = "")
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            PriceListViewModel model = InitializeGridView(type, criteria);
            model.SearchType = type;
            model.SelectedSections = criteria;
            return View(model);
        }

        public ActionResult Export(string type, string criteria = "")
        {
            if (CurrentUser == null)
                return RedirectToAction("index", "root");

            PriceListViewModel model = InitializeGridView(type, criteria);
            
            // build string
            var sw = new StringWriter();
            //write the header
            switch (CurrentUser.PricePreference)
            {
                case "Both":
                    sw.WriteLine("Item,Description,Section,Series,Var,Size,UOM,UPP,CTN,Use,Updated,Retail,Cost");
                    //write items
                    foreach (var item in model.PriceListProducts.Where(x => x.ProductSeries != null).OrderBy(x => x.Section).ThenBy(x => x.ProductSeries.Name).ThenBy(x => x.ItemNumber))
                    {
                        var ppc = model.ProviderCosts.FirstOrDefault(x => x.PriceGroup == item.PriceCodeGroup && x.PriceCode == CurrentUser.PriceCode);
                        var ppr = model.ProviderCosts.FirstOrDefault(x => x.PriceGroup == item.PriceCodeGroup && x.PriceCode == CurrentUser.RetailCode);
                        sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                                         item.ItemNumber,
                                         item.Description.Replace(',', ' '),
                                         item.Section,
                                         item.ProductSeries != null ? item.ProductSeries.Name : item.Series,
                                         item.Variation != null ? item.Variation.Name : "",
                                         item.Size != null ? item.Size.Name : "",
                                         item.UnitOfMeasure != null ? item.UnitOfMeasure.Name : "",
                                         item.UnitsPerPiece,
                                         item.CartonQuantity,
                                         item.Usage != null ? "\"" + item.Usage.Name + "\"" : "",
                                         item.LastModifiedOn.ToShortDateString(),
                                         ppr != null ? ppr.Price : item.RetailPrice,
                                         ppc != null ? ppc.Price : item.RetailPrice));
                    }
                    break;
                case "CostOnly":
                    sw.WriteLine("Item,Description,Section,Series,Var,Size,UOM,UPP,CTN,Use,Updated,Cost");
                    //write items
                    foreach (var item in model.PriceListProducts)
                    {
                        var ppc = model.ProviderCosts.FirstOrDefault(x => x.PriceGroup == item.PriceCodeGroup && x.PriceCode == CurrentUser.PriceCode);
                        sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                                         item.ItemNumber,
                                         item.Description.Replace(',', ' '),
                                         item.Section,
                                         item.ProductSeries != null ? item.ProductSeries.Name : item.Series,
                                         item.Variation != null ? item.Variation.Name : "",
                                         item.Size != null ? item.Size.Name : "",
                                         item.UnitOfMeasure != null ? item.UnitOfMeasure.Name : "",
                                         item.UnitsPerPiece,
                                         item.CartonQuantity,
                                         item.Usage != null ? "\"" + item.Usage.Name + "\"" : "",
                                         item.LastModifiedOn.ToShortDateString(),
                                         ppc != null ? ppc.Price : item.RetailPrice));
                    }
                    break;
                case "RetailOnly":
                    sw.WriteLine("Item,Description,Section,Series,Var,Size,UOM,UPP,CTN,Use,Updated,Retail");
                    //write items
                    foreach (var item in model.PriceListProducts)
                    {
                        var ppr = model.ProviderCosts.FirstOrDefault(x => x.PriceGroup == item.PriceCodeGroup && x.PriceCode == CurrentUser.RetailCode);
                        sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                                         item.ItemNumber,
                                         item.Description.Replace(',', ' '),
                                         item.Section,
                                         item.ProductSeries != null ? item.ProductSeries.Name : item.Series,
                                         item.Variation != null ? item.Variation.Name : "",
                                         item.Size != null ? item.Size.Name : "",
                                         item.UnitOfMeasure != null ? item.UnitOfMeasure.Name : "",
                                         item.UnitsPerPiece,
                                         item.CartonQuantity,
                                         item.Usage != null ? "\"" + item.Usage.Name + "\"" : "",
                                         item.LastModifiedOn.ToShortDateString(),
                                         ppr != null ? ppr.Price : item.RetailPrice));
                    }
                    break;
                default: // None
                    sw.WriteLine("Item,Description,Section,Series,Var,Size,UOM,UPP,CTN,Use,Updated");
                    //write items
                    foreach (var item in model.PriceListProducts)
                    {
                        sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                                         item.ItemNumber,
                                         item.Description.Replace(',', ' '),
                                         item.Section,
                                         item.ProductSeries != null ? item.ProductSeries.Name : item.Series,
                                         item.Variation != null ? item.Variation.Name : "",
                                         item.Size != null ? item.Size.Name : "",
                                         item.UnitOfMeasure != null ? item.UnitOfMeasure.Name : "",
                                         item.UnitsPerPiece,
                                         item.CartonQuantity,
                                         item.Usage != null ? "\"" + item.Usage.Name + "\"" : "",
                                         item.LastModifiedOn.ToShortDateString()));
                    }
                    break;
            }
            
            // build filename
            string fileName = String.Format("CAPCO-export-{0}.csv", DateTime.Now.Ticks);

            // return file
            return File(new System.Text.UTF8Encoding().GetBytes(sw.ToString()), "text/csv", fileName);
        }

        private PriceListViewModel InitializeGridView(string type, string criteria)
        {
            var model = new PriceListViewModel();
            model.PriceDisplayPreference = (PricePreferences)Enum.Parse(typeof(PricePreferences), CurrentUser.PricePreference);
            var query = _ProductRepository.AllIncluding(x => x.Manufacturer, x => x.ProductSeries, x => x.Usage, x => x.Category, x => x.Color, x => x.Finish, x => x.Group, x => x.Size, x => x.Status, x => x.Type, x => x.UnitOfMeasure, x => x.Variation);
            switch (type)
            {
                case "section":
                    var selectedManufacturers = new List<Manufacturer>();
                    if (!String.IsNullOrWhiteSpace(criteria))
                    {
                        var selectedSections = criteria.Split(',');
                        selectedManufacturers.AddRange(selectedSections.Select(section => this._MfgRepo.All.FirstOrDefault(x => x.Section == section)).Where(mfg => mfg != null));
                        if (selectedManufacturers.Any())
                        {
                            model.PriceListProducts = query.Where(u => selectedSections.Contains(u.Section)).ToList();
                        }
                    }
                    break;
                case "updates":
                    if (!String.IsNullOrWhiteSpace(criteria))
                    {
                        var selectedMonth = DateTime.Parse(criteria);
                        var startDate = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
                        model.PriceListProducts = query.Where(x => x.LastModifiedOn >= startDate).ToList();
                    }
                    break;
                default:
                    model.PriceListProducts = query.ToList();
                    break;
            }

            model.ProviderCosts = new List<ProductPriceCode>();
            var reqPriceCodes = model.PriceListProducts.Select(x => x.PriceCodeGroup).Distinct();
            model.ProviderCosts = (from ppc in _ProductPriceCodeRepo.All where (reqPriceCodes.Contains(ppc.PriceGroup) && (ppc.PriceCode == CurrentUser.PriceCode || ppc.PriceCode == CurrentUser.RetailCode)) select ppc).ToList();
            
            ViewBag.CurrentUser = CurrentUser;
            model.SearchType = type;
            
            return model;
        }

        
    }
}
