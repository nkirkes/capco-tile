using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Security
{
    public class CapcoAuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
                return false;
            
            var currUser = MembershipHelpers.GetCurrentUser();
            if (currUser == null)
            {
                return false;
            }

            if (currUser.Status == AccountStatus.Suspended.ToString())
            {
                return false;
            }

            return isAuthorized;
        }
    }
}