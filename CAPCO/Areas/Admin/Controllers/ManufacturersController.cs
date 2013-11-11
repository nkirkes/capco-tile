using System.Linq;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class ManufacturersController : Controller
    {
		private readonly IRepository<Manufacturer> _ManufacturerRepository;
        
		public ManufacturersController(IRepository<Manufacturer> manufacturerRepository)
        {
            _ManufacturerRepository = manufacturerRepository;
        }

        public ViewResult Index()
        {
            return View(_ManufacturerRepository.AllIncluding(manufacturer => manufacturer.Products));
        }

        public ViewResult Show(int id)
        {
            return View(_ManufacturerRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(Manufacturer manufacturer)
        {
            if (ModelState.IsValid) {
                _ManufacturerRepository.InsertOrUpdate(manufacturer);
                _ManufacturerRepository.Save();

                this.FlashInfo("The manufacturer was successfully created.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem creating the manufacturer.");
            return View("New", manufacturer);
        }
        
        public ActionResult Edit(int id)
        {
             return View(_ManufacturerRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, Manufacturer manufacturer)
        {
            if (ModelState.IsValid) {
                var mfgToUpdate = _ManufacturerRepository.Find(id);
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

                _ManufacturerRepository.InsertOrUpdate(mfgToUpdate);
                _ManufacturerRepository.Save();

				this.FlashInfo("The manufacturer was successfully saved.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem saving the manufacturer.");
            return View("Edit", manufacturer);
        }

        public ActionResult Delete(int id)
        {
            _ManufacturerRepository.Delete(id);
            _ManufacturerRepository.Save();

            this.FlashInfo("The manufacturer was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

