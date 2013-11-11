using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;
using System.Web.Security;
using CAPCO.Controllers;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class ContentController : BaseAdminController
    {

        private readonly IRepository<ContentSection> _ContentSectionRepository;
        public ContentController(IRepository<ContentSection> contentSectionRepository)
        {
            _ContentSectionRepository = contentSectionRepository;			
        }

        public ViewResult Index()
        {
            return View(_ContentSectionRepository.All);
        }

        public ViewResult Show(int id)
        {
            return View(_ContentSectionRepository.Find(id));
        }

        public ActionResult New()
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ContentSection section)
        {
            if (ModelState.IsValid)
            {
                section.CreatedOn = DateTime.Now;
                section.CreatedBy = CurrentUser.UserName;
                section.LastModifiedOn = DateTime.Now;
                section.LastModifiedBy = CurrentUser.UserName;
                
                _ContentSectionRepository.InsertOrUpdate(section);
                _ContentSectionRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View("New", section);
            }
        }
        
        public ActionResult Edit(int id)
        {
            return View(_ContentSectionRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ContentSection section)
        {
            if (ModelState.IsValid)
            {

                var contentToUpdate = _ContentSectionRepository.Find(section.Id);
                contentToUpdate.LastModifiedBy = Membership.GetUser().UserName;
                contentToUpdate.LastModifiedOn = DateTime.Now;
                //contentToUpdate.SectionName = section.SectionName;
                contentToUpdate.Text = section.Text;
                contentToUpdate.Title = section.Title;

                _ContentSectionRepository.InsertOrUpdate(contentToUpdate);
                _ContentSectionRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Edit", section);
            }
        }

        public ActionResult Delete(int id)
        {
            //contentpageRepository.Delete(id);
            //contentpageRepository.Save();

            //this.FlashInfo("The page was deleted successfully.");

            //return RedirectToAction("Index");
            throw new NotImplementedException();
        }
    }
}

