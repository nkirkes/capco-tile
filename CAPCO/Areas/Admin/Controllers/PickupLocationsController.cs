using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class PickupLocationsController : BaseAdminController
    {
		private readonly IRepository<PickupLocation> _PickuplocationRepository;

		public PickupLocationsController(IRepository<PickupLocation> pickuplocationRepository)
        {
			_PickuplocationRepository = pickuplocationRepository;
        }

        public ViewResult Index()
        {
            return View(_PickuplocationRepository.AllIncluding(x => x.Users));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(PickupLocation pickuplocation)
        {
            if (ModelState.IsValid) {
                _PickuplocationRepository.InsertOrUpdate(pickuplocation);
                _PickuplocationRepository.Save();

				this.FlashInfo("The location was successfully created.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem creating the location.");
            return View("New", pickuplocation);
        }
        
        public ActionResult Edit(int id)
        {
             return View(_PickuplocationRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, PickupLocation pickuplocation)
        {
            if (ModelState.IsValid) {
                var loc = _PickuplocationRepository.Find(id);

                loc.Name = pickuplocation.Name;
                loc.Code = pickuplocation.Code;

                _PickuplocationRepository.InsertOrUpdate(loc);
                _PickuplocationRepository.Save();

				this.FlashInfo("The location was successfully saved.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem saving the location.");
            return View("Edit", pickuplocation);
        }

        public ActionResult Delete(int id)
        {
            _PickuplocationRepository.Delete(id);
            _PickuplocationRepository.Save();

			this.FlashInfo("The location was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

