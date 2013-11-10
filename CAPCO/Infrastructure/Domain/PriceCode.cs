using System;
using System.Collections.Generic;

namespace CAPCO.Infrastructure.Domain
{
    public class PriceCode : Entity
    {
        public virtual PickupLocation Location { get; set; }
        public virtual DiscountCode DiscountCode { get; set; }
        public string Code 
        {
            get
            {
                if (Location != null && DiscountCode != null)
                    return String.Format("{0}{1}", Location.Code, DiscountCode.Code);

                return null;
            }
        }
    }
}
