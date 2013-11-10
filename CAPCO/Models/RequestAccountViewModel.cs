using System;
using CAPCO.Infrastructure.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DataAnnotationsExtensions;

namespace CAPCO.Models
{
    public class RequestAccountViewModel
    {
        public ApplicationUser CurrentUser { get; set; }
        public string StreetAddressLine2 { get; set; }
        [Required, DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [Required, DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required, DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        public string StreetAddressLine1 { get; set; }
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
        public string AccountNumber { get; set; }
    }
}
