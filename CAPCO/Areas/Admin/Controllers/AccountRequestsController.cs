using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Areas.Admin.Controllers
{   

    public class AccountRequestsController : BaseAdminController
    {
		private readonly IAccountRequestRepository accountrequestRepository;

		public AccountRequestsController(IAccountRequestRepository accountrequestRepository)
        {
			this.accountrequestRepository = accountrequestRepository;
        }

        public ViewResult Index()
        {
            return View(accountrequestRepository.AllIncluding(x => x.User));
        }

        public ActionResult Show(int id)
        {
            AccountRequest request = accountrequestRepository.Find(id);
            if (request == null)
            {
                this.FlashError("That request does not exist.");
                return RedirectToAction("index");
            }
            return View(request);
        }

        public ActionResult Edit(int id)
        {
            AccountRequest request = accountrequestRepository.Find(id);
            if (request == null)
            {
                this.FlashError("That request does not exist.");
                return RedirectToAction("index");
            }
            return View(request);
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(int id, AccountRequest accountrequest)
        {
            try
            {
                var toUpdate = accountrequestRepository.Find(id);
                toUpdate.AccountNumber = accountrequest.AccountNumber;
                toUpdate.City = accountrequest.City;
                toUpdate.CompanyName = accountrequest.CompanyName;
                toUpdate.Email = accountrequest.Email;
                toUpdate.Fax = accountrequest.Fax;
                toUpdate.FirstName = accountrequest.FirstName;
                toUpdate.HasBeenVerified = accountrequest.HasBeenVerified;
                toUpdate.LastName = accountrequest.LastName;
                toUpdate.Notes = accountrequest.Notes;
                toUpdate.Phone = accountrequest.Phone;
                toUpdate.State = accountrequest.State;
                toUpdate.StreetAddressLine1 = accountrequest.StreetAddressLine1;
                toUpdate.StreetAddressLine2 = accountrequest.StreetAddressLine2;
                toUpdate.Zip = accountrequest.Zip;

                accountrequestRepository.InsertOrUpdate(toUpdate);
                accountrequestRepository.Save();

				this.FlashInfo("The account request was successfully saved.");

                return RedirectToAction("Index");	
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem saving the account request: " + ex.Message);	
            }
            			
			return View("Edit", accountrequest);
			
        }

        public ActionResult Delete(int id)
        {
            accountrequestRepository.Delete(id);
            accountrequestRepository.Save();

			this.FlashInfo("The account request was successfully deleted.");

            return RedirectToAction("Index");
        }
    }
}

