using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DataAnnotationsExtensions;

namespace CAPCO.Infrastructure.Domain
{
    public class ContentSection : Entity
    {
        [DisplayName("Section Name")]
        public string SectionName { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Last Modified On")]
        public DateTime LastModifiedOn { get; set; }
        [DisplayName("Last Modified By")]
        public string LastModifiedBy { get; set; }
    }
}
