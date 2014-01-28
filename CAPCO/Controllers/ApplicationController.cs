using System;
using System.Linq;
using System.Web.Mvc;

using Excel;

using RestfulRouting.Format;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Domain;
using System.Web.Security;
using System.Reflection;
using CAPCO.Infrastructure.Services;
using System.Configuration;

namespace CAPCO.Controllers
{
    [CAPCO.Infrastructure.Core.HandleError]
    public abstract class ApplicationController : Controller
    {
        protected readonly IContentService _ContentService;

        public ApplicationController()
        {
            _ContentService = DependencyResolver.Current.GetService<IContentService>();
            //CurrentUser = Membership.GetUser().GetMember();
        }

        public ApplicationUser CurrentUser {
            get
            {
                return MembershipHelpers.GetCurrentUser();// Membership.GetUser().GetMember();
            }
        }

        protected ActionResult RespondTo(Action<FormatCollection> format)
        {
            return new FormatResult(format);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.BuildVersion = Assembly.GetAssembly(typeof(ApplicationController)).GetName().Version.ToString();
            ViewBag.FooterSection = _ContentService.GetContentSection(ContentSectionNames.Footer.ToString());
            @ViewBag.BlogUrl = ConfigurationManager.AppSettings["BlogUrl"];
            @ViewBag.BlogRssFeed = ConfigurationManager.AppSettings["BlogRssFeed"];
            @ViewBag.IsPricingEnabled = bool.Parse(ConfigurationManager.AppSettings["EnablePricingFeatures"]);
            base.OnActionExecuting(filterContext);
        }
    }

    
}