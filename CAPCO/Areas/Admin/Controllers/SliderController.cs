using System.IO;
using System.Text.RegularExpressions;
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
        private readonly IRepository<SliderImage> _SliderImageRepo;
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

        public ActionResult New(SliderImage model)
        {
            
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(SliderImage model, HttpPostedFileBase image)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    if (image == null || image.ContentLength <= 0 || string.IsNullOrWhiteSpace(image.FileName))
                    {
                        throw new Exception("You must select a valid image.");
                    }
                    if (!Regex.IsMatch(Path.GetExtension(image.FileName), @"^.*\.(jpg|JPG|gif|GIF|png|PNG)$"))
                        throw new Exception("You must select a valid image.");

                    var fileName = Path.GetFileName(image.FileName) + "-" + DateTime.Now.Ticks;
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    image.SaveAs(path);
                    model.Path = path;

                    model.Order = _SliderImageRepo.All.Count() + 1;
                    _SliderImageRepo.InsertOrUpdate(model);
                    _SliderImageRepo.Save();

                    this.FlashInfo("The slider image was successfully saved.");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                this.FlashError(ex.Message);
            }
            return View("New", model);
            
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
