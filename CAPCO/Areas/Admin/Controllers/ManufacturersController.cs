using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class ManufacturersController : Controller
    {
		private readonly IRepository<Manufacturer> manufacturerRepository;
        private readonly IRepository<Product> _ProductRepo;
  

		public ManufacturersController(IRepository<Manufacturer> manufacturerRepository, IRepository<Product> productRepo)
        {
            _ProductRepo = productRepo;
            this.manufacturerRepository = manufacturerRepository;
        }

        public ViewResult Index()
        {
            return View(manufacturerRepository.AllIncluding(manufacturer => manufacturer.Products));
        }

        public ViewResult Show(int id)
        {
            return View(manufacturerRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(Manufacturer manufacturer)
        {
            if (ModelState.IsValid) {
                manufacturerRepository.InsertOrUpdate(manufacturer);
                manufacturerRepository.Save();

                this.FlashInfo("The manufacturer was successfully created.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the manufacturer.");
				return View("New", manufacturer);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(manufacturerRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, Manufacturer manufacturer)
        {
            if (ModelState.IsValid) {
                var mfgToUpdate = manufacturerRepository.Find(id);
                mfgToUpdate.Name = manufacturer.Name;

                if (mfgToUpdate.Section != manufacturer.Section)
                {
                    mfgToUpdate.Section = manufacturer.Section;
                    // clean up products
                    mfgToUpdate.Products.ToList().ForEach(x => 
                    {
                        x.Section = mfgToUpdate.Section;
                    });
                }

                manufacturerRepository.InsertOrUpdate(mfgToUpdate);
                manufacturerRepository.Save();

				this.FlashInfo("The manufacturer was successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the manufacturer.");
				return View("Edit", manufacturer);
			}
        }

        public ActionResult Delete(int id)
        {
            manufacturerRepository.Delete(id);
            manufacturerRepository.Save();

            this.FlashInfo("The manufacturer was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

