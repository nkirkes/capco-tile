using System;
using System.Collections.Generic;

namespace CAPCO.Infrastructure.Domain
{
    public abstract class TypedProductProperty<T> : Entity 
    {
        public string Name { get; set; }
        public T Code { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }

    public abstract class ProductProperty : TypedProductProperty<int>
    {
        
    }

    public class ProductSeries : ProductProperty
    {
        
    }

    public class ProductUsage : ProductProperty
    {

    }

    public class ProductStatus : TypedProductProperty<string>
    {
        
    }

    public class ProductUnitOfMeasure : TypedProductProperty<string>
    {

    }

    public class ProductVariation : ProductProperty
    {

    }

    public class ProductGroup : ProductProperty
    {
        
    }

    public class ProductCategory : ProductProperty
    {
        
    }

    public class ProductType : ProductProperty
    {
        
    }

    public class ProductColor : ProductProperty
    {
        
    }

    public class ProductSize : ProductProperty
    {
        
    }

    public class ProductFinish : ProductProperty
    {
        
    }
}
