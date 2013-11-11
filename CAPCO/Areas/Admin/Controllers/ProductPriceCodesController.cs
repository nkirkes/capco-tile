using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;
using CAPCO.Areas.Admin.Models;
using PagedList;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class ProductPriceCodesController : Controller
    {
		private readonly IRepository<ProductPriceCode> productpricecodeRepository;

		public ProductPriceCodesController(IRepository<ProductPriceCode> productpricecodeRepository)
        {
			this.productpricecodeRepository = productpricecodeRepository;
        }

        public ViewResult Index(PagedViewModel<ProductPriceCode> model)
        {
            var results = productpricecodeRepository.All.OrderBy(x => x.PriceGroup).ThenBy(x => x.PriceCode);
            model.TotalCount = results.Count();
            model.Entities = results.ToPagedList(model.Page ?? 1, 100);
            return View(model);
        }

        public ActionResult Search(PagedViewModel<ProductPriceCode> model)
        {
            var results = productpricecodeRepository.All.Where(x => x.PriceGroup.Contains(model.Criteria)).OrderBy(x => x.PriceGroup).ThenBy(x => x.PriceCode);
            model.TotalCount = results.Count();
            model.Entities = results.ToPagedList(model.Page ?? 1, 100);
            return View("Index", model);
        }

        public ViewResult Show(int id)
        {
            return View(productpricecodeRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductPriceCode productpricecode)
        {
            if (ModelState.IsValid) {
                productpricecodeRepository.InsertOrUpdate(productpricecode);
                productpricecodeRepository.Save();

				this.FlashInfo("The price code was successfully created.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem creating the price code.");
				return View("New", productpricecode);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productpricecodeRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, ProductPriceCode productpricecode)
        {
            if (ModelState.IsValid) {

                var pc = productpricecodeRepository.Find(id);
                pc.PriceCode = productpricecode.PriceCode;
                pc.PriceGroup = productpricecode.PriceGroup;
                pc.Price = productpricecode.Price;

                productpricecodeRepository.InsertOrUpdate(pc);
                productpricecodeRepository.Save();

                this.FlashInfo("The price code was successfully saved.");

                return RedirectToAction("Index");
            } else {
                this.FlashError("There was a problem saving the price code.");
				return View("Edit", productpricecode);
			}
        }

        public ActionResult Delete(int id)
        {
            productpricecodeRepository.Delete(id);
            productpricecodeRepository.Save();

            this.FlashInfo("The price code was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

