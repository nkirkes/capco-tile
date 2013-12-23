using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CAPCO.Infrastructure.Domain
{
    public class ProjectItem : Entity
    {
        
        public ProjectItem()
        {
            
        }

        public virtual Project Project { get; set; }
        public virtual Product Product { get; set; }
        public string Comment { get; set; }
    }

    public class Project : Entity
    {

        public Project()
        {
            Users = new List<ApplicationUser>();
            Comments = new List<ProjectComment>();
            Products = new List<ProjectItem>();
            Invitations = new List<ProjectInvitation>();
        }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        
        public virtual ApplicationUser CreatedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string PriceDisplayPreference { get; set; }
        public ICollection<ProjectComment> Comments { get; set; }
        public ICollection<ProjectItem> Products { get; set; }
        public ICollection<ProjectInvitation> Invitations { get; set; }

        public virtual PickupLocation Location { get; set; }
        [NotMapped]
        public int? DiscountCode
        {
            get
            {
                if (CreatedBy != null && CreatedBy.DiscountCode != null)
                    return CreatedBy.DiscountCode.Code;

                return null;
            }
        }
        [NotMapped]
        public string PriceCode
        {
            get
            {
                if ((Location == null && CreatedBy.DefaultLocation == null) || !DiscountCode.HasValue)
                    return null;

                return String.Format("{0}{1}", Location != null ? Location.Code : CreatedBy.DefaultLocation.Code, DiscountCode);
            }
        }

        [NotMapped]
        public string RetailCode
        {
            get
            {
                if ((Location == null && CreatedBy.DefaultLocation == null))
                    return "D3";

                return String.Format("{0}{1}", Location != null ? Location.Code : CreatedBy.DefaultLocation.Code, 3);
            }
        }
        
        public ICollection<ApplicationUser> Users { get; set; }

        //public decimal GetTotalRetail()
        //{
        //    var result = 0m;
        //    if (this.Products != null && this.Products.Any())
        //    {
        //        result = Products.Sum(x => x.Product.ProviderRetail());
        //    }
        //    return result;
        //}

        public void AddUser(ApplicationUser user)
        {
            if (Users == null)
                Users = new List<ApplicationUser>();

            if (!Users.Contains(user))
            {
                Users.Add(user);
            }
        }

        public void AddProduct(ProjectItem item)
        {
            if (Products == null)
                Products = new List<ProjectItem>();

            item.Project = this;
            Products.Add(item);
        }
        public void AddComment(ProjectComment comment)
        {
            if (Comments == null)
                Comments = new List<ProjectComment>();
            Comments.Add(comment);
        }

        public void AddInvite(ProjectInvitation invite)
        {
            if (Invitations == null)
                Invitations = new List<ProjectInvitation>();

            if (!Invitations.Contains(invite))
            {
                Invitations.Add(invite);
            }
        }


    }

    public class ProjectComment : Entity
    {
        public string Text { get; set; }

        [ForeignKey("CreatedBy")]
        public int? CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }

    public class ProjectInvitation : Entity
    {
        public string InvitationKey { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("CreatedBy")]
        public int? CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string Email { get; set; }
    }
}
