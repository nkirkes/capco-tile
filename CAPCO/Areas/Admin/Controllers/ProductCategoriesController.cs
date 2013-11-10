using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductCategoriesController : BaseAdminController
    {
		private readonly IProductCategoryRepository productcategoryRepository;

		public ProductCategoriesController(IProductCategoryRepository productcategoryRepository)
        {
			this.productcategoryRepository = productcategoryRepository;
        }

        public ViewResult Index()
        {
            return View(productcategoryRepository.All.ToList());
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductCategory productcategory)
        {
            if (ModelState.IsValid) {
                productcategoryRepository.InsertOrUpdate(productcategory);
                productcategoryRepository.Save();

                this.FlashInfo("The product category was successfully created.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the product category.");
				return View("New", productcategory);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productcategoryRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductCategory productcategory)
        {
            if (ModelState.IsValid) {
                var prop = productcategoryRepository.Find(productcategory.Id);
                prop.Name = productcategory.Name;
                prop.Code = productcategory.Code;

                productcategoryRepository.InsertOrUpdate(prop);
                productcategoryRepository.Save();

				this.FlashInfo("The product category was successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the product category.");
				return View("Edit", productcategory);
			}
        }

        public ActionResult Delete(int id)
        {
            productcategoryRepository.Delete(id);
            productcategoryRepository.Save();

            this.FlashInfo("The product category was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

