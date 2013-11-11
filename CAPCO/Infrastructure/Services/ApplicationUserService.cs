using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Drawing;
using CAPCO.Infrastructure.Domain;
using CAPCO.Models;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Infrastructure.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IRepository<ApplicationUser> applicationUserRepository;
        public ApplicationUserService(IRepository<ApplicationUser> applicationUserRepository)
        {
            this.applicationUserRepository = applicationUserRepository;
        }
        
        public ApplicationUser CreateNewUser(RegisterModel model, UserRoles role = UserRoles.ApplicationUsers)
        {
            // Attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
                
            if (createStatus == MembershipCreateStatus.Success)
            {
                try
                {
                    if (applicationUserRepository.All.Any(x => x.UserName == model.UserName))
                        throw new Exception("There is already an account with that username.");

                    // ensure unique username/email
                    if (applicationUserRepository.All.Any(x => x.Email == model.Email))
                        throw new Exception("There is already an account registered for that email address.");

                    // set up the new app user
                    var newUser = new ApplicationUser();
                    newUser.FirstName = model.FirstName;
                    newUser.LastName = model.LastName;
                    newUser.CompanyName = model.CompanyName;
                    newUser.Email = model.Email;
                    newUser.UserName = model.UserName;
                    newUser.ActivationKey = GenerateActivationKey();
                    newUser.IsActivated = false;
                    newUser.CanReceiveSystemEmails = true;
                    newUser.CanReceiveMarketingEmails = true;
                    newUser.PricePreference = PricePreferences.None.ToString();
                    newUser.AccountNumber = model.AccountNumber;
                    applicationUserRepository.InsertOrUpdate(newUser);
                    applicationUserRepository.Save();

                    // attach user to role
                    Roles.AddUserToRole(newUser.UserName, role.ToString());

                    return newUser;
                }
                catch (Exception ex)
                {
                    DeleteMember(model.UserName);
                    throw;
                }
            }
            else
            {
                throw new Exception(ErrorCodeToString(createStatus));
            }
        }

        public ApplicationUser CreateNewUser(ApplicationUser model, UserRoles role = UserRoles.ApplicationUsers)
        {
            // Attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser(model.UserName, GeneratePassword(), model.Email, null, null, true, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                try
                {
                    // ensure unique username/email
                    if (applicationUserRepository.All.Any(x => x.UserName == model.UserName))
                        throw new Exception("There is already an account with that username.");
                    if (applicationUserRepository.All.Any(x => x.Email == model.Email))
                        throw new Exception("There is already an account with that email address.");

                    // set up the new app user
                    model.ActivationKey = GenerateActivationKey();
                    model.IsActivated = model.IsActivated;

                    applicationUserRepository.InsertOrUpdate(model);
                    applicationUserRepository.Save();

                    // attach user to role
                    Roles.AddUserToRole(model.UserName, role.ToString());

                    return model;
                }
                catch (Exception ex)
                {
                    DeleteMember(model.UserName);
                    throw;
                }
            }
            else
            {
                throw new Exception(ErrorCodeToString(createStatus));
            }
        }

        public void EvictMembershipUser(string username)
        {
            var user = Membership.GetUser(username);
            if (user != null)
            {
                Membership.DeleteUser(username);
            }
        }

        public void DeleteMember(string username)
        {
            var customer = applicationUserRepository.All.Where(x => x.UserName == username).FirstOrDefault();
            if (customer != null)
                applicationUserRepository.Delete(customer.Id);

            EvictMembershipUser(username);
        }

        #region Membership Status Codes
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
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

        public string GenerateActivationKey()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private string GeneratePassword()
        {
            // TODO: Implement password generation
            return "Passw0rd";
        }
    }
}