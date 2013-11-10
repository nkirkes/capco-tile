using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAPCO.Infrastructure.Domain
{
    public class Notification : Entity
    {
        [ForeignKey("Recipient")]
        public int RecipientId { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
