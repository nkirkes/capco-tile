using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAPCO.Areas.Admin.Controllers
{
    public class SliderController : Controller
    {
        private IRepository<SliderImage> _SliderImageRepo;
        public SliderController(IRepository<SliderImage> sliderImageRepo)
        {
            _SliderImageRepo = sliderImageRepo;
        }

        public ActionResult Index()
        {
            return View(_SliderImageRepo.All);
        }

        public ActionResult Show(int id)
        {
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        public ActionResult Create(SliderImage model)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult Update(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        public ActionResult Destroy(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
