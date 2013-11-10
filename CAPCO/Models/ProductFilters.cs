using System;
using CAPCO.Infrastructure.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DataAnnotationsExtensions;

namespace CAPCO.Models
{
    public class ProductFilters
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public int? ColorId { get; set; }
        public string ColorName { get; set; }
        public int? SizeId { get; set; }
        public string SizeName { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? FinishId { get; set; }
        public string FinishName { get; set; }
        public string Series { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
    }
}
