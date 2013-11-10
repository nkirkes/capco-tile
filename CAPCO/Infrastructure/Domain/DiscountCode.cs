using System;

namespace CAPCO.Infrastructure.Domain
{
    public class DiscountCode : Entity
    {
        public string Name { get; set; }
        public int Code { get; set; }
    }
}
