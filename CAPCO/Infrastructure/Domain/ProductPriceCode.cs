using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAPCO.Infrastructure.Domain
{
    public class PriceGroup : Entity
    {
        [Required, DisplayName("Price Group")]
        public string GroupName { get; set; }

        public virtual ICollection<ProductPriceCode> PriceCodes { get; set; }
        public ICollection<Product> Products { get; set; }
    }

    public class ProductPriceCode : Entity
    {
        [Required, DisplayName("Price Group")]
        public PriceGroup PriceGroup { get; set; }

        [NotMapped]
        public string GroupName { get; set; }
        
        [Required, DisplayName("Price Code")]
        public string PriceCode { get; set; }
        
        [Required]
        public decimal Price { get; set; }
    }
}
