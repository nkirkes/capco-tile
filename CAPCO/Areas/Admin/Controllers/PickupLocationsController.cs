using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class PickupLocationsController : BaseAdminController
    {
		private readonly IPickupLocationRepository pickuplocationRepository;

		public PickupLocationsController(IPickupLocationRepository pickuplocationRepository)
        {
			this.pickuplocationRepository = pickuplocationRepository;
        }

        public ViewResult Index()
        {
            return View(pickuplocationRepository.AllIncluding(x => x.Users));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(PickupLocation pickuplocation)
        {
            if (ModelState.IsValid) {
                pickuplocationRepository.InsertOrUpdate(pickuplocation);
                pickuplocationRepository.Save();

				this.FlashInfo("The location was successfully created.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the location.");
				return View("New", pickuplocation);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(pickuplocationRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, PickupLocation pickuplocation)
        {
            if (ModelState.IsValid) {
                var loc = pickuplocationRepository.Find(id);

                loc.Name = pickuplocation.Name;
                loc.Code = pickuplocation.Code;

                pickuplocationRepository.InsertOrUpdate(loc);
                pickuplocationRepository.Save();

				this.FlashInfo("The location was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the location.");
				return View("Edit", pickuplocation);
			}
        }

        public ActionResult Delete(int id)
        {
            pickuplocationRepository.Delete(id);
            pickuplocationRepository.Save();

			this.FlashInfo("The location was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

