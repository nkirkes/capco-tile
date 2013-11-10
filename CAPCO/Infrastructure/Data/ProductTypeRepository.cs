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
    public class ProductTypeRepository : RepositoryBase, IProductTypeRepository
    {
        public ProductTypeRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductType> All
        {
            get { return context.ProductTypes; }
        }

        public IQueryable<ProductType> AllIncluding(params Expression<Func<ProductType, object>>[] includeProperties)
        {
            IQueryable<ProductType> query = context.ProductTypes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductType Find(int id)
        {
            return context.ProductTypes.Find(id);
        }

        public void InsertOrUpdate(ProductType producttype)
        {
            if (producttype.Id == default(int)) {
                // New entity
                context.ProductTypes.Add(producttype);
            } else {
                // Existing entity
                context.Entry(producttype).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var producttype = context.ProductTypes.Find(id);
            context.ProductTypes.Remove(producttype);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductTypeRepository
    {
        IQueryable<ProductType> All { get; }
        IQueryable<ProductType> AllIncluding(params Expression<Func<ProductType, object>>[] includeProperties);
        ProductType Find(int id);
        void InsertOrUpdate(ProductType producttype);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}