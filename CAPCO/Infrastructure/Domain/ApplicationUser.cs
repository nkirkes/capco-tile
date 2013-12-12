using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DataAnnotationsExtensions;

namespace CAPCO.Infrastructure.Domain
{
    public class ApplicationUser : Entity
    {
        public ApplicationUser()
        {
            Projects = new List<Project>();
        }

        public string UserName { get; set; }
        
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        /* IsAccountActivated provides a check for whether we've attached a user to a pricing account */
        [NotMapped]
        public bool IsAccountActivated 
        {
            get 
            {
                return !String.IsNullOrWhiteSpace(AccountNumber) && DefaultLocation != null && DiscountCode != null;
            }
        }
        /* Used to reduce number of incoming requests */
        public bool HasRequestedAccount { get; set; }

        [DisplayName("Default Pickup Location")]
        public virtual PickupLocation DefaultLocation { get; set; }
        [DisplayName("Discount Code")]
        public virtual DiscountCode DiscountCode { get; set; }

        public List<Project> Projects { get; set; }

        [NotMapped]
        public string PriceCode
        {
            get
            {
                if (DefaultLocation != null && DiscountCode != null)
                    return String.Format("{0}{1}", DefaultLocation.Code, DiscountCode.Code);
                else
                    return null;
            }
        }

        [NotMapped]
        public string RetailCode
        {
            get
            {
                if (DefaultLocation != null)
                {
                    return String.Format("{0}{1}", DefaultLocation.Code, 3);
                }
                else
                    return "D3";

            }
        }
        
        public string AccountNumber { get; set; }

        [DisplayName("Street Address Line 1")]
        public string StreetAddressLine1 { get; set; }
        [DisplayName("Street Address Line 2")]
        public string StreetAddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        //[Digits]
        public string Zip { get; set; }
        [Required, Email]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }

        public string Status { get; set; }

        [DisplayName("Price Preference")]
        public string PricePreference { get; set; }

        public string ActivationKey { get; set; }
        public bool IsActivated { get; set; }

        public bool CanReceiveMarketingEmails { get; set; }
        public bool CanReceiveSystemEmails { get; set; }

        [NotMapped()]
        public string FullName { get { return String.Format("{0} {1}", FirstName, LastName); } }
        
        public ICollection<Notification> Notifications { get; set; }
        
    }
}