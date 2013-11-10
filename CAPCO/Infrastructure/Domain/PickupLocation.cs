using System;
using System.Collections.Generic;

namespace CAPCO.Infrastructure.Domain
{
    public class PickupLocation : Entity
    {
        public PickupLocation()
        {
            Users = new List<ApplicationUser>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        
    }
}
