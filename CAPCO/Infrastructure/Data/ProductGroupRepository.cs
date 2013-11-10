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
    public class ProductGroupRepository : RepositoryBase, IProductGroupRepository
    {
        public ProductGroupRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductGroup> All
        {
            get { return context.ProductGroups; }
        }

        public IQueryable<ProductGroup> AllIncluding(params Expression<Func<ProductGroup, object>>[] includeProperties)
        {
            IQueryable<ProductGroup> query = context.ProductGroups;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductGroup Find(int id)
        {
            return context.ProductGroups.Find(id);
        }

        public void InsertOrUpdate(ProductGroup productgroup)
        {
            if (productgroup.Id == default(int)) {
                // New entity
                context.ProductGroups.Add(productgroup);
            } else {
                // Existing entity
                context.Entry(productgroup).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productgroup = context.ProductGroups.Find(id);
            context.ProductGroups.Remove(productgroup);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductGroupRepository
    {
        IQueryable<ProductGroup> All { get; }
        IQueryable<ProductGroup> AllIncluding(params Expression<Func<ProductGroup, object>>[] includeProperties);
        ProductGroup Find(int id);
        void InsertOrUpdate(ProductGroup productgroup);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}