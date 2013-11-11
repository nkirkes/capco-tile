using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class DiscountCodesController : BaseAdminController
    {
		private readonly IRepository<DiscountCode> discountcodeRepository;

		public DiscountCodesController(IRepository<DiscountCode> discountcodeRepository)
        {
			this.discountcodeRepository = discountcodeRepository;
        }

        public ViewResult Index()
        {
            return View(discountcodeRepository.All);
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(DiscountCode discountcode)
        {
            if (ModelState.IsValid) {
                discountcodeRepository.InsertOrUpdate(discountcode);
                discountcodeRepository.Save();

				this.FlashInfo("The discount code was successfully created.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the discount code.");
				return View("New", discountcode);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(discountcodeRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, DiscountCode discountcode)
        {
            if (ModelState.IsValid) {
                var toUpdate = discountcodeRepository.Find(id);
                if (toUpdate == null)
                {
                    this.FlashError("There is no discount code with that id.");
                    return RedirectToAction("index");
                }

                toUpdate.Code = discountcode.Code;
                toUpdate.Name = discountcode.Name;
                
                discountcodeRepository.InsertOrUpdate(toUpdate);
                discountcodeRepository.Save();

				this.FlashInfo("The was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the discount code.");
				return View("Edit", discountcode);
			}
        }

        public ActionResult Delete(int id)
        {
            discountcodeRepository.Delete(id);
            discountcodeRepository.Save();

			this.FlashInfo("The discount code was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

