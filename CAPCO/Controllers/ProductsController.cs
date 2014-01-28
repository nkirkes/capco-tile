using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Models;
using CAPCO.Infrastructure.Data;
using System.Drawing;
using CAPCO.Infrastructure.Specifications;
using CAPCO.Infrastructure.Services;
using Elmah.ContentSyndication;
using PagedList;

namespace CAPCO.Controllers
{
    public class ProductsController : ApplicationController
    {
        private readonly IRepository<ProductGroup> productgroupRepository;
        private readonly IRepository<ProductCategory> productcategoryRepository;
        private readonly IRepository<ProductType> producttypeRepository;
        private readonly IRepository<ProductColor> productcolorRepository;
        private readonly IRepository<ProductSize> productsizeRepository;
        private readonly IRepository<ProductFinish> productfinishRepository;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<Project> _ProjectRepository;
        private readonly IContentService _ContentService;
        
        public ProductsController(IRepository<ProductGroup> productgroupRepository, 
            IRepository<ProductCategory> productcategoryRepository, 
            IRepository<ProductType> producttypeRepository, 
            IRepository<ProductColor> productcolorRepository, 
            IRepository<ProductSize> productsizeRepository, 
            IRepository<ProductFinish> productfinishRepository, 
            IRepository<Product> productRepository,
            IRepository<Project> projectRepository,
            IContentService contentService)
        {
            _ProjectRepository = projectRepository;
            this.productgroupRepository = productgroupRepository;
            this.productcategoryRepository = productcategoryRepository;
            this.producttypeRepository = producttypeRepository;
            this.productcolorRepository = productcolorRepository;
            this.productsizeRepository = productsizeRepository;
            this.productfinishRepository = productfinishRepository;
            this.productRepository = productRepository;
            _ContentService = contentService;
        }

        public PagedProductsList GetPagedProducts(int take, int page)
        {
            var query = productRepository.All.OrderBy(x => x.ItemNumber);
            var count = query.Count();
            var products = query.Skip(page*take).Take(take).ToList();
            
            return new PagedProductsList
            {
                Products = products,//.ToPagedList(page<1?1:page, take),
                HasNext = (page < count),
                HasPrevious = (page*take > 0),
                Count = count
            };
        }

        public ActionResult Index(int page = 1, 
            string group = "",
            string type = "",
            string size = "",
            string color = "",
            string finish = "",
            string category = "",
            string series = "",
            string itemNumber = "",
            string description = "")
        {
            int pageSize = 24;
            var query = CreateProductsQuery(group, type, size, color, finish, category, series, itemNumber, description);
            
            // filter criteria
            var filters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(group))
                filters.Add("Group", group);
            if (!string.IsNullOrWhiteSpace(type))
                filters.Add("Type", type);
            if (!string.IsNullOrWhiteSpace(size))
                filters.Add("Size", size);
            if (!string.IsNullOrWhiteSpace(color))
                filters.Add("Color", color);
            if (!string.IsNullOrWhiteSpace(finish))
                filters.Add("Finish", finish);
            if (!string.IsNullOrWhiteSpace(category))
                filters.Add("Category", category);
            if (!string.IsNullOrWhiteSpace(series))
                filters.Add("Series", series);
            if (!string.IsNullOrWhiteSpace(itemNumber))
                filters.Add("Item Number", itemNumber);
            if (!string.IsNullOrWhiteSpace(description))
                filters.Add("Description", description);
            ViewBag.Filters = filters;

            // filter options
            ViewBag.Groups = (from p in query where p.Group != null select p.Group).GroupBy(x => x.Name).ToList();
            ViewBag.Categories = (from p in query where p.Category != null select p.Category).GroupBy(x => x.Name).ToList();
            ViewBag.Types = (from p in query where p.Type != null select p.Type).GroupBy(x => x.Name).ToList();
            ViewBag.Sizes = (from p in query where p.Size != null select p.Size).GroupBy(x => x.Name).ToList();
            ViewBag.Colors = (from p in query where p.Color != null select p.Color).GroupBy(x => x.Name).ToList();
            ViewBag.Finishes = (from p in query where p.Finish != null select p.Finish).GroupBy(x => x.Name).ToList();

            return View(query.ToPagedList(page, pageSize));
        }

        private IQueryable<Product> CreateProductsQuery(string group = "",
            string type = "",
            string size = "",
            string color = "",
            string finish = "",
            string category = "",
            string series = "",
            string itemNumber = "",
            string description = "")
        {
            var specs = new List<Specification<Product>>();
            
            if (!string.IsNullOrWhiteSpace(group))
            {
                specs.Add(new ProductsByGroupSpecification(HttpUtility.HtmlDecode(group)));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                specs.Add(new ProductsByCategorySpecification(HttpUtility.HtmlDecode(category)));
            }

            if (!string.IsNullOrWhiteSpace(finish))
            {
                specs.Add(new ProductsByFinishSpecification(finish));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                specs.Add(new ProductsByTypeSpecification(type));
            }

            if (!string.IsNullOrWhiteSpace(color))
            {
                specs.Add(new ProductsByColorSpecification(color));
            }

            if (!string.IsNullOrWhiteSpace(size))
            {
                specs.Add(new ProductsBySizeSpecification(size));
            }

            if (!String.IsNullOrWhiteSpace(series))
            {
                specs.Add(new ProductsBySeriesSpecification(series));
            }

            if (!String.IsNullOrWhiteSpace(itemNumber))
            {
                specs.Add(new ProductsByItemNumberSpecification(itemNumber.Replace(" ", "")));
            }

            if (!String.IsNullOrWhiteSpace(description))
            {
                specs.Add(new ProductsByDescriptionSpecification(description));
            }

            return specs.Any() ? productRepository.FindBySpecification(specs.ToArray()).OrderBy(x => x.ItemNumber) : productRepository.AllIncluding(x => x.ProductPriceCodes).OrderBy(x => x.ItemNumber);
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

