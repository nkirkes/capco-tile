using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class PriceCodesController : BaseAdminController
    {
		private readonly IPriceCodeRepository pricecodeRepository;

		public PriceCodesController(IPriceCodeRepository pricecodeRepository)
        {
			this.pricecodeRepository = pricecodeRepository;
        }

        public ViewResult Index()
        {
            return View(pricecodeRepository.All);//AllIncluding(x => x.Accounts, x => x.ProductPriceCodes));
        }

        public ViewResult Show(int id)
        {
            return View(pricecodeRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(PriceCode pricecode)
        {
            if (ModelState.IsValid) {
                pricecodeRepository.InsertOrUpdate(pricecode);
                pricecodeRepository.Save();

                this.FlashInfo("The price code was created successfully.");
                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the price code.");
				return View("New", pricecode);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(pricecodeRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult Update(PriceCode pricecode)
        {
            if (ModelState.IsValid) {
                pricecodeRepository.InsertOrUpdate(pricecode);
                pricecodeRepository.Save();

                this.FlashInfo("The price code was saved successfully.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem removing the price code.");
				return View("Edit", pricecode);
			}
        }

        public ActionResult Delete(int id)
        {
            pricecodeRepository.Delete(id);
            pricecodeRepository.Save();

            this.FlashInfo("The price code was deleted successfully.");

            return RedirectToAction("Index");
        }
    }
}

