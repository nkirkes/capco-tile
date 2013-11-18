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
            return View(_SliderImageRepo.All.ToList());
        }

        public ActionResult New()
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

                    var fileName = Path.GetFileNameWithoutExtension(image.FileName) + "-" + DateTime.Now.Ticks + Path.GetExtension(image.FileName);
                    var virtualPath = "Public/Assets/uploads";
                    var serverPath = Server.MapPath("~/" + virtualPath);
                    image.SaveAs(Path.Combine(serverPath, fileName));
                    model.Path = virtualPath + "/" + fileName;

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
            var slider = _SliderImageRepo.Find(id);
            return View(slider);
        }

        public ActionResult Update(int id, SliderImage model)
        {
            if (ModelState.IsValid)
            {
                var toUpdate = _SliderImageRepo.Find(id);
                toUpdate.Label = model.Label;
                toUpdate.Caption = model.Caption;


                _SliderImageRepo.InsertOrUpdate(toUpdate);
                _SliderImageRepo.Save();

                this.FlashInfo("The slider image details were successfully saved.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem saving the slider image details.");
            return View("Edit", model);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var filePath = (from si in _SliderImageRepo.All where si.Id == id select si.Path).FirstOrDefault();
                var serverPath = Server.MapPath("~/" + filePath);
                if (!string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(serverPath))
                    System.IO.File.Delete(serverPath);

                _SliderImageRepo.Delete(id);
                _SliderImageRepo.Save();


                this.FlashInfo("The image was successfully deleted.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.FlashError("There was an error deleting the slider image: " + ex.Message);
            }
            return RedirectToAction("Edit", id);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Reorder(string itemIds)
        {
            try
            {
                var ids = itemIds.Replace("&", "").Split(new[] { "sliderItem[]=" }, StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                foreach (var id in ids)
                {
                    var slider = _SliderImageRepo.Find(Int32.Parse(id));
                    slider.Order = index++;
                    _SliderImageRepo.InsertOrUpdate(slider);
                }
                _SliderImageRepo.Save();

                this.FlashInfo("The images were reordered successfully.");

            }
            catch (Exception ex)
            {
                this.FlashError("There was an error updating the image order: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

    }
}
