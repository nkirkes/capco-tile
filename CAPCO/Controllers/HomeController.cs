using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Data;
using System.Configuration;
using CAPCO.Infrastructure.Services;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Mailers;
using Mvc.Mailer;

namespace CAPCO.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly IContactRequestRepository _ContactRequestRepository;
        private readonly ILinkRepository _LinkRepo;
        public HomeController(IContactRequestRepository contactRequestRepository, ILinkRepository linkRepo)
        {
            _LinkRepo = linkRepo;
            _ContactRequestRepository = contactRequestRepository;            
        }

        public ActionResult Index(string id = "")
        {
            @ViewBag.WelcomeSection = _ContentService.GetContentSection(ContentSectionNames.Welcome.ToString());
            @ViewBag.WhatWeDoSection = _ContentService.GetContentSection(ContentSectionNames.WhatWeDo.ToString());
            @ViewBag.WhoWeAreSection = _ContentService.GetContentSection(ContentSectionNames.WhoWeAre.ToString());

            return View();
        }

        public ActionResult Access()
        {
            return Redirect("http://216.197.120.185/access");
        }

        public ActionResult Showroom()
        {
            return Redirect("http://216.197.120.185/showroom");
        }

        
        public ActionResult About(string section = "")
        {
            switch (section)
            {
                case "tile":
                    ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.AboutTile.ToString());
                    break;
                case "stone":
                    ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.AboutStone.ToString());
                    break;
                case "slab":
                    ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.AboutSlab.ToString());
                    break;
                case "handpainting":
                    ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.AboutHandPainting.ToString());
                    break;
                case "employment":
                    ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.AboutEmployment.ToString());
                    break;
                case "handpaint":
                    ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.AboutHandPainting.ToString());
                    break;
                default:
                    ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.AboutUs.ToString());
                    break;
            }
            
            return View();
        }

        public ActionResult Privacy()
        {
            ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.Privacy.ToString());
            return View();
        }

        public ActionResult Locations()
        {
            ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.Locations.ToString());
            return View();
        }

        public ActionResult Links()
        {
            ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.Links.ToString());
            var links = _LinkRepo.All.OrderBy(x => x.Order);
            return View(links.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.Contact.ToString());
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactRequest(ContactRequest request)
        {
            try
            {
                // store the contact request
                request.CreatedOn = DateTime.Now;
                request.IsArchived = false;

                try
                {
                    new AdminMailer().ContactRequest(request).SendAsync();
                }
                catch (Exception ex)
                {
                        
                }

                _ContactRequestRepository.InsertOrUpdate(request);
                _ContactRequestRepository.Save();

                // display a msg to the user
                this.FlashInfo("Your message has been sent. Someone will respond as soon as possible.");
                return RedirectToAction("contact");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem submitting your information. If you continue to have trouble, please contact us directly at 303-759-1919.");
            }
            ViewBag.Section = _ContentService.GetContentSection(ContentSectionNames.Contact.ToString());
            return View("Contact", request);
        }
    }
}
