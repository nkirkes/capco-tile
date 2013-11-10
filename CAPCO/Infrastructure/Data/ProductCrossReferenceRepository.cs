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
    public class ProductCrossReferenceRepository : RepositoryBase, IProductCrossReferenceRepository
    {
        public ProductCrossReferenceRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductCrossReference> All
        {
            get { return context.ProductCrossReferences; }
        }

        public IQueryable<ProductCrossReference> AllIncluding(params Expression<Func<ProductCrossReference, object>>[] includeProperties)
        {
            IQueryable<ProductCrossReference> query = context.ProductCrossReferences;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductCrossReference Find(int id)
        {
            return context.ProductCrossReferences.Find(id);
        }

        public void InsertOrUpdate(ProductCrossReference productcrossreference)
        {
            if (productcrossreference.Id == default(int)) {
                // New entity
                context.ProductCrossReferences.Add(productcrossreference);
            } else {
                // Existing entity
                context.Entry(productcrossreference).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productcrossreference = context.ProductCrossReferences.Find(id);
            context.ProductCrossReferences.Remove(productcrossreference);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductCrossReferenceRepository
    {
        IQueryable<ProductCrossReference> All { get; }
        IQueryable<ProductCrossReference> AllIncluding(params Expression<Func<ProductCrossReference, object>>[] includeProperties);
        ProductCrossReference Find(int id);
        void InsertOrUpdate(ProductCrossReference productcrossreference);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}