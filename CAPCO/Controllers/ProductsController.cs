using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Models;
using CAPCO.Infrastructure.Data;
using System.Drawing;
using CAPCO.Infrastructure.Specifications;
using CAPCO.Infrastructure.Services;
using PagedList;

namespace CAPCO.Controllers
{
    
    public class ProductsController : ApplicationController
    {
        private readonly IContentService _ContentService;
        private readonly IProductGroupRepository productgroupRepository;
        private readonly IProductCategoryRepository productcategoryRepository;
        private readonly IProductTypeRepository producttypeRepository;
        private readonly IProductColorRepository productcolorRepository;
        private readonly IProductSizeRepository productsizeRepository;
        private readonly IProductFinishRepository productfinishRepository;
        private readonly IProductRepository productRepository;
        private readonly IProjectRepository _ProjectRepository;
        
        public ProductsController(IProductGroupRepository productgroupRepository, 
            IProductCategoryRepository productcategoryRepository, 
            IProductTypeRepository producttypeRepository, 
            IProductColorRepository productcolorRepository, 
            IProductSizeRepository productsizeRepository, 
            IProductFinishRepository productfinishRepository, 
            IProductRepository productRepository,
            IProjectRepository projectRepository,
            IContentService contentService)
        {
            _ContentService = contentService;
            _ProjectRepository = projectRepository;
            this.productgroupRepository = productgroupRepository;
            this.productcategoryRepository = productcategoryRepository;
            this.producttypeRepository = producttypeRepository;
            this.productcolorRepository = productcolorRepository;
            this.productsizeRepository = productsizeRepository;
            this.productfinishRepository = productfinishRepository;
            this.productRepository = productRepository;
        }

        public ActionResult Index(
            ProductsViewModel model,
            int? groupId = null,
            int? typeId = null,
            int? sizeId = null,
            int? colorId = null,
            int? finishId = null,
            int? categoryId = null,
            string series = "",
            string itemNumber = "",
            string description = "")
        {
            
            if (model == null)
                model = new ProductsViewModel();

            model.Filters = new ProductFilters();
            model.Filters.GroupId = groupId;
            model.Filters.SizeId = sizeId;
            model.Filters.ColorId = colorId;
            model.Filters.CategoryId = categoryId;
            model.Filters.FinishId = finishId;
            model.Filters.TypeId = typeId;
            model.Filters.Series = series.ToLower();
            model.Filters.ItemNumber = itemNumber.ToLower();
            model.Filters.Description = description.ToLower();

            bool countGroups = false;
            bool countCategories = false;
            bool countFinishes = false;
            bool countTypes = false;
            bool countColors = false;
            bool countSizes = false;

            var specs = new List<Specification<Product>>();
            if (model.Filters.GroupId.HasValue)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByGroupSpecification(model.Filters.GroupId.Value));
                ProductGroup group = productgroupRepository.Find(model.Filters.GroupId.Value);
                model.Filters.GroupName = group != null ? group.Name : "Other Group";
                countGroups = true;
            }

            if (model.Filters.CategoryId.HasValue)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByCategorySpecification(model.Filters.CategoryId.Value));
                ProductCategory category = productcategoryRepository.Find(model.Filters.CategoryId.Value);
                model.Filters.CategoryName = category != null ? category.Name : "Other Category";
                countCategories = true;
            }

            if (model.Filters.FinishId.HasValue)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByFinishSpecification(model.Filters.FinishId.Value));
                ProductFinish finish = productfinishRepository.Find(model.Filters.FinishId.Value);
                model.Filters.FinishName = finish != null ? finish.Name : "Other Finish";
                countFinishes = true;
            }

            if (model.Filters.TypeId.HasValue)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByTypeSpecification(model.Filters.TypeId.Value));
                ProductType type = producttypeRepository.Find(model.Filters.TypeId.Value);
                model.Filters.TypeName = type != null ? type.Name : "Other Type";
                countTypes = true;
            }

            if (model.Filters.ColorId.HasValue)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByColorSpecification(model.Filters.ColorId.Value));
                ProductColor color = productcolorRepository.Find(model.Filters.ColorId.Value);
                model.Filters.ColorName = color != null ? color.Name : "Other Color";
                countColors = true;
            }

            if (model.Filters.SizeId.HasValue)
            {
                specs.Add(new Infrastructure.Specifications.ProductsBySizeSpecification(model.Filters.SizeId.Value));
                ProductSize size = productsizeRepository.Find(model.Filters.SizeId.Value);
                model.Filters.SizeName = size != null ? size.Name : "Other Size";
                countSizes = true;
            }

            if (!String.IsNullOrWhiteSpace(model.Filters.Series))
            {
                specs.Add(new ProductsBySeriesSpecification(model.Filters.Series));
            }

            if (!String.IsNullOrWhiteSpace(model.Filters.ItemNumber))
            {
                model.Filters.ItemNumber = model.Filters.ItemNumber.Replace(" ", "");
                specs.Add(new ProductsByItemNumberSpecification(model.Filters.ItemNumber));
            }

            if (!String.IsNullOrWhiteSpace(model.Filters.Description))
            {
                specs.Add(new ProductsByDescriptionSpecification(model.Filters.Description));
            }

            //IQueryable<Product> productsQuery;
            //if (specs.Any())
            //{
               var productsQuery = productRepository.FindBySpecification(specs.ToArray());
            //}
            //else
            //{
            //    productsQuery = productRepository.All;
            //}

            model.ProductsInGroups = countGroups ? productsQuery.Count(x => x.Group != null && x.Group.Id == model.Filters.GroupId.Value) : productsQuery.Count(x => x.Group != null);
            model.ProductsInCategories = countCategories ? productsQuery.Count(x => x.Category != null && x.Category.Id == model.Filters.CategoryId.Value) : 0;
            model.ProductsInColors = countColors ? productsQuery.Count(x => x.Color != null && x.Color.Id == model.Filters.ColorId.Value) : 0;
            model.ProductsInSizes = countSizes ? productsQuery.Count(x => x.Size != null && x.Size.Id == model.Filters.SizeId.Value) : 0;
            model.ProductsInTypes = countTypes ? productsQuery.Count(x => x.Type != null && x.Type.Id == model.Filters.TypeId.Value) : 0;
            model.ProductsInFinishes = countFinishes ? productsQuery.Count(x => x.Finish != null && x.Finish.Id == model.Filters.FinishId.Value) : 0;

            model.TotalCount = productsQuery.Count();
            model.Products = productsQuery.OrderBy(x => x.ItemNumber).ToPagedList(model.Page ?? 1, 24);
            model.AllProducts = productsQuery.ToList();

            if (model.Products.Count() > 0)
            {
                var plural = model.Products.Count() > 1 ? "products" : "product";
                ViewBag.ProductsMessage = String.Format("Showing {0} - {1} " + plural + " of {2} matching " + plural + ".", model.Page.HasValue && model.Page > 1 ? (model.Page - 1) * 24 + 1 : 1, (model.Page ?? 1) * model.Products.Count, model.TotalCount);
                model.AvailableGroups = (from p in productsQuery where p.Group != null select p.Group).Distinct().ToList();
                model.AvailableCategories = (from p in productsQuery where p.Category != null select p.Category).Distinct().ToList();
                model.AvailableTypes = (from p in productsQuery where p.Type != null select p.Type).Distinct().ToList();
                model.AvailableColors = (from p in productsQuery where p.Color != null select p.Color).Distinct().ToList();
                model.AvailableSizes = (from p in productsQuery where p.Size != null select p.Size).Distinct().ToList();
                model.AvailableFinishes = (from p in productsQuery where p.Finish != null select p.Finish).Distinct().ToList();
            }
            else
            {
                ViewBag.ProductsMessage = "There are no products to display";
                model.AvailableCategories = productcategoryRepository.All.ToList();
                model.AvailableGroups = productgroupRepository.All.ToList();
                model.AvailableTypes = producttypeRepository.All.ToList();
                model.AvailableColors = productcolorRepository.All.ToList();
                model.AvailableSizes = productsizeRepository.All.ToList();
                model.AvailableFinishes = productfinishRepository.All.ToList();
            }


            return View(model);
        }

        public ActionResult Slabs()
        {
            ViewBag.SlabContent = _ContentService.GetContentSection(ContentSectionNames.Slabs.ToString());
            return View();
        }

        public ActionResult Show(int id)
        {
            var product = productRepository.AllIncluding(x => x.Manufacturer).FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                this.FlashError("Sorry, but I couldn't find that product. Please try expanding your search.");
                return RedirectToAction("Search");
            }

            var relatedProducts = productRepository.FindBySpecification(new ProductsBySeriesSpecification(product.Series)).ToList();
            if (relatedProducts != null && relatedProducts.Any())
            { 
                if (relatedProducts.Contains(product))
                    relatedProducts.Remove(product);
                ViewBag.RelatedProducts = relatedProducts.Take(4).ToList();
            }

            return View(productRepository.Find(id));
        }

        public ActionResult Search()
        {
            ProductSearchCriteria model = new ProductSearchCriteria();
            model.Filters = new ProductFilters();

            // fill the collection then pare down the available options
            model.Products = productRepository.AllIncluding(x => x.Group, x => x.Category, x => x.Color, x => x.Finish, x => x.Type, x => x.Size).ToList();
            model.AvailableGroups = (from p in model.Products select p.Group).Distinct().ToList();
            model.AvailableCategories = (from p in model.Products select p.Category).Distinct().ToList();
            model.AvailableTypes = (from p in model.Products select p.Type).Distinct().ToList();
            model.AvailableColors = (from p in model.Products select p.Color).Distinct().ToList();
            model.AvailableSizes = (from p in model.Products select p.Size).Distinct().ToList();
            model.AvailableFinishes = (from p in model.Products select p.Finish).Distinct().ToList();

            // empty the collection
            model.Products = new List<Product>();

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(ProductSearchCriteria model, int page = 1, int pageSize = 25)
        {
            model.Products = new List<Product>();

            model.Filters = new ProductFilters();
            model.Filters.GroupId = model.SelectedProductGroup;
            model.Filters.SizeId = model.SelectedSize;
            model.Filters.ColorId = model.SelectedColor;
            model.Filters.CategoryId = model.SelectedCategory;
            model.Filters.FinishId = model.SelectedFinish;
            model.Filters.TypeId = model.SelectedType;
            model.Filters.Series = !String.IsNullOrWhiteSpace(model.Series) ? model.Series.ToLower() : "";
            model.Filters.ItemNumber = !String.IsNullOrWhiteSpace(model.ItemNumber) ? model.ItemNumber.Replace(" ","").ToLower() : "";

            var specs = new List<Specification<Product>>();
            if (model.Filters.GroupId.HasValue && model.Filters.GroupId.Value > 0)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByGroupSpecification(model.Filters.GroupId.Value));
            }

            if (model.Filters.CategoryId.HasValue && model.Filters.CategoryId.Value > 0)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByCategorySpecification(model.Filters.CategoryId.Value));
            }

            if (model.Filters.FinishId.HasValue && model.Filters.FinishId.Value > 0)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByFinishSpecification(model.Filters.FinishId.Value));
            }

            if (model.Filters.TypeId.HasValue && model.Filters.TypeId.Value > 0)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByTypeSpecification(model.Filters.TypeId.Value));
            }

            if (model.Filters.ColorId.HasValue && model.Filters.ColorId.Value > 0)
            {
                specs.Add(new Infrastructure.Specifications.ProductsByColorSpecification(model.Filters.ColorId.Value));
            }

            if (model.Filters.SizeId.HasValue && model.Filters.SizeId.Value > 0)
            {
                specs.Add(new Infrastructure.Specifications.ProductsBySizeSpecification(model.Filters.SizeId.Value));
            }

            if (!String.IsNullOrWhiteSpace(model.Filters.Series))
            {
                specs.Add(new ProductsBySeriesSpecification(model.Filters.Series));
            }

            if (!String.IsNullOrWhiteSpace(model.Filters.ItemNumber))
            {
                specs.Add(new ProductsByItemNumberSpecification(model.Filters.ItemNumber));
            }

            var productsQuery = productRepository.FindBySpecification(specs.ToArray());
            model.Products = productsQuery.ToPagedList(page, pageSize);

            if (model.Products.Count() > 0)
            {
                var plural = model.Products.Count() > 1 ? "products" : "product";
                ViewBag.ProductsMessage = String.Format("Showing {0} " + plural + ".", model.Products.Count());
                model.AvailableGroups = (from p in model.Products select p.Group).Distinct().ToList();
                model.AvailableCategories = (from p in model.Products select p.Category).Distinct().ToList();
                model.AvailableTypes = (from p in model.Products select p.Type).Distinct().ToList();
                model.AvailableColors = (from p in model.Products select p.Color).Distinct().ToList();
                model.AvailableSizes = (from p in model.Products select p.Size).Distinct().ToList();
                model.AvailableFinishes = (from p in model.Products select p.Finish).Distinct().ToList();
            }
            else
            {
                ViewBag.ProductsMessage = String.Format("There are no products to display", model.Products.Count());
                model.AvailableCategories = productcategoryRepository.All.ToList();
                model.AvailableGroups = productgroupRepository.All.ToList();
                model.AvailableTypes = producttypeRepository.All.ToList();
                model.AvailableColors = productcolorRepository.All.ToList();
                model.AvailableSizes = productsizeRepository.All.ToList();
                model.AvailableFinishes = productfinishRepository.All.ToList();
            }

            return View(model);
        }

        [ValidateAntiForgeryToken()]
        public ActionResult AddToProject(int id, int projectId)
        {
            try
            {
                var product = productRepository.Find(id);
                var project = _ProjectRepository.Find(projectId);
                var item = new ProjectItem { Product = product };
                item.Comment = Request.Form["ItemComment"] != null && !String.IsNullOrEmpty(Request.Form["ItemComment"]) ? Request.Form["ItemComment"] : "";
                
                project.AddProduct(item);
                _ProjectRepository.InsertOrUpdate(project);
                _ProjectRepository.Save();
                var path = @Url.Action("show", "projects", new { id = projectId });
                this.FlashInfo(String.Format("The product was added to the <em><a href=\"{0}\">{1}</a></em> project successfully.", path, project.ProjectName));
                return RedirectToAction("Show", "products", new { id = id });
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem adding the product to the selected project: " + ex.Message);
            }
            return RedirectToAction("Show", "products", new { id = id });
        }

        //private ProductSearchCriteria InitSearchModel()
        //{
        //    ProductSearchCriteria model = new ProductSearchCriteria();
        //    return InitSearchModel(model);
        //}

        //private ProductSearchCriteria InitSearchModel(ProductSearchCriteria model)
        //{
        //    model.PossibleGroups = productgroupRepository.All.ToList();
        //    model.PossibleCategories = productcategoryRepository.All.ToList();
        //    model.PossibleTypes = producttypeRepository.All.ToList();
        //    model.PossibleSizes = productsizeRepository.All.ToList();
        //    model.PossibleColors = productcolorRepository.All.ToList();
        //    model.PossibleFinishes = productfinishRepository.All.ToList();
        //    return model;
        //}
    }
}

