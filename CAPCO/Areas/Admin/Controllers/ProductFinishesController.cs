using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductFinishesController : BaseAdminController
    {
		private readonly IProductFinishRepository productfinishRepository;

		public ProductFinishesController(IProductFinishRepository productfinishRepository)
        {
			this.productfinishRepository = productfinishRepository;
        }

        public ViewResult Index()
        {
            return View(productfinishRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productfinishRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductFinish productfinish)
        {
            if (ModelState.IsValid) {
                productfinishRepository.InsertOrUpdate(productfinish);
                productfinishRepository.Save();

                this.FlashInfo("The product finish was successfully deleted.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the product finish.");
				return View("New", productfinish);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productfinishRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductFinish productfinish)
        {
            if (ModelState.IsValid) {
                var finish = productfinishRepository.Find(productfinish.Id);
                finish.Name = productfinish.Name;
                finish.Code = productfinish.Code;

                productfinishRepository.InsertOrUpdate(finish);
                productfinishRepository.Save();

				this.FlashInfo("The was product finish successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the product finish.");
				return View("Edit", productfinish);
			}
        }

        public ActionResult Delete(int id)
        {
            productfinishRepository.Delete(id);
            productfinishRepository.Save();

            this.FlashInfo("The product finish was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

