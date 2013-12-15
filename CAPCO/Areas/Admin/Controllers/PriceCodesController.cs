using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{
    public class PriceCodesController : BaseAdminController
    {
		private readonly IRepository<PriceCode> _PricecodeRepository;

		public PriceCodesController(IRepository<PriceCode> pricecodeRepository)
        {
			_PricecodeRepository = pricecodeRepository;
        }

        public ViewResult Index()
        {
            return View(_PricecodeRepository.All);//AllIncluding(x => x.Accounts, x => x.ProductPriceCodes));
        }

        public ViewResult Show(int id)
        {
            return View(_PricecodeRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(PriceCode pricecode)
        {
            if (ModelState.IsValid) {
                _PricecodeRepository.InsertOrUpdate(pricecode);
                _PricecodeRepository.Save();

                this.FlashInfo("The price code was created successfully.");
                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem creating the price code.");
            return View("New", pricecode);
        }
        
        public ActionResult Edit(int id)
        {
             return View(_PricecodeRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult Update(PriceCode pricecode)
        {
            if (ModelState.IsValid) {
                _PricecodeRepository.InsertOrUpdate(pricecode);
                _PricecodeRepository.Save();

                this.FlashInfo("The price code was saved successfully.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem removing the price code.");
            return View("Edit", pricecode);
        }

        public ActionResult Delete(int id)
        {
            _PricecodeRepository.Delete(id);
            _PricecodeRepository.Save();

            this.FlashInfo("The price code was deleted successfully.");

            return RedirectToAction("Index");
        }
    }
}

