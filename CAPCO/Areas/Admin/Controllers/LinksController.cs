using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class LinksController : Controller
    {
		private readonly IRepository<Link> linkRepository;

		public LinksController(IRepository<Link> linkRepository)
        {
			this.linkRepository = linkRepository;
        }

        public ViewResult Index()
        {
            return View(linkRepository.All);
        }

        public ViewResult Show(int id)
        {
            return View(linkRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(Link link)
        {
            if (ModelState.IsValid) {
                link.Order = linkRepository.All.Count() + 1;
                linkRepository.InsertOrUpdate(link);
                linkRepository.Save();

				this.FlashInfo("The link was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem creating the link.");
				return View("New", link);
			}
        }
        
        public ActionResult Edit(int id)
        {
             return View(linkRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, Link link)
        {
            if (ModelState.IsValid)
            {
                var toUpdate = linkRepository.Find(id);
                toUpdate.Description = link.Description;
                toUpdate.Label = link.Label;
                toUpdate.Url = link.Url;

                
                linkRepository.InsertOrUpdate(toUpdate);
                linkRepository.Save();
                
                this.FlashInfo("The link was successfully saved.");

                return RedirectToAction("Index");
            } else {
				this.FlashError("There was a problem saving the link.");
				return View("Edit", link);
			}
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Reorder(string itemIds)
        {
            try
            {
                var ids = itemIds.Replace("&", "").Split(new string[] { "linkItem[]=" }, StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                foreach (var id in ids)
                {
                    var link = linkRepository.Find(Int32.Parse(id));
                    link.Order = index++;
                    linkRepository.InsertOrUpdate(link);
                }
                linkRepository.Save();

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
            linkRepository.Delete(id);
            linkRepository.Save();

			this.FlashInfo("The link was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

