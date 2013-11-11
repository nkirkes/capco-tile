using System;
using System.Linq;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class LinksController : Controller
    {
		private readonly IRepository<Link> _LinkRepository;

		public LinksController(IRepository<Link> linkRepository)
        {
			this._LinkRepository = linkRepository;
        }

        public ViewResult Index()
        {
            return View(_LinkRepository.All);
        }
        
        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(Link link)
        {
            if (ModelState.IsValid) {
                link.Order = _LinkRepository.All.Count() + 1;
                _LinkRepository.InsertOrUpdate(link);
                _LinkRepository.Save();

				this.FlashInfo("The link was successfully saved.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem creating the link.");
            return View("New", link);
        }
        
        public ActionResult Edit(int id)
        {
             return View(_LinkRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, Link link)
        {
            if (ModelState.IsValid)
            {
                var toUpdate = _LinkRepository.Find(id);
                toUpdate.Description = link.Description;
                toUpdate.Label = link.Label;
                toUpdate.Url = link.Url;

                
                _LinkRepository.InsertOrUpdate(toUpdate);
                _LinkRepository.Save();
                
                this.FlashInfo("The link was successfully saved.");

                return RedirectToAction("Index");
            }

            this.FlashError("There was a problem saving the link.");
            return View("Edit", link);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Reorder(string itemIds)
        {
            try
            {
                var ids = itemIds.Replace("&", "").Split(new[] { "linkItem[]=" }, StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                foreach (var id in ids)
                {
                    var link = _LinkRepository.Find(Int32.Parse(id));
                    link.Order = index++;
                    _LinkRepository.InsertOrUpdate(link);
                }
                _LinkRepository.Save();

                this.FlashInfo("The links were reordered successfully.");
                
            }
            catch (Exception ex)
            {
                this.FlashError("There was an error updating the link order: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            _LinkRepository.Delete(id);
            _LinkRepository.Save();

			this.FlashInfo("The link was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

