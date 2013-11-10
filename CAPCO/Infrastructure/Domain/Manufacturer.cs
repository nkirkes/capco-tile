using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CAPCO.Infrastructure.Domain
{
    public class Manufacturer : Entity
    {
        public Manufacturer()
        {
        
        }
        public string Name { get; set; }
        [DisplayName("Section / Manufacturer Code")]
        public string Section { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
