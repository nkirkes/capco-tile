using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Security;
using System.Web.Security;
using CAPCO.Models;
using CAPCO.Infrastructure.Mailers;
using Mvc.Mailer;

namespace CAPCO.Controllers
{
    [CapcoAuthorizationAttribute]
    public class ProfileController : ApplicationController
    {
        private readonly IApplicationUserRepository _AppUserRepo;
        private readonly IAccountRequestRepository _AccountRequestRepo;
        private readonly IPickupLocationRepository _PickupLocationRepo;
        /// <summary>
        /// Initializes a new instance of the ProfileController class.
        /// </summary>
        public ProfileController(IApplicationUserRepository appUserRepo, IAccountRequestRepository accountRequestRepo, IPickupLocationRepository pickupLocationRepo)
        {
            _PickupLocationRepo = pickupLocationRepo;
            _AccountRequestRepo = accountRequestRepo;
            _AppUserRepo = appUserRepo;            
        }

        public ActionResult Index()
        {
            return View("Show", CurrentUser);
        }

        public ActionResult Show(int id)
        {
            var user = _AppUserRepo.Find(id);

            if (user == null)
            {
                this.FlashError("I couldn't find that user.");
                return RedirectToAction("Index", "Root", new { area = "" });
            }
            return View(user);
        }

        public ActionResult Edit(int id = 0)
        {
            if (id <= 0 || id != CurrentUser.Id)
            {
                this.FlashError("Sorry, but you can only edit your own profile.");
                return RedirectToAction("Show", new { id });
            }

            var user = _AppUserRepo.Find(id);
            if (user == null)
            {
                this.FlashError("I couldn't find that user.");
                return RedirectToAction("Index", "Root", new { area = "" });
            }
            ViewBag.PossibleLocations = _PickupLocationRepo.All.ToList();
            return View(user);
        }

        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult Update(ApplicationUser user, int id)
        {
            try
            {
                if (CurrentUser.Id != id)
                    throw new Exception("You are not allowed to edit that profile.");

                var userToUpdate = _AppUserRepo.Find(id);
                var membershipUser = Membership.GetUser(userToUpdate.UserName);

                userToUpdate.Email = user.Email;

                membershipUser.Email = user.Email;
                Membership.UpdateUser(membershipUser);

                userToUpdate.City = user.City;
                userToUpdate.CompanyName = user.CompanyName;
                userToUpdate.Fax = user.Fax;
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Phone = user.Phone;
                userToUpdate.State = user.State;
                userToUpdate.StreetAddressLine1 = user.StreetAddressLine1;
                userToUpdate.StreetAddressLine2 = user.StreetAddressLine2;
                userToUpdate.Zip = user.Zip;
                userToUpdate.CanReceiveSystemEmails = user.CanReceiveSystemEmails;
                userToUpdate.CanReceiveMarketingEmails = user.CanReceiveMarketingEmails;
                if (Roles.IsUserInRole(UserRoles.ServiceProviders.ToString()))
                {
                    userToUpdate.PricePreference = user.PricePreference;

                    int locationId = 0;
                    if (Int32.TryParse(Request["SelectedLocation"], out locationId))
                    {
                        userToUpdate.DefaultLocation = _PickupLocationRepo.Find(locationId);
                    }
                }
                else
                {
                    userToUpdate.PricePreference = PricePreferences.None.ToString();
                }

                _AppUserRepo.InsertOrUpdate(userToUpdate);
                _AppUserRepo.Save();

                this.FlashInfo("Your profile has been updated.");
                return RedirectToAction("index", "profile");
            }
            catch (Exception ex)
            {
                this.FlashError("There was an error updating your profile: " + ex.Message);

            }

            return RedirectToAction("Edit");
        }


        private AccountRequest InitAccountRequestModel()
        {
            var model = new AccountRequest();
            model.StreetAddressLine1 = CurrentUser.StreetAddressLine1;
            model.StreetAddressLine2 = CurrentUser.StreetAddressLine2;
            model.City = CurrentUser.City;
            model.State = CurrentUser.State;
            model.Zip = CurrentUser.Zip;
            model.FirstName = CurrentUser.FirstName;
            model.LastName = CurrentUser.LastName;
            model.Phone = CurrentUser.Phone;
            model.Fax = CurrentUser.Fax;
            model.Email = CurrentUser.Email;
            model.CompanyName = CurrentUser.CompanyName;
            model.User = CurrentUser;
            return model;
        }

        public ActionResult RequestAccount()
        {
            if (CurrentUser.HasRequestedAccount)
            {
                this.FlashInfo("You have already submitted a request for an account. If you need to speak with someone immediately, please contact us at 800-727-2272.");
                return RedirectToAction("show", new { id = CurrentUser.Id });
            }

            AccountRequest model = InitAccountRequestModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RequestAccount(AccountRequest accountRequest)
        {
            if (CurrentUser.HasRequestedAccount)
            {
                this.FlashInfo("You have already submitted a request for an account. If you need to speak with someone immediately, please contact us at 800-727-2272.");
                return RedirectToAction("show", new { id = CurrentUser.Id });
            }

            try
            {
                accountRequest.User = CurrentUser;
                accountRequest.CreatedOn = DateTime.Now;
                accountRequest.HasBeenVerified = false;
                _AccountRequestRepo.InsertOrUpdate(accountRequest);
                _AccountRequestRepo.Save();

                CurrentUser.HasRequestedAccount = true;
                _AppUserRepo.InsertOrUpdate(CurrentUser);
                _AppUserRepo.Save();

                new AdminMailer().AccountRequest(accountRequest).Send();
                
                this.FlashInfo("Your request has been recieved. Someone will contact you as soon as possible.");
                return RedirectToAction("show", new { id = CurrentUser.Id });
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem sending the account request.");
            }

            return View(accountRequest);
        }
    }
}
