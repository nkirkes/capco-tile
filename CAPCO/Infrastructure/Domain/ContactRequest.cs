using System;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace CAPCO.Infrastructure.Domain
{
    public class ContactRequest : Entity
    {
        [Required]
        public string Name { get; set; }
        [Required, Email]
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsArchived { get; set; }
    }
}
