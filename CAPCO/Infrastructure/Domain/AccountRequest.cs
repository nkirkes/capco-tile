using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace CAPCO.Infrastructure.Domain
{
    public class AccountRequest : Entity
    {
        public virtual ApplicationUser User { get; set; }
        [Required, DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [Required, DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required, DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required, DisplayName("Street Address")]
        public string StreetAddressLine1 { get; set; }
        [DisplayName("Street Address")]
        public string StreetAddressLine2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required, DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }
        [Required, Email]
        public string Email { get; set; }
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        [DisplayName("Has Been Verified?")]
        public bool HasBeenVerified { get; set; }
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }
        public string Notes { get; set; }
    }
}
