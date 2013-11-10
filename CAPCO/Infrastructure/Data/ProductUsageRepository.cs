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
    public class ProductUsageRepository : RepositoryBase, IProductUsageRepository
    {
        public ProductUsageRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductUsage> All
        {
            get { return context.ProductUsages; }
        }

        public IQueryable<ProductUsage> AllIncluding(params Expression<Func<ProductUsage, object>>[] includeProperties)
        {
            IQueryable<ProductUsage> query = context.ProductUsages;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductUsage Find(int id)
        {
            return context.ProductUsages.Find(id);
        }

        public void InsertOrUpdate(ProductUsage productusage)
        {
            if (productusage.Id == default(int)) {
                // New entity
                context.ProductUsages.Add(productusage);
            } else {
                // Existing entity
                context.Entry(productusage).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productusage = context.ProductUsages.Find(id);
            context.ProductUsages.Remove(productusage);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductUsageRepository
    {
        IQueryable<ProductUsage> All { get; }
        IQueryable<ProductUsage> AllIncluding(params Expression<Func<ProductUsage, object>>[] includeProperties);
        ProductUsage Find(int id);
        void InsertOrUpdate(ProductUsage productusage);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}