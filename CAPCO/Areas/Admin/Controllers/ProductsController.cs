using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;
using System.Web.Security;
using CAPCO.Areas.Admin.Models;
using PagedList;
using CAPCO.Infrastructure.Specifications;


namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductsController : BaseAdminController
    {
		private readonly IRepository<ProductGroup> _productgroupRepository;
		private readonly IRepository<ProductCategory> _productcategoryRepository;
        private readonly IRepository<ProductStatus> _ProductStatusRepo;
        private readonly IRepository<ProductType> _producttypeRepository;
		private readonly IRepository<ProductColor> _productcolorRepository;
		private readonly IRepository<ProductSize> _productsizeRepository;
		private readonly IRepository<ProductFinish> _productfinishRepository;
		private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariation> _VariationRepo;
        private readonly IRepository<ProductUnitOfMeasure> _UomRepo;
        private readonly IRepository<Manufacturer> _ManufacturerRepo;
        private readonly IRepository<ProductUsage> _ProductUsageRepo;
        private readonly IRepository<Project> _ProjectRepo;
  
  
  
		public ProductsController(IRepository<ProductGroup> productgroupRepository, 
            IRepository<ProductCategory> productcategoryRepository, 
            IRepository<ProductType> producttypeRepository, 
            IRepository<ProductColor> productcolorRepository, 
            IRepository<ProductSize> productsizeRepository, 
            IRepository<ProductFinish> productfinishRepository, 
            IRepository<Product> productRepository,
            IRepository<ProductStatus> productStatusRepo,
            IRepository<ProductUnitOfMeasure> uomRepo,
            IRepository<ProductVariation> variationRepo,
            IRepository<Manufacturer> manufacturerRepo,
            IRepository<ProductUsage> productUsageRepo,
            IRepository<Project> projectRepo)
        {
            _ProjectRepo = projectRepo;
            _ProductUsageRepo = productUsageRepo;
            _ManufacturerRepo = manufacturerRepo;
            _ProductStatusRepo = productStatusRepo;
            _UomRepo = uomRepo;
            _VariationRepo = variationRepo;
            this._productgroupRepository = productgroupRepository;
			this._productcategoryRepository = productcategoryRepository;
			this._producttypeRepository = producttypeRepository;
			this._productcolorRepository = productcolorRepository;
			this._productsizeRepository = productsizeRepository;
			this._productfinishRepository = productfinishRepository;
			this._productRepository = productRepository;
        }

        public ViewResult Index(PagedProductsViewModel model)
        {
            if (Request.Cookies["SearchCriteria"] != null)
            {
                var cookie = Request.Cookies["SearchCriteria"];
                model.Page = Int32.Parse(cookie.Values["Page"]);
            }
            
            // TODO: Ok, need to figure out when to clear the cookie out or ignore it. Maybe a clear search button?

            var results = _productRepository.AllIncluding(x => x.Manufacturer, x => x.Group).OrderBy(x => x.ItemNumber);
            model.ProductsCount = results.Count();

            
            
            model.PagedProducts = results.ToPagedList(model.Page ?? 1, 100);

            

            return View("Index", model);
        }

        public ActionResult Search(PagedProductsViewModel model)
        {
            var results = _productRepository.FindBySpecification(new ProductsByItemNumberSpecification(model.Criteria.Trim()));
            model.ProductsCount = results.Count();
            model.PagedProducts = results.ToPagedList(model.Page ?? 1, 100);

            SetSearchCookie("search", model.Criteria, model.Page ?? 1);
            
            return View("Index", model);
        }

        public ViewResult Show(int id)
        {
            return View(_productRepository.Find(id));
        }

        public ActionResult New()
        {
            InitSelectLists();
            return View();
        }

        private void InitSelectLists()
        {
            ViewBag.PossibleProductGroups = _productgroupRepository.All;
            ViewBag.PossibleProductCategories = _productcategoryRepository.All;
            ViewBag.PossibleProductTypes = _producttypeRepository.All;
            ViewBag.PossibleProductColors = _productcolorRepository.All;
            ViewBag.PossibleProductSizes = _productsizeRepository.All;
            ViewBag.PossibleProductFinishes = _productfinishRepository.All;
            ViewBag.PossibleProductStatuses = _ProductStatusRepo.All;
            ViewBag.PossibleProductVariations = _VariationRepo.All;
            ViewBag.PossibleProductUoMs = _UomRepo.All;
            ViewBag.PossibleManufacturers = _ManufacturerRepo.All;
            ViewBag.PossibleUsages = _ProductUsageRepo.All;
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(Product product)
        {
            try
            {
                string userName = CurrentUser.UserName;
                product.CreatedBy = userName;
                product.CreatedOn = DateTime.Now;
                product.LastModifiedBy = userName;
                product.LastModifiedOn = DateTime.Now;

                int mfgId = 0;
                if (Int32.TryParse(Request["SelectedManufacturer"], out mfgId))
                {
                    product.Manufacturer = _ManufacturerRepo.Find(mfgId);
                    product.Section = product.Manufacturer != null ? product.Manufacturer.Section : String.Empty;
                }

                product.Series = product.Series;

                int usageId = 0;
                if (Int32.TryParse(Request["SelectedUsage"], out usageId))
                {
                    var usage = _ProductUsageRepo.Find(usageId);
                    if (usage != null)
                        product.Usage = usage;
                }

                int catId = 0;
                if (Int32.TryParse(Request["SelectedCategory"], out catId))
                {
                    product.Category = _productcategoryRepository.Find(catId);
                }

                int colorId = 0;
                if (Int32.TryParse(Request["SelectedColor"], out colorId))
                    product.Color = _productcolorRepository.Find(colorId);

                int finishId = 0;
                if (Int32.TryParse(Request["SelectedFinish"], out finishId))
                    product.Finish = _productfinishRepository.Find(finishId);

                int groupId = 0;
                if (Int32.TryParse(Request["SelectedGroup"], out groupId))
                    product.Group = _productgroupRepository.Find(groupId);

                int sizeId = 0;
                if (Int32.TryParse(Request["SelectedSize"], out sizeId))
                    product.Size = _productsizeRepository.Find(sizeId);

                int typeId = 0;
                if (Int32.TryParse(Request["SelectedType"], out typeId))
                    product.Type = _producttypeRepository.Find(typeId);

                int varId = 0;
                if (Int32.TryParse(Request["SelectedVariation"], out varId))
                    product.Variation = _VariationRepo.Find(varId);

                int uomId = 0;
                if (Int32.TryParse(Request["SelectedUoM"], out uomId))
                    product.UnitOfMeasure = _UomRepo.Find(uomId);

                int statusId = 0;
                if (Int32.TryParse(Request["SelectedStatus"], out statusId))
                    product.Status = _ProductStatusRepo.Find(statusId);

                _productRepository.InsertOrUpdate(product);
                _productRepository.Save();

                this.FlashInfo("The new product was saved successfully.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem creating the product: " + ex.Message);	
            }

            InitSelectLists();
            return View("New", product);
            
        }

        public ActionResult Edit(int id)
        {
            Product prod = _productRepository.AllIncluding(x => x.Category, x => x.Color, x => x.Finish, x => x.Group, x => x.Manufacturer, x => x.Size, x => x.Status, x => x.Type, x => x.Variation).FirstOrDefault(x => x.Id == id);
            if (prod == null)
            {
                this.FlashError("There is no product in the database with that id.");
                return RedirectToAction("index");
            }

            InitSelectLists();
            return View(prod);
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(Product product)
        {
            //if (ModelState.IsValid) {
            try
            {
                var prod = _productRepository.Find(product.Id);

                prod.LastModifiedBy = CurrentUser.UserName;
                prod.LastModifiedOn = DateTime.Now;

                prod.RetailPrice = product.RetailPrice;
                prod.Description = product.Description;
                prod.ItemNumber = product.ItemNumber;
                prod.ManufacturerColor = product.ManufacturerColor;
                prod.Usage = product.Usage;
                prod.MadeIn = product.MadeIn;
                prod.CartonQuantity = product.CartonQuantity;
                prod.CoefficientOfFrictionWet = product.CoefficientOfFrictionWet;
                prod.CoefficientOfFrictionDry = product.CoefficientOfFrictionDry;
                prod.BreakingStrength = product.BreakingStrength;
                prod.WaterAbsorption = product.WaterAbsorption;
                prod.IsChemicalResistant = product.IsChemicalResistant;
                prod.IsFrostResistant = product.IsFrostResistant;
                prod.ScratchHardiness = product.ScratchHardiness;
                prod.UnitsPerPiece = product.UnitsPerPiece;
                //prod.PriceCodeGroup = product.PriceCodeGroup;

                int mfgId = 0;
                if (Int32.TryParse(Request["SelectedManufacturer"], out mfgId))
                {
                    prod.Manufacturer = _ManufacturerRepo.Find(mfgId);
                    prod.Section = prod.Manufacturer != null ? prod.Manufacturer.Section : prod.Section;
                }

                prod.Series = product.Series;

                int usageId = 0;
                if (Int32.TryParse(Request["SelectedUsage"], out usageId))
                {
                    var usage = _ProductUsageRepo.Find(usageId);
                    if (usage != null)
                        prod.Usage = usage;
                }

                int catId = 0;
                if (Int32.TryParse(Request["SelectedCategory"], out catId))
                {
                    prod.Category = _productcategoryRepository.Find(catId);
                }

                int colorId = 0;
                if (Int32.TryParse(Request["SelectedColor"], out colorId))
                    prod.Color = _productcolorRepository.Find(colorId);

                int finishId = 0;
                if (Int32.TryParse(Request["SelectedFinish"], out finishId))
                    prod.Finish = _productfinishRepository.Find(finishId);

                int groupId = 0;
                if (Int32.TryParse(Request["SelectedGroup"], out groupId))
                    prod.Group = _productgroupRepository.Find(groupId);

                int sizeId = 0;
                if (Int32.TryParse(Request["SelectedSize"], out sizeId))
                    prod.Size = _productsizeRepository.Find(sizeId);

                int typeId = 0;
                if (Int32.TryParse(Request["SelectedType"], out typeId))
                    prod.Type = _producttypeRepository.Find(typeId);

                int varId = 0;
                if (Int32.TryParse(Request["SelectedVariation"], out varId))
                    prod.Variation = _VariationRepo.Find(varId);

                int uomId = 0;
                if (Int32.TryParse(Request["SelectedUoM"], out uomId))
                    prod.UnitOfMeasure = _UomRepo.Find(uomId);

                int statusId = 0;
                if (Int32.TryParse(Request["SelectedStatus"], out statusId))
                    prod.Status = _ProductStatusRepo.Find(statusId);

                prod.YouTubeUrl = product.YouTubeUrl;
                prod.SizeDescription = product.SizeDescription;
                _productRepository.InsertOrUpdate(prod);
                _productRepository.Save();

                this.FlashInfo("The product was successfully saved.");
                return RedirectToAction("Edit", new { id = product.Id });
            }
            catch (Exception ex)
            {
                this.FlashError("There were some problems updating the product.");    
            }

            InitSelectLists();
            return View("Edit", product);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var prod = _productRepository.Find(id);
                if (prod != null)
                {
                    var projects = _ProjectRepo.All.Where(x => x.Products.Any(y => y.Product.Id == prod.Id));
                    if (projects.Any())
                    {
                        foreach (var project in projects)
                        {
                            var items = project.Products.Where(x => x.Product.Id == prod.Id);
                            foreach (var projectItem in items)
                            {
                                projectItem.Comment +=
                                    string.Format(
                                        "<p>Item number {0} has been removed from our catalog and is no longer associated with your project. If you have any questions or concerns, please contact us.</p>",
                                        prod.ItemNumber);
                                projectItem.Product = null;
                                
                            }
                            _ProjectRepo.InsertOrUpdate(project);
                            
                        }
                        
                    }

                    var relProds = _productRepository.AllIncluding(x => x.RelatedSizes, x => x.RelatedAccents, x => x.RelatedTrims)
                        .Where(x => x.RelatedSizes.Any(y => y.Id == prod.Id) || x.RelatedTrims.Any(y => y.Id == prod.Id) || x.RelatedAccents.Any(y => y.Id == prod.Id)).ToList();
                    if (relProds != null && relProds.Count > 0)
                    {
                        foreach (var relProd in relProds)
                        {
                            //if (relProd.RelatedSizes.Any(x => x.Id == prod.Id))
                            relProd.RelatedSizes.Remove(prod);
                            relProd.RelatedAccents.Remove(prod);
                            relProd.RelatedTrims.Remove(prod);

                            _productRepository.InsertOrUpdate(relProd);
                            _productRepository.Save();
                        }

                    }

                    prod.RelatedAccents.Clear();
                    prod.RelatedTrims.Clear();
                    prod.RelatedProducts.Clear();
                    prod.RelatedSizes.Clear();

                }

                _productRepository.InsertOrUpdate(prod);
                _productRepository.Save();

                _productRepository.Delete(id);
                _productRepository.Save();

                _ProjectRepo.Save();

                this.FlashInfo("The product was successfully deleted.");
            }
            catch (Exception ex)
            {
                this.FlashError(ex.Message);
            }

            return RedirectToAction("Index");
        }
    }
}

