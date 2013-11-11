using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductStatusController : BaseAdminController
    {
		private readonly IRepository<ProductStatus> productstatusRepository;

		public ProductStatusController(IRepository<ProductStatus> productstatusRepository)
        {
			this.productstatusRepository = productstatusRepository;
        }

        public ViewResult Index()
        {
            return View(productstatusRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productstatusRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductStatus productstatus)
        {
            if (ModelState.IsValid) {
                productstatusRepository.InsertOrUpdate(productstatus);
                productstatusRepository.Save();

				this.FlashInfo("The status was successfully created.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the status.");
				return View("New", productstatus);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productstatusRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductStatus productstatus)
        {
            if (ModelState.IsValid) {
                var prop = productstatusRepository.Find(productstatus.Id);
                prop.Name = productstatus.Name;
                prop.Code = productstatus.Code;

                productstatusRepository.InsertOrUpdate(prop);
                productstatusRepository.Save();

				this.FlashInfo("The status was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the status.");
				return View("Edit", productstatus);
			}
        }

        public ActionResult Delete(int id)
        {
            productstatusRepository.Delete(id);
            productstatusRepository.Save();

			this.FlashInfo("The status was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

