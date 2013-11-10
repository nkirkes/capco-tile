using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;

namespace CAPCO.Infrastructure.Data
{ 
    public class ProductRepository : RepositoryBase, IProductRepository
    {
        public ProductRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<Product> All
        {
            get { return context.Products; }
        }

        public IQueryable<Product> AllIncluding(params Expression<Func<Product, object>>[] includeProperties)
        {
            IQueryable<Product> query = context.Products;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Product Find(int id)
        {
            return context.Products.Find(id);
        }

        public void InsertOrUpdate(Product product)
        {
            if (product.Id == default(int)) {
                // New entity
                context.Products.Add(product);
            } else {
                // Existing entity
                context.Entry(product).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var product = context.Products.Find(id);
            context.Products.Remove(product);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public IQueryable<Product> FindBySpecification(Specification<Product> specification)
        {
            if (specification == null)
                throw new ArgumentNullException("specification", "specification is null");

            var specs = new List<Specification<Product>>();
            specs.Add(specification);
            return FindBySpecification(specs.ToArray());
        }

        public IQueryable<Product> FindBySpecification(params Specification<Product>[] specifications)
        {
            if (specifications == null || specifications.Any(x => x == null))
                throw new ArgumentNullException("specifications", "specifications is null or collection contains a null specification");

            var entities = AllIncluding(p => p.Manufacturer);
            foreach (var specification in specifications)
            {
                entities = specification.SatisfyingElementsFrom(entities);
            }

            return entities.OrderBy(x => x.CreatedOn);

            //, 
            //                   p => p.Group, 
            //                   p => p.Category, 
            //                   p => p.Color, 
            //                   p => p.Finish, 
            //                   p => p.Size, 
            //                   p => p.Type, 
            //                   p => p.ProductBundles, 
            //                   p => p.RelatedProducts, 
            //                   p => p.RelatedSizes, 
            //                   p => p.RelatedTrims, 
            //                   p => p.Status, 
            //                   p => p.UnitOfMeasure, 
            //                   p => p.Usage,
            //                   p => p.Variation,
            //                   p => p.RelatedAccents,
            //                   p => p.ParentProduct
        }
    }

    public interface IProductRepository
    {
        IQueryable<Product> FindBySpecification(Specification<Product> specification);
        IQueryable<Product> FindBySpecification(params Specification<Product>[] specifications);
        IQueryable<Product> All { get; }
        IQueryable<Product> AllIncluding(params Expression<Func<Product, object>>[] includeProperties);
        Product Find(int id);
        void InsertOrUpdate(Product product);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
        
    }
}