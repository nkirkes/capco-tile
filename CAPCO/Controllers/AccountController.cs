using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CAPCO.Models;
using CAPCO.Infrastructure.Services;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Mailers;
using CAPCO.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;
using CAPCO.Infrastructure.Security;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using Mvc.Mailer;

namespace CAPCO.Controllers
{
    public class AccountController : ApplicationController
    {
        private readonly IApplicationUserService _CustomerService;
        private readonly IRepository<ApplicationUser> _ApplicationUserRepository;
        private readonly IRepository<ProjectInvitation> _ProjectInvitationRepository;
        private readonly IRepository<Project> _ProjectRepository;
        private readonly IContentService _ContentService;

        public AccountController(IApplicationUserService customerService,
            IRepository<ApplicationUser> applicationUserRepository,
            IRepository<ProjectInvitation> projectInvitationRepository,
            IRepository<Project> projectRepository, IContentService contentService)
        {
            _ProjectRepository = projectRepository;
            _ProjectInvitationRepository = projectInvitationRepository;
            _ApplicationUserRepository = applicationUserRepository;
            _CustomerService = customerService;
            _ContentService = contentService;
        }

        public ActionResult LogIn()
        {
            var model = new LogOnModel();
            string key = Request.QueryString["t"];

            if (!String.IsNullOrWhiteSpace(key))
            {
                // this is an invite, need to attach the invitation details
                var invite = _ProjectInvitationRepository.All.FirstOrDefault(x => x.InvitationKey == key);
                if (invite != null)
                {
                    model.InvitationKey = key;
                }
                else
                {
                    this.FlashError("The project invitation you are responding to has expired or has already been used. You can still logon, but you won't be able to see the project until another invitation has been issued.");
                }
            }

            if (CurrentUser != null && CurrentUser.Status == AccountStatus.Suspended.ToString())
            { 
                ViewBag.AccountMessage = "Sorry, but your account has been suspended. Please contact customer service directly.";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult LogIn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        var customer = _ApplicationUserRepository.All.FirstOrDefault(x => x.UserName == model.UserName);
                        if (customer != null)
                        {
                            if (customer.IsActivated == false)
                            {
                                model.ShowActivationMessage = true;
                                model.ActivationToken = customer.ActivationKey;
                                return View(model);
                            }

                            if (customer.Status == AccountStatus.Suspended.ToString())
                            {
                                ViewBag.AccountMessage = "Your account has been suspended. Please contact customer service directly.";
                                return View(model);
                            }
                        }

                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                        if (!String.IsNullOrWhiteSpace(model.InvitationKey))
                        {
                            // this is an invitee, so we need to activate them, log them in, and take them to the project
                            var invite = _ProjectInvitationRepository.All.FirstOrDefault(x => x.InvitationKey == model.InvitationKey);
                            if (invite != null)
                            {
                                var project = _ProjectRepository.Find(invite.ProjectId);
                                project.AddUser(customer);
                                _ProjectRepository.Save();

                                // kill the invitation
                                _ProjectInvitationRepository.Delete(invite.Id);
                                _ProjectInvitationRepository.Save();

                                this.FlashInfo(String.Format("You have successfully joined the <em>{0}</em> project.", project.ProjectName));
                                return RedirectToAction("index", "projects");
                            }
                        }

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        this.FlashError("The user name or password provided is incorrect.");
                    }
                }
                catch (Exception ex)
                {
                    this.FlashError("There were problems logging you in: " + ex.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        
        public ActionResult Register()
        {
            var model = new RegisterModel();
            string key = Request.QueryString["t"];

            if (!String.IsNullOrWhiteSpace(key))
            { 
                // this is an invite, need to attach the invitation details
                var invite = _ProjectInvitationRepository.All.FirstOrDefault(x => x.InvitationKey == key);
                if (invite != null)
                {
                    model.InvitationKey = key;
                    this.FlashInfo("It looks like you're responding to a project invitation. You'll need to register to see the project details, or log in if you've already registered.");
                }
                else
                {
                    this.FlashError("The project invitation you are responding to has expired or has already been used. You can still register, but you won't be able to see the project until another invitation has been issued.");
                }

            }

            ViewBag.WhyRegisterSection = _ContentService.GetContentSection(ContentSectionNames.WhyRegister.ToString());
            return View(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            
            
            ViewBag.WhyRegisterSection = _ContentService.GetContentSection(ContentSectionNames.WhyRegister.ToString());
            if (ModelState.IsValid)
            {
                try
                {
                    RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
                    if (String.IsNullOrEmpty(recaptchaHelper.Response))
                    {
                        throw new Exception("Captcha answer cannot be empty.");
                    }
                    
                    try
                    {
                        RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

                        if (recaptchaResult != RecaptchaVerificationResult.Success)
                        {
                            throw new Exception("Incorrect captcha answer.");
                        }
                    }
                    catch (Exception e)
                    {
                        // swallow a server error from recaptcha and carry on.
                    }

                    UserRoles role = model.AccountType == AccountTypes.ServiceProvider.ToString() ? UserRoles.ServiceProviders : UserRoles.Consumers;
                    var customer = _CustomerService.CreateNewUser(model, role);

                    if (!String.IsNullOrWhiteSpace(model.InvitationKey))
                    {
                        // this is an invitee, so we need to activate them, log them in, and take them to the project
                        var invite = _ProjectInvitationRepository.All.FirstOrDefault(x => x.InvitationKey == model.InvitationKey);
                        if (invite != null)
                        {
                            customer.IsActivated = true;
                            customer.Status = AccountStatus.Active.ToString();
                            _ApplicationUserRepository.InsertOrUpdate(customer);
                            _ApplicationUserRepository.Save();

                            var project = _ProjectRepository.Find(invite.ProjectId);
                            project.AddUser(customer);
                            _ProjectRepository.Save();

                            // kill the invitation
                            _ProjectInvitationRepository.Delete(invite.Id);
                            _ProjectInvitationRepository.Save();

                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            this.FlashInfo("Welcome to CAPCO! Your registration is complete. Since you were invited to view a project, we've taken you to your projects list, but feel free to browse around.");
                            return RedirectToAction("index", "projects");
                        }
                        else
                        { 
                            // hmm, no invite, just process them normally.
                            new ApplicationUserMailer().Activation(customer).Send();
                            this.FlashInfo("<b>Welcome to CAPCO!</b> We've sent an activation email to your email address. You'll need to grab that before you can log in.");
                            return RedirectToAction("Index", "Root", new { area = "" });
                        }
                    }
                    else
                    {
                        new ApplicationUserMailer().Activation(customer).Send();
                        this.FlashInfo("<b>Welcome to CAPCO!</b> We've sent an activation email to your email address. You'll need to grab that before you can log in.");
                        return RedirectToAction("Index", "Root", new { area = "" });
                    }
                }
                catch (Exception ex)
                {
                    // ensure user roll back
                    _CustomerService.DeleteMember(model.Email);
                    ModelState.AddModelError("MembershipException", ex);
                    this.FlashError(ex.Message);
                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string email)
        {
            try
            {
                if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "username");

                var userName = Membership.GetUserNameByEmail(email);
                
                if (String.IsNullOrWhiteSpace(userName))
                    throw new Exception("Sorry, but we have no record of a user with that email address.");

                var user = Membership.GetUser(userName);
                var newPassword = user.ResetPassword();
                var customer = _ApplicationUserRepository.All.FirstOrDefault(x => x.UserName == userName);

                // send the user an email stating their password has changed.
                new ApplicationUserMailer().PasswordReset(customer.FirstName, customer.UserName, newPassword, user.Email).Send();
                this.FlashInfo("Your password was reset and sent to your email address on file.");
                return RedirectToAction("LogIn");
            }
            catch (Exception ex)
            {
                this.FlashError(ex.Message);
            }

            return View();
        }


        //
        // GET: /Account/ChangePassword

        [CapcoAuthorizationAttribute]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [CapcoAuthorizationAttribute]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    this.FlashInfo("Your password has been updated successfully.");
                    return RedirectToAction("show", "profile", new { id = CurrentUser.Id });
                }
                else
                {
                    this.FlashError("The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult Activate()
        {
            string key = Request.QueryString["t"];
            // activate the account, reset the send them to login
            if (!String.IsNullOrEmpty(key))
            {
                // find an account that matches the activation key
                var customer = _ApplicationUserRepository.All.FirstOrDefault(x => x.ActivationKey == key);

                if (customer != null && !customer.IsActivated)
                {
                    customer.IsActivated = true;
                    customer.Status = AccountStatus.Active.ToString();
                    _ApplicationUserRepository.InsertOrUpdate(customer);
                    _ApplicationUserRepository.Save();

                    // send a notification to the admins that a new account has been created and activated.
                    try
                    {
                        new AdminMailer().NewAccountNotification(customer).Send();    
                    }
                    catch
                    {
                        // swallow the exception? ugly
                    }
                    
                    this.FlashInfo("Your account has been activated. You may now login and start using CAPCO!");
                    return RedirectToAction("LogIn", "Account");
                }
            }

            this.FlashError("There was a problem activating the account. Please double check the activation key we sent to your email address.");
            return RedirectToAction("Index", "Root");
        }
        
        public ActionResult ResendActivation(string key)
        {
            try
            {
                var customer = _ApplicationUserRepository.All.FirstOrDefault(x => x.ActivationKey == key);
                if (customer != null)
                {
                    new ApplicationUserMailer().Activation(customer).Send();
                    this.FlashInfo(@"<b>Welcome to The Pype!</b> We have re-sent your activation link via email. It should arrive shortly (but double check your spam filter if it doesn't).");
                }
            }
            catch (Exception ex)
            {
                this.FlashError("There was an error sending the activation email.");
            }

            return RedirectToAction("Index", "Root", new { area = "" });
        }
    }
}
