using CAPCO.Infrastructure.Domain;
using CAPCO.Models;

namespace CAPCO.Infrastructure.Services
{
    public interface IApplicationUserService
    {
        ApplicationUser CreateNewUser(RegisterModel model, UserRoles role = UserRoles.ApplicationUsers);
        ApplicationUser CreateNewUser(ApplicationUser model, UserRoles role = UserRoles.ApplicationUsers);
        void EvictMembershipUser(string username);
        void DeleteMember(string username);
        string GenerateActivationKey();
    }
}