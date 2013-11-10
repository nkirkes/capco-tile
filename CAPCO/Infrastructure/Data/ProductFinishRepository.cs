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
    public class ProductFinishRepository : RepositoryBase, IProductFinishRepository
    {
        public ProductFinishRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductFinish> All
        {
            get { return context.ProductFinishes; }
        }

        public IQueryable<ProductFinish> AllIncluding(params Expression<Func<ProductFinish, object>>[] includeProperties)
        {
            IQueryable<ProductFinish> query = context.ProductFinishes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductFinish Find(int id)
        {
            return context.ProductFinishes.Find(id);
        }

        public void InsertOrUpdate(ProductFinish productfinish)
        {
            if (productfinish.Id == default(int)) {
                // New entity
                context.ProductFinishes.Add(productfinish);
            } else {
                // Existing entity
                context.Entry(productfinish).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productfinish = context.ProductFinishes.Find(id);
            context.ProductFinishes.Remove(productfinish);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductFinishRepository
    {
        IQueryable<ProductFinish> All { get; }
        IQueryable<ProductFinish> AllIncluding(params Expression<Func<ProductFinish, object>>[] includeProperties);
        ProductFinish Find(int id);
        void InsertOrUpdate(ProductFinish productfinish);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}