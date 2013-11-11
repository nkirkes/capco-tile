using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductColorsController : BaseAdminController
    {
		private readonly IRepository<ProductColor> productcolorRepository;

		public ProductColorsController(IRepository<ProductColor> productcolorRepository)
        {
			this.productcolorRepository = productcolorRepository;
        }

        public ViewResult Index()
        {
            return View(productcolorRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productcolorRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductColor productcolor)
        {
            if (ModelState.IsValid) {
                productcolorRepository.InsertOrUpdate(productcolor);
                productcolorRepository.Save();

				this.FlashInfo("The product color was successfully deleted.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the product color.");
				return View("New", productcolor);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productcolorRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductColor productcolor)
        {
            if (ModelState.IsValid) {
                var prop = productcolorRepository.Find(productcolor.Id);
                prop.Name = productcolor.Name;
                prop.Code = productcolor.Code;

                productcolorRepository.InsertOrUpdate(prop);
                productcolorRepository.Save();

                this.FlashInfo("The product color was successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the product color.");
				return View("Edit", productcolor);
			}
        }

        public ActionResult Delete(int id)
        {
            productcolorRepository.Delete(id);
            productcolorRepository.Save();

            this.FlashInfo("The product color was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

