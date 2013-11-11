using System.Linq;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductCategoriesController : BaseAdminController
    {
		private readonly IRepository<ProductCategory> _ProductcategoryRepository;

		public ProductCategoriesController(IRepository<ProductCategory> productcategoryRepository)
        {
			_ProductcategoryRepository = productcategoryRepository;
        }

        public ViewResult Index()
        {
            return View(_ProductcategoryRepository.All.ToList());
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductCategory productcategory)
        {
            if (ModelState.IsValid) {
                _ProductcategoryRepository.InsertOrUpdate(productcategory);
                _ProductcategoryRepository.Save();

                this.FlashInfo("The product category was successfully created.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem creating the product category.");
            return View("New", productcategory);
        }
        
        public ActionResult Edit(int id)
        {
             return View(_ProductcategoryRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductCategory productcategory)
        {
            if (ModelState.IsValid) {
                var prop = _ProductcategoryRepository.Find(productcategory.Id);
                prop.Name = productcategory.Name;
                prop.Code = productcategory.Code;

                _ProductcategoryRepository.InsertOrUpdate(prop);
                _ProductcategoryRepository.Save();

				this.FlashInfo("The product category was successfully saved.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem saving the product category.");
            return View("Edit", productcategory);
        }

        public ActionResult Delete(int id)
        {
            _ProductcategoryRepository.Delete(id);
            _ProductcategoryRepository.Save();

            this.FlashInfo("The product category was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

