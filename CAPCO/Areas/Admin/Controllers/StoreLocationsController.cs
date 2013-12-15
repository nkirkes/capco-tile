using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class StoreLocationsController : Controller
    {
		private readonly IRepository<StoreLocation> storelocationRepository;

		public StoreLocationsController(IRepository<StoreLocation> storelocationRepository)
        {
			this.storelocationRepository = storelocationRepository;
        }

        public ViewResult Index()
        {
            return View(storelocationRepository.All);
        }

        public ViewResult Show(int id)
        {
            return View(storelocationRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(StoreLocation storelocation)
        {
            if (ModelState.IsValid) {
                storelocationRepository.InsertOrUpdate(storelocation);
                storelocationRepository.Save();

				this.FlashInfo("The  was successfully deleted.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the .");
				return View("New", storelocation);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(storelocationRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(StoreLocation storelocation)
        {
            if (ModelState.IsValid) {
                storelocationRepository.InsertOrUpdate(storelocation);
                storelocationRepository.Save();

				this.FlashInfo("The was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the .");
				return View("Edit", storelocation);
			}
        }

        public ActionResult Delete(int id)
        {
            storelocationRepository.Delete(id);
            storelocationRepository.Save();

			this.FlashInfo("The  was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

