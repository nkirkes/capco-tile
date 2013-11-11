using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductTypesController : BaseAdminController
    {
		private readonly IRepository<ProductType> producttypeRepository;

		public ProductTypesController(IRepository<ProductType> producttypeRepository)
        {
			this.producttypeRepository = producttypeRepository;
        }

        public ViewResult Index()
        {
            return View(producttypeRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(producttypeRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductType producttype)
        {
            if (ModelState.IsValid) {
                producttypeRepository.InsertOrUpdate(producttype);
                producttypeRepository.Save();

                this.FlashInfo("The product type was successfully created.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the product type.");
				return View("New", producttype);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(producttypeRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductType producttype)
        {
            if (ModelState.IsValid) {
                var prop = producttypeRepository.Find(producttype.Id);
                prop.Name = producttype.Name;
                prop.Code = producttype.Code;

                producttypeRepository.InsertOrUpdate(prop);
                producttypeRepository.Save();

				this.FlashInfo("The product type was successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the product type.");
				return View("Edit", producttype);
			}
        }

        public ActionResult Delete(int id)
        {
            producttypeRepository.Delete(id);
            producttypeRepository.Save();

            this.FlashInfo("The product type was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

