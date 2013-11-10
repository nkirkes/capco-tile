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
    public class ProductSizeRepository : RepositoryBase, IProductSizeRepository
    {
        public ProductSizeRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductSize> All
        {
            get { return context.ProductSizes; }
        }

        public IQueryable<ProductSize> AllIncluding(params Expression<Func<ProductSize, object>>[] includeProperties)
        {
            IQueryable<ProductSize> query = context.ProductSizes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductSize Find(int id)
        {
            return context.ProductSizes.Find(id);
        }

        public void InsertOrUpdate(ProductSize productsize)
        {
            if (productsize.Id == default(int)) {
                // New entity
                context.ProductSizes.Add(productsize);
            } else {
                // Existing entity
                context.Entry(productsize).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productsize = context.ProductSizes.Find(id);
            context.ProductSizes.Remove(productsize);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductSizeRepository
    {
        IQueryable<ProductSize> All { get; }
        IQueryable<ProductSize> AllIncluding(params Expression<Func<ProductSize, object>>[] includeProperties);
        ProductSize Find(int id);
        void InsertOrUpdate(ProductSize productsize);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}