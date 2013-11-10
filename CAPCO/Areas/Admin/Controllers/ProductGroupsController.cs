using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ProductGroupsController : BaseAdminController
    {
		private readonly IProductGroupRepository productgroupRepository;

		public ProductGroupsController(IProductGroupRepository productgroupRepository)
        {
			this.productgroupRepository = productgroupRepository;
        }

        public ViewResult Index()
        {
            return View(productgroupRepository.All.ToList());
        }

        public ViewResult Show(int id)
        {
            return View(productgroupRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductGroup productgroup)
        {
            if (ModelState.IsValid) {
                productgroupRepository.InsertOrUpdate(productgroup);
                productgroupRepository.Save();

				this.FlashInfo("The Product Group was successfully deleted.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the ProductGroup.");
				return View("New", productgroup);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productgroupRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ProductGroup productgroup)
        {
            if (ModelState.IsValid) {

                var group = productgroupRepository.Find(productgroup.Id);
                group.Name = productgroup.Name;
                group.Code = productgroup.Code;

                productgroupRepository.InsertOrUpdate(group);
                productgroupRepository.Save();

				this.FlashInfo("The ProductGroup was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the ProductGroup.");
				return View("Edit", productgroup);
			}
        }

        public ActionResult Delete(int id)
        {
            productgroupRepository.Delete(id);
            productgroupRepository.Save();

			this.FlashInfo("The ProductGroup was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

