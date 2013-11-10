using System;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace CAPCO.Infrastructure.Domain
{
    public class Link : Entity
    {
        [Required, Url]
        public string Url { get; set; }
        [Required]
        public string Label { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}
