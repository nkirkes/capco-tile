using System;

namespace CAPCO.Infrastructure.Domain
{
    public abstract class RelatedProduct : Entity
    {
        public virtual Product Product { get; set; }
    }

    public class RelatedProductSize : RelatedProduct
    {
        
    }

    public class RelatedAccent : RelatedProduct
    {

    }

    public class RelatedTrim : RelatedProduct
    {

    }

    public class RelatedFinish : RelatedProduct
    {


    }

    //public class ProductCrossReference : Entity
    //{
    //    public Product Parent { get; set; }
    //    public Product Child { get; set; }
    //    public int? TypeCode { get; set; }
    //}
}
