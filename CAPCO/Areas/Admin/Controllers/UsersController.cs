using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;
using CAPCO.Models;
using CAPCO.Infrastructure.Services;
using CAPCO.Infrastructure.Mailers;
using System.Web.Security;
using CAPCO.Helpers;
using CAPCO.Areas.Admin.Models;
using PagedList;
using CAPCO.Infrastructure.Specifications;
using Mvc.Mailer;

namespace CAPCO.Areas.Admin.Controllers
{
    public class UsersController : BaseAdminController
    {
        private readonly IApplicationUserService _AppUserService;
        private readonly IRepository<PickupLocation> _LocationRepository;
        private readonly IRepository<DiscountCode> _DiscountCodeRepository;
        private readonly IRepository<ApplicationUser> applicationuserRepository;
        

		public UsersController(IRepository<ApplicationUser> applicationuserRepository, 
            IApplicationUserService appUserService,
            IRepository<PickupLocation> locationRepository,
            IRepository<DiscountCode> discountCodeRepository)
        {
            _DiscountCodeRepository = discountCodeRepository;
            _LocationRepository = locationRepository;
            _AppUserService = appUserService;
			this.applicationuserRepository = applicationuserRepository;
        }

        private void Init()
        {
            ViewBag.PossibleDiscountCodes = _DiscountCodeRepository.All.ToList();
            ViewBag.PossibleLocations = _LocationRepository.All.ToList();
        }

        public ViewResult Index(PagedViewModel<ApplicationUser> model)
        {
            var results = applicationuserRepository.All.OrderBy(x => x.UserName);
            model.TotalCount = results.Count();
            model.Entities = results.ToPagedList(model.Page ?? 1, 100);
            return View("Index", model);
        }

        public ActionResult Search(PagedViewModel<ApplicationUser> model)
        {
            var results = applicationuserRepository.FindBySpecification(new UsersByUserNameOrCompanyNameSpecification(model.Criteria)).OrderBy(x => x.UserName);
            model.TotalCount = results.Count();
            model.Entities = results.ToPagedList(model.Page ?? 1, 100);
            return View("Index", model);
        }

        public ActionResult Show(int id)
        {
            ApplicationUser user = applicationuserRepository.AllIncluding(x => x.DiscountCode, x => x.DefaultLocation, x => x.Notifications).FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                this.FlashError("There is no user with that id.");
                return RedirectToAction("index");
            }
            return View(user);

        }

        public ActionResult New()
        {
            Init();
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ApplicationUser applicationuser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var selectedRole = Request["SelectedRole"] != null ? (UserRoles)Enum.Parse(typeof(UserRoles), Request["SelectedRole"]) : UserRoles.ApplicationUsers;
                    
                    var newUser = _AppUserService.CreateNewUser(applicationuser, selectedRole);
                    
                    newUser.CanReceiveSystemEmails = true;
                    newUser.CanReceiveMarketingEmails = true;
                    newUser.PricePreference = PricePreferences.None.ToString();

                    newUser.State = Request["SelectedState"] != null ? ((States)Enum.Parse(typeof(States), Request["SelectedState"])).ToString() : "";
                    
                    string selectedStatus = Request["SelectedStatus"] ?? AccountStatus.Active.ToString();
                    newUser.Status = ((AccountStatus)Enum.Parse(typeof(AccountStatus), selectedStatus)).ToString();

                    int locationId = 0;
                    if (Int32.TryParse(Request["SelectedLocation"], out locationId))
                    {
                        newUser.DefaultLocation = _LocationRepository.Find(locationId);
                    }

                    int discontCodeId = 0;
                    if (Int32.TryParse(Request["SelectedDiscountCode"], out discontCodeId))
                    {
                        newUser.DiscountCode = _DiscountCodeRepository.Find(discontCodeId);
                    }
                    
                    applicationuserRepository.InsertOrUpdate(newUser);
                    applicationuserRepository.Save();

                    new ApplicationUserMailer().Activation(newUser).Send();
                    this.FlashInfo("The user was successfully created. A welcome email with their username and password has been sent.");
                    return RedirectToAction("Index", "Users", new { area = "admin" });
                }
                catch (Exception ex)
                {
                    // ensure user roll back
                    _AppUserService.DeleteMember(applicationuser.UserName);
                    //ModelState.AddModelError("MembershipException", ex);
                    this.FlashError(ex.Message);
                }
            }
            
            //this.FlashError("There was a problem creating the user.");
            ViewBag.SelectedRole = Request["SelectedRole"];
            ViewBag.SelectedState = Request["SelectedState"];
            Init();
            return View("New", applicationuser);
        }
        
        public ActionResult Edit(int id)
        {
            ApplicationUser user = applicationuserRepository.AllIncluding(x => x.DiscountCode, x => x.DefaultLocation, x => x.Notifications).FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                this.FlashError("There is no user with that id.");
                return RedirectToAction("index");
            }

            Init();
            return View(user);
        }

        
        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ApplicationUser applicationuser)
        {
            try
            {
                var user = applicationuserRepository.Find(applicationuser.Id);
                var memUser = Membership.GetUser(user.UserName);

                user.Email = applicationuser.Email;

                try
                {
                    memUser.Email = applicationuser.Email;
                    Membership.UpdateUser(memUser);
                }
                catch (Exception ex)
                {
                    throw new Exception("The email address is invalid or is already in use.");   
                }

                user.AccountNumber = applicationuser.AccountNumber;
                user.City = applicationuser.City;
                user.CompanyName = applicationuser.CompanyName;
                
                user.Fax = applicationuser.Fax;
                user.FirstName = applicationuser.FirstName;
                user.IsActivated = applicationuser.IsActivated;
                user.LastName = applicationuser.LastName;
                user.Phone = applicationuser.Phone;
                user.StreetAddressLine1 = applicationuser.StreetAddressLine1;
                user.StreetAddressLine2 = applicationuser.StreetAddressLine2;
                user.State = applicationuser.State;
                user.Zip = applicationuser.Zip;


                user.State = Request["SelectedState"] != null ? ((States)Enum.Parse(typeof(States), Request["SelectedState"])).ToString() : user.State;

                int locationId = 0;
                if (Int32.TryParse(Request["SelectedLocation"], out locationId))
                {
                    user.DefaultLocation = _LocationRepository.Find(locationId);
                }

                int discontCodeId = 0;
                if (Int32.TryParse(Request["SelectedDiscountCode"], out discontCodeId))
                {
                    user.DiscountCode = _DiscountCodeRepository.Find(discontCodeId);
                }

                user.Status = Request["SelectedStatus"] != null && !String.IsNullOrWhiteSpace(Request["SelectedStatus"]) ? ((AccountStatus)Enum.Parse(typeof(AccountStatus), Request["SelectedStatus"])).ToString() : AccountStatus.Active.ToString();

                var selectedRole = Request["SelectedRole"] != null && !String.IsNullOrWhiteSpace(Request["SelectedRole"]) ? (UserRoles)Enum.Parse(typeof(UserRoles), Request["SelectedRole"]) : UserRoles.ApplicationUsers;
                // clear user from all roles
                Roles.RemoveUserFromRoles(user.UserName, Roles.GetRolesForUser(user.UserName));
                // add user to selected role
                Roles.AddUserToRole(user.UserName, selectedRole.ToString());

                applicationuserRepository.InsertOrUpdate(user);
                applicationuserRepository.Save();

                this.FlashInfo("The user was successfully saved.");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem saving the user: " + ex.Message);
            }
                
            Init();
			return View("Edit", applicationuser);
        }

        public ActionResult Delete(int id)
        {
            
            try
            {
                var user = applicationuserRepository.Find(id);
                string username = user.UserName;

                applicationuserRepository.Detach(user);
                applicationuserRepository.Delete(id);
                applicationuserRepository.Save();

                _AppUserService.DeleteMember(username);
                this.FlashInfo("The user was successfully deleted.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem deleting the user: " + ex.Message);
            }

            return RedirectToAction("edit", new { id = id });
        }

        public ActionResult ResetPassword(int id)
        {
            var appUser = applicationuserRepository.Find(id);
            if (appUser != null)
            {
                try
                {
                    var user = Membership.GetUser(appUser.UserName);
                    var newPassword = user.ResetPassword();

                    // send the user an email stating their password has changed.
                    new ApplicationUserMailer().PasswordReset(!String.IsNullOrWhiteSpace(appUser.FirstName) ? appUser.FirstName : appUser.CompanyName, appUser.UserName, newPassword, user.Email).Send();
                    this.FlashInfo("The user's password was reset and sent to the email address on file.");
                }
                catch (Exception ex)
                {
                    this.FlashError("There was a problem resetting the user's password: " + ex.Message);   
                }
            }
            else
            {
                this.FlashError("I could not find a user with that id.");
            }
            return RedirectToAction("Show", new { id });
        }

        public ActionResult UnlockUser(int id)
        {
            var appUser = applicationuserRepository.Find(id);
            if (appUser != null)
            {
                try
                {
                    var user = Membership.GetUser(appUser.UserName);
                    if (user.IsLockedOut)
                        user.UnlockUser();

                    // send the user an email stating their password has changed.
                    this.FlashInfo("The user has been unlocked.");
                }
                catch (Exception ex)
                {
                    this.FlashError("There was a problem unlocking the user: " + ex.Message);
                }
            }
            else
            {
                this.FlashError("I could not find a user with that id.");
            }
            return RedirectToAction("Show", new { id });
        }

    }
}

