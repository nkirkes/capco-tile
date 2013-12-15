using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductSizesController : BaseAdminController
    {
		private readonly IRepository<ProductSize> productsizeRepository;

		public ProductSizesController(IRepository<ProductSize> productsizeRepository)
        {
			this.productsizeRepository = productsizeRepository;
        }

        public ViewResult Index()
        {
            return View(productsizeRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productsizeRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductSize productsize)
        {
            if (ModelState.IsValid) {
                productsizeRepository.InsertOrUpdate(productsize);
                productsizeRepository.Save();

                this.FlashInfo("The product size was successfully deleted.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the product size.");
				return View("New", productsize);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productsizeRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductSize productsize)
        {
            if (ModelState.IsValid) {
                var prop = productsizeRepository.Find(productsize.Id);
                prop.Name = productsize.Name;
                prop.Code = productsize.Code;

                productsizeRepository.InsertOrUpdate(prop);
                productsizeRepository.Save();

				this.FlashInfo("The product size was successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the product size.");
				return View("Edit", productsize);
			}
        }

        public ActionResult Delete(int id)
        {
            productsizeRepository.Delete(id);
            productsizeRepository.Save();

            this.FlashInfo("The product size was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

