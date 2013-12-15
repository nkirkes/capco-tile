using System.Web.Mvc;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Domain;
using CAPCO.Models;

namespace CAPCO.Controllers
{
    public class RequestAccountController : ApplicationController
    {
        private readonly IRepository<ApplicationUser> _AppUserRepo;
        /// <summary>
        /// Initializes a new instance of the RequestAccountController class.
        /// </summary>
        public RequestAccountController(IRepository<ApplicationUser> appUserRepo)
        {
            _AppUserRepo = appUserRepo;            
        }
        public ActionResult Show(int id)
        {
            var user = _AppUserRepo.Find(id);
            if (user == null)
            {
                this.FlashError("Sorry, but I couldn't find that user.");
                return RedirectToAction("index", "root", new { area = "" });
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult RequestAccount(RequestAccountViewModel model)
        {
            this.FlashError("The request notifications have not been implemented yet.");
            return View("Show", model);
        }
    }
}
