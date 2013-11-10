using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAPCO.Infrastructure.Domain
{
    public class ProductImage : Entity
    {
        public string Path { get; set; }
        public string DetailPath { get; set; }
        public string ThumbnailPath { get; set; }
        public string SmallThumbnailPath { get; set; }
        public string Description { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
