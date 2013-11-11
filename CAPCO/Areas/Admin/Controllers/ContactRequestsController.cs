using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   
    public class ContactRequestsController : BaseAdminController
    {
		private readonly IRepository<ContactRequest> contactrequestRepository;

		public ContactRequestsController(IRepository<ContactRequest> contactrequestRepository)
        {
			this.contactrequestRepository = contactrequestRepository;
        }

        public ViewResult Index()
        {
            return View(contactrequestRepository.All);
        }

        public ViewResult Show(int id)
        {
            return View(contactrequestRepository.Find(id));
        }

        public ActionResult Edit(int id)
        {
             return View(contactrequestRepository.Find(id));
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, ContactRequest contactrequest)
        {
            try
            {
                var toUpdate = contactrequestRepository.Find(id);
                toUpdate.IsArchived = contactrequest.IsArchived;

                contactrequestRepository.InsertOrUpdate(toUpdate);
                contactrequestRepository.Save();

                this.FlashInfo("The contact request was successfully saved.");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem saving the contact request: " + ex.Message);        
            }

			return View("Edit", contactrequest);
        }

        public ActionResult Delete(int id)
        {
            contactrequestRepository.Delete(id);
            contactrequestRepository.Save();

			this.FlashInfo("The contact request was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

