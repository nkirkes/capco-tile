using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class ProductUsagesController : Controller
    {
		private readonly IRepository<ProductUsage> productusageRepository;

		public ProductUsagesController(IRepository<ProductUsage> productusageRepository)
        {
			this.productusageRepository = productusageRepository;
        }

        public ViewResult Index()
        {
            return View(productusageRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productusageRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductUsage productusage)
        {
            if (ModelState.IsValid) {
                productusageRepository.InsertOrUpdate(productusage);
                productusageRepository.Save();

				this.FlashInfo("The product usage was successfully created.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the product usage.");
				return View("New", productusage);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productusageRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductUsage productusage)
        {
            if (ModelState.IsValid) {
                productusageRepository.InsertOrUpdate(productusage);
                productusageRepository.Save();

                this.FlashInfo("The product usage was successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the product usage.");
				return View("Edit", productusage);
			}
        }

        public ActionResult Delete(int id)
        {
            productusageRepository.Delete(id);
            productusageRepository.Save();

            this.FlashInfo("The product usage was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

