using System;
using System.Linq;
using System.Web.Mvc;
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
        protected readonly IApplicationUserRepository _AppUserRepo;
        protected readonly IContentService _ContentService;

        public ApplicationController()
        {
            _AppUserRepo = DependencyResolver.Current.GetService<IApplicationUserRepository>();
            _ContentService = DependencyResolver.Current.GetService<IContentService>();
            CurrentUser = Membership.GetUser().GetMember();
        }

        public ApplicationUser CurrentUser { get; set; }

        protected ActionResult RespondTo(Action<FormatCollection> format)
        {
            return new FormatResult(format);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // TODO: Need to sort out what to do if the user has a logged in cookie but the CurrentUser is null
            //if (CurrentUser != null)
            //{
            //    if (CurrentUser.Status == AccountStatus.Suspended.ToString())
            //    {
            //        @ViewBag.AccountMessage = "Your account has been suspended. Please contact customer service directly.";
            //        FormsAuthentication.SignOut();
            //        Response.Redirect(Url.Action("login", "account"));
            //    }
            //}

            ViewBag.BuildVersion = Assembly.GetAssembly(typeof(ApplicationController)).GetName().Version.ToString();
            ViewBag.FooterSection = _ContentService.GetContentSection(ContentSectionNames.Footer.ToString());
            @ViewBag.BlogUrl = ConfigurationManager.AppSettings["BlogUrl"];
            @ViewBag.BlogRssFeed = ConfigurationManager.AppSettings["BlogRssFeed"];
            @ViewBag.IsPricingEnabled = bool.Parse(ConfigurationManager.AppSettings["EnablePricingFeatures"]);
            //ViewData["Pages"] = _ContentPageRepo.All.Where(x => x.ParentPageId.HasValue && x.ParentPageId.Value > 0).ToList();
            base.OnActionExecuting(filterContext);
        }
    }

    
}