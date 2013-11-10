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
    public class ProductStatusRepository : RepositoryBase, IProductStatusRepository
    {
        public ProductStatusRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductStatus> All
        {
            get { return context.ProductStatus; }
        }

        public IQueryable<ProductStatus> AllIncluding(params Expression<Func<ProductStatus, object>>[] includeProperties)
        {
            IQueryable<ProductStatus> query = context.ProductStatus;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductStatus Find(int id)
        {
            return context.ProductStatus.Find(id);
        }

        public void InsertOrUpdate(ProductStatus productstatus)
        {
            if (productstatus.Id == default(int)) {
                // New entity
                context.ProductStatus.Add(productstatus);
            } else {
                // Existing entity
                context.Entry(productstatus).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productstatus = context.ProductStatus.Find(id);
            context.ProductStatus.Remove(productstatus);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductStatusRepository
    {
        IQueryable<ProductStatus> All { get; }
        IQueryable<ProductStatus> AllIncluding(params Expression<Func<ProductStatus, object>>[] includeProperties);
        ProductStatus Find(int id);
        void InsertOrUpdate(ProductStatus productstatus);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}