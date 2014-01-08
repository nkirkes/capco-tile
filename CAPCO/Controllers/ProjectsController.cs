using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Mailers;
using System.Configuration;
using System.Web.Security;
using CAPCO.Infrastructure.Security;
using Mvc.Mailer;

namespace CAPCO.Controllers
{
    [CapcoAuthorizationAttribute]
    public class ProjectsController : ApplicationController
    {
        private readonly IRepository<Product> _ProductRepository;
        private readonly IRepository<Project> _ProjectRepository;
        private readonly IRepository<ProjectComment> _ProjectCommentRepository;
        private readonly IRepository<PickupLocation> _PickupLocationRepo;
        private readonly IRepository<ProjectInvitation> _InviteRepo;
        private readonly IRepository<ApplicationUser> _AppUserRepo; 
        
        /// <summary>
        /// Initializes a new instance of the ProjectsController class.
        /// </summary>
        public ProjectsController(IRepository<Product> productRepository, 
            IRepository<Project> projectRepository, 
            IRepository<ProjectComment> projectCommentRepository,
            IRepository<PickupLocation> pickupLocationRepo,
            IRepository<ProjectInvitation> inviteRepo,
            IRepository<ApplicationUser> appUserRepo)
        {
            _InviteRepo = inviteRepo;
            _PickupLocationRepo = pickupLocationRepo;
            _ProjectCommentRepository = projectCommentRepository;
            _ProjectRepository = projectRepository;
            _ProductRepository = productRepository;
            _AppUserRepo = appUserRepo;
        }

        public ActionResult Index()
        {
            return View(CurrentUser.Projects);
        }
        
        public ActionResult Archives()
        {
            int expirationPeriodInDays = Convert.ToInt32(ConfigurationManager.AppSettings["ProjectExpirationInDays"]) * -1;
            var expirationDate = DateTime.Today.AddDays(expirationPeriodInDays);
            
            return View(CurrentUser.Projects.Where(x => x.LastModifiedOn <= expirationDate).ToList());
        }

        public ActionResult Show(int id)
        {
            var project = _ProjectRepository.AllIncluding(x => x.Products).FirstOrDefault(x => x.Id == id);
            if (project == null)
            {
                this.FlashError("Oops. Looks like that project doesn't exist.");
                return RedirectToAction("index", "projects");
            }

            if (project.Users!= null && project.Users.Any() && !project.Users.Contains(CurrentUser))
            {
                this.FlashError("You don't have permission to view that project.");
                return RedirectToAction("index", "projects");
            }

            return View(project);
        }

        public ActionResult New(int productId = 0)
        {
            var newProject = new Project();

            if (productId > 0)
            {
                var prod = _ProductRepository.Find(productId);
                var item = new ProjectItem { Product = prod };
                newProject.AddProduct(item);
            }

            if (Roles.IsUserInRole(UserRoles.ServiceProviders.ToString()))
            {
                newProject.PriceDisplayPreference = !String.IsNullOrEmpty(CurrentUser.PricePreference) ? CurrentUser.PricePreference : PricePreferences.None.ToString();
                newProject.Location = CurrentUser.DefaultLocation ?? null;
            }

            ViewBag.PossibleLocations = _PickupLocationRepo.All.ToList();
            return View(newProject);
        }

        

        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project, int productId = 0)
        {
            Product initialProduct = null;
            if (productId > 0)
                initialProduct = _ProductRepository.Find(productId);
            
            try
            {
                if (initialProduct != null)
                {
                    var item = new ProjectItem { Product = initialProduct };
                    item.Comment = Request.Form["ItemComment"];
                    project.AddProduct(item);
                }

                project.CreatedBy = CurrentUser;
                project.CreatedOn = DateTime.Now;
                project.LastModifiedOn = DateTime.Now;
                project.AddUser(CurrentUser);
                if (Roles.IsUserInRole(UserRoles.ServiceProviders.ToString()))
                {
                    //project.PriceDisplayPreference = !String.IsNullOrEmpty(CurrentUser.PricePreference) ? CurrentUser.PricePreference : PricePreferences.None.ToString();
                    //project.Location =  CurrentUser.DefaultLocation ?? null; 
                    if (!String.IsNullOrWhiteSpace(Request["SelectedLocation"]))
                    //if (project.Location != null && project.Location.Id > 0)
                    {
                        var locationId = Int32.Parse(Request["SelectedLocation"]);
                        project.Location = _PickupLocationRepo.Find(locationId);
                    }
                    else
                        project.Location = CurrentUser.DefaultLocation;
                }
                
                _ProjectRepository.InsertOrUpdate(project);
                _ProjectRepository.Save();
                                
                this.FlashInfo("The project was created successfully.");
                return RedirectToAction("Show", new { id = project.Id });
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem saving your project: " + ex.Message);
            }

            ViewBag.PossibleLocations = _PickupLocationRepo.All.ToList();
            return View("New", project);
        }

        public ActionResult Edit(int id)
        {
            var project = _ProjectRepository.Find(id);
            if (project == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            if (project.CreatedBy != CurrentUser)
            {
                this.FlashError("You don't have permission to edit that project.");
                return RedirectToAction("show", new { id });
            }
            ViewBag.PossibleLocations = _PickupLocationRepo.All.ToList();
            return View(project);
        }

        [HttpPut, ValidateAntiForgeryToken()]
        public ActionResult Update(int id, Project project)
        {
            var projectToUpdate = _ProjectRepository.Find(id);
            if (projectToUpdate == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            if (projectToUpdate.CreatedBy != CurrentUser)
            {
                this.FlashError("You don't have permission to edit that project.");
                return RedirectToAction("show", new { id });
            }

            try
            {
                projectToUpdate.Description = project.Description;
                projectToUpdate.ProjectName = project.ProjectName;
                projectToUpdate.LastModifiedOn = DateTime.Now;
                if (Roles.IsUserInRole(UserRoles.ServiceProviders.ToString()))
                {
                    projectToUpdate.PriceDisplayPreference = project.PriceDisplayPreference;
                    if (!String.IsNullOrWhiteSpace(Request["SelectedLocation"]))
                    //if (project.Location != null && project.Location.Id > 0)
                    {
                        var locationId = Int32.Parse(Request["SelectedLocation"]);
                        projectToUpdate.Location = _PickupLocationRepo.Find(locationId);
                    }
                    else
                        projectToUpdate.Location = CurrentUser.DefaultLocation;
                }
                else
                {
                    projectToUpdate.PriceDisplayPreference = PricePreferences.None.ToString();
                }
                _ProjectRepository.InsertOrUpdate(projectToUpdate);
                _ProjectRepository.Save();

                this.FlashInfo("The project was successfully updated.");
                return RedirectToAction("show", new { id });
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem updating the project.");    
            }

            project.Products = projectToUpdate.Products;
            ViewBag.PossibleLocations = _PickupLocationRepo.All.ToList();
            return View("Edit", project);
        }

        public ActionResult RemoveProduct(int id, int productId) // productId refers to ProjectItem Id
        {
            var projectToUpdate = _ProjectRepository.Find(id);
            if (projectToUpdate == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            if (projectToUpdate.CreatedBy != CurrentUser)
            {
                this.FlashError("You don't have permission to edit that project.");
                return RedirectToAction("show", new { id });
            }

            try
            {
                var product = projectToUpdate.Products.FirstOrDefault(x => x.Id == productId);
                if (product != null)
                {
                    projectToUpdate.Products.Remove(product);
                }
                projectToUpdate.LastModifiedOn = DateTime.Now;
                _ProjectRepository.InsertOrUpdate(projectToUpdate);
                _ProjectRepository.Save();
                this.FlashInfo("The product was successfully removed from the project.");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem removing the product from the project.");
            }

            return RedirectToAction("edit", new { id });
        }

        public ActionResult UpdateItemComment(int id, int projectId)
        {
            var project = _ProjectRepository.Find(projectId);
            var item = project.Products.FirstOrDefault(x => x.Id == id);

            try
            {
                item.Comment = Request.Form["ItemComment"];
                _ProjectRepository.InsertOrUpdate(project);
                _ProjectRepository.Save();
                this.FlashInfo("The comment was successfully updated.");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem updating the item comment.");
            }

            return RedirectToAction("edit", new { id = projectId });
        }

        public ActionResult RemoveMember(int id, int memberId)
        {
            var projectToUpdate = _ProjectRepository.Find(id);
            if (projectToUpdate == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            if (projectToUpdate.CreatedBy != CurrentUser)
            {
                this.FlashError("You don't have permission to edit that project.");
                return RedirectToAction("show", new { id });
            }

            try
            {
                var user = _AppUserRepo.Find(memberId);
                if (user != null)
                {
                    projectToUpdate.Users.Remove(user);
                }
                projectToUpdate.LastModifiedOn = DateTime.Now;
                _ProjectRepository.InsertOrUpdate(projectToUpdate);
                _ProjectRepository.Save();
                this.FlashInfo("The member was successfully removed from the project.");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem removing the member from the project.");
            }

            return RedirectToAction("edit", new { id });
        }

        public ActionResult RemoveInvite(int id, int inviteId)
        {
            var projectToUpdate = _ProjectRepository.Find(id);
            if (projectToUpdate == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            if (projectToUpdate.CreatedBy != CurrentUser)
            {
                this.FlashError("You don't have permission to edit that project.");
                return RedirectToAction("show", new { id });
            }

            try
            {
                _InviteRepo.Delete(inviteId);
                _InviteRepo.Save();
                this.FlashInfo("The project invitation was successfully removed from the project.");
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem removing the invitation from the project.");
            }

            return RedirectToAction("edit", new { id });
        }

        public ActionResult DeleteComment(int id)
        {
            var comment = _ProjectCommentRepository.Find(id);
            if (comment == null)
            {
                this.FlashError("Oops, that comment doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            var projectToUpdate = comment.Project;
            if (projectToUpdate == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            // TODO: Something is borked here. The first dec. should be true, values look the same, but it's still failing.
            //if ((!comment.CreatedBy.Equals(CurrentUser)) || (projectToUpdate.CreatedBy.Id != CurrentUser.Id))
            //{
            //    this.FlashError("You don't have permission to delete that comment.");
            //    return RedirectToAction("show", new { projectToUpdate.Id });
            //}

            try
            {
                //projectToUpdate.Comments.Remove(comment);
                //comment.Project = null;
                _ProjectCommentRepository.Delete(id);

                //_ProjectRepository.InsertOrUpdate(projectToUpdate);
                _ProjectCommentRepository.Save();
                projectToUpdate.LastModifiedOn = DateTime.Now;
                _ProjectRepository.Save();

                this.FlashInfo("The comment was successfully deleted.");

            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem deleting the comment.");
            }

            return RedirectToAction("show", "projects", new { projectToUpdate.Id });
        }

        public ActionResult NewComment(ProjectComment comment, int id)
        {
            try
            {
                // TODO: check for ownership
                comment.CreatedBy = CurrentUser;
                comment.CreatedOn = DateTime.Now;

                var project = _ProjectRepository.Find(id);
                project.AddComment(comment);
                project.LastModifiedOn = DateTime.Now;
                _ProjectRepository.InsertOrUpdate(project);
                _ProjectRepository.Save();
                                
                // send email to other parties
                foreach (var user in project.Users)
                {
                    try
                    {
                        if (user != comment.CreatedBy && user.CanReceiveSystemEmails)
                            new ApplicationUserMailer().NewProjectComment(comment, user.Email).Send();
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                
                this.FlashInfo("Your comment was added successfully.");
                return RedirectToAction("Show", new { id = id });
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem adding your comment: " + ex.Message);
            }

            return RedirectToAction("Show", new { id = id });
        }

        public ActionResult Delete(int id)
        {
            var project = _ProjectRepository.Find(id);
            if (project.CreatedBy == CurrentUser)
            {
                while (project.Products.Any())
                {
                    project.Products.Remove(project.Products.First());
                }

                _ProjectRepository.Delete(id);
                _ProjectRepository.Save();

                this.FlashInfo("The project was successfully deleted.");
                return RedirectToAction("index", "projects");
            }
            this.FlashError("You cannot delete the project.");
            return RedirectToAction("show", "projects", new { id = id });
        }

        public ActionResult Copy(int id)
        {
            try
            {
                var existingProject = _ProjectRepository.Find(id);
                var newProject = new Project();
                newProject.CreatedBy = CurrentUser;
                newProject.CreatedOn = DateTime.Now;
                newProject.LastModifiedOn = DateTime.Now;
                newProject.Description = existingProject.Description;
                newProject.Location = existingProject.Location;

                newProject.Products = new List<ProjectItem>();
                foreach (var item in existingProject.Products)
                {
                    var newItem = new ProjectItem { Product = item.Product, Comment = item.Comment };
                    newProject.AddProduct(newItem);
                }
                
                newProject.ProjectName = "Copy of " + existingProject.ProjectName;
                newProject.PriceDisplayPreference = existingProject.PriceDisplayPreference;
                newProject.AddUser(CurrentUser);

                _ProjectRepository.InsertOrUpdate(newProject);
                _ProjectRepository.Save();

                this.FlashInfo("The project was successfully copied.");
                return RedirectToAction("show", new { id = newProject.Id });
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem copying the project.");
            }
            return RedirectToAction("show", new { id = id });
        }

        public ActionResult Invite(int id)
        {
            var projectToUpdate = _ProjectRepository.Find(id);
            if (projectToUpdate == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            if (projectToUpdate.CreatedBy != CurrentUser)
            {
                this.FlashError("You don't have permission to invite people to the project.");
                return RedirectToAction("show", new { id });
            }

            return View(projectToUpdate);
        }

        [HttpPut, ValidateAntiForgeryToken()]
        public ActionResult Invite(int id, string emails)
        {
            var projectToUpdate = _ProjectRepository.Find(id);
            if (projectToUpdate == null)
            {
                this.FlashError("Oops, that project doesn't seem to exist.");
                return RedirectToAction("index", "projects");
            }

            if (projectToUpdate.CreatedBy != CurrentUser)
            {
                this.FlashError("You don't have permission to invite people to the project.");
                return RedirectToAction("show", new { id });
            }

            try
            {
                string[] emailArray = emails.Split(',');
                var newInvites = new List<ProjectInvitation>();
                foreach (string email in emailArray)
                {
                    var invite = new ProjectInvitation
                    {
                        CreatedBy = CurrentUser,
                        CreatedOn = DateTime.Now,
                        Email = email.Trim().ToLower(),
                        InvitationKey = Guid.NewGuid().ToString().Replace("-", "")
                    };
                    projectToUpdate.AddInvite(invite);
                    newInvites.Add(invite);
                }
                projectToUpdate.LastModifiedOn = DateTime.Now;
                _ProjectRepository.InsertOrUpdate(projectToUpdate);
                _ProjectRepository.Save();

                // send invites
                foreach (var invite in newInvites)
                {
                    new ApplicationUserMailer().ProjectInvitation(invite).Send();	
                }
                
                this.FlashInfo("The invitations have been sent successfully.");
                return RedirectToAction("show", new { id });
            }
            catch (Exception ex)
            {
                this.FlashError("There was an error sending the invitations.");
                
            }

            return View(projectToUpdate);
        }
    }
}
