using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CAPCO.Infrastructure.Domain
{
    public class ProductPriceCode : Entity
    {
        [Required, DisplayName("Price Group")]
        public string PriceGroup { get; set; }
        [Required, DisplayName("Price Code")]
        public string PriceCode { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
