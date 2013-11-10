using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductVariationsController : BaseAdminController
    {
		private readonly IProductVariationRepository productvariationRepository;

		public ProductVariationsController(IProductVariationRepository productvariationRepository)
        {
			this.productvariationRepository = productvariationRepository;
        }

        public ViewResult Index()
        {
            return View(productvariationRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productvariationRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductVariation productvariation)
        {
            if (ModelState.IsValid) {
                productvariationRepository.InsertOrUpdate(productvariation);
                productvariationRepository.Save();

				this.FlashInfo("The variation was successfully created.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the variation.");
				return View("New", productvariation);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productvariationRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductVariation productvariation)
        {
            if (ModelState.IsValid) {
                var prop = productvariationRepository.Find(productvariation.Id);
                prop.Name = productvariation.Name;
                prop.Code = productvariation.Code;

                productvariationRepository.InsertOrUpdate(prop);
                productvariationRepository.Save();

				this.FlashInfo("The variation was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the variation.");
				return View("Edit", productvariation);
			}
        }

        public ActionResult Delete(int id)
        {
            productvariationRepository.Delete(id);
            productvariationRepository.Save();

			this.FlashInfo("The variation was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

