using System;

namespace CAPCO.Infrastructure.Domain
{
    public class StoreLocation : Entity
    {
        public string Name { get; set; }
        public string LocationDetails { get; set; }
        public string Description { get; set; }
        public string GoogleMapEmbedCode { get; set; }
    }
}
