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
    public class ProductSeriesController : Controller
    {
		private readonly IRepository<ProductSeries> productseriesRepository;

		public ProductSeriesController(IRepository<ProductSeries> productseriesRepository)
        {
			this.productseriesRepository = productseriesRepository;
        }

        public ViewResult Index(PagedViewModel<ProductSeries> model)
        {
            var results = productseriesRepository.All.OrderBy(x => x.Name);
            model.TotalCount = results.Count();
            model.Entities = results.ToPagedList(model.Page ?? 1, 100);
            return View("Index", model);
        }

        public ActionResult Search(PagedViewModel<ProductSeries> model)
        {
            var results = productseriesRepository.All.Where(x => x.Name.Contains(model.Criteria)).OrderBy(x => x.Name);
            model.TotalCount = results.Count();
            model.Entities = results.ToPagedList(model.Page ?? 1, 100);
            return View("Index", model);
        }

        public ViewResult Show(int id)
        {
            return View(productseriesRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductSeries productseries)
        {
            if (ModelState.IsValid) {
                productseriesRepository.InsertOrUpdate(productseries);
                productseriesRepository.Save();

				this.FlashInfo("The series was successfully created.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the series.");
				return View("New", productseries);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(productseriesRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, ProductSeries productseries)
        {
            if (ModelState.IsValid) {
                var seriesToUpdate = productseriesRepository.Find(id);
                seriesToUpdate.Name = productseries.Name;
                seriesToUpdate.Code = productseries.Code;

                productseriesRepository.InsertOrUpdate(seriesToUpdate);
                productseriesRepository.Save();

				this.FlashInfo("The series was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the series.");
				return View("Edit", productseries);
			}
        }

        public ActionResult Delete(int id)
        {
            productseriesRepository.Delete(id);
            productseriesRepository.Save();

			this.FlashInfo("The series was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

