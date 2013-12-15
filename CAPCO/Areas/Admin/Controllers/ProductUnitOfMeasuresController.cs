using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductUnitOfMeasuresController : BaseAdminController
    {
		private readonly IRepository<ProductUnitOfMeasure> productunitofmeasureRepository;

		public ProductUnitOfMeasuresController(IRepository<ProductUnitOfMeasure> productunitofmeasureRepository)
        {
			this.productunitofmeasureRepository = productunitofmeasureRepository;
        }

        public ViewResult Index()
        {
            return View(productunitofmeasureRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productunitofmeasureRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductUnitOfMeasure productunitofmeasure)
        {
            if (ModelState.IsValid) {
                productunitofmeasureRepository.InsertOrUpdate(productunitofmeasure);
                productunitofmeasureRepository.Save();

				this.FlashInfo("The unit of measure was successfully created.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the unit of measure.");
				return View("New", productunitofmeasure);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productunitofmeasureRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductUnitOfMeasure productunitofmeasure)
        {
            if (ModelState.IsValid) {
                var prop = productunitofmeasureRepository.Find(productunitofmeasure.Id);
                prop.Name = productunitofmeasure.Name;
                prop.Code = productunitofmeasure.Code;

                productunitofmeasureRepository.InsertOrUpdate(prop);
                productunitofmeasureRepository.Save();

				this.FlashInfo("The unit of measure was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the unit of measure.");
				return View("Edit", productunitofmeasure);
			}
        }

        public ActionResult Delete(int id)
        {
            productunitofmeasureRepository.Delete(id);
            productunitofmeasureRepository.Save();

			this.FlashInfo("The unit of measure was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

