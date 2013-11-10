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
    public class ProductUnitOfMeasureRepository : RepositoryBase, IProductUnitOfMeasureRepository
    {
        public ProductUnitOfMeasureRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductUnitOfMeasure> All
        {
            get { return context.ProductUnitOfMeasures; }
        }

        public IQueryable<ProductUnitOfMeasure> AllIncluding(params Expression<Func<ProductUnitOfMeasure, object>>[] includeProperties)
        {
            IQueryable<ProductUnitOfMeasure> query = context.ProductUnitOfMeasures;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductUnitOfMeasure Find(int id)
        {
            return context.ProductUnitOfMeasures.Find(id);
        }

        public void InsertOrUpdate(ProductUnitOfMeasure productunitofmeasure)
        {
            if (productunitofmeasure.Id == default(int)) {
                // New entity
                context.ProductUnitOfMeasures.Add(productunitofmeasure);
            } else {
                // Existing entity
                context.Entry(productunitofmeasure).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productunitofmeasure = context.ProductUnitOfMeasures.Find(id);
            context.ProductUnitOfMeasures.Remove(productunitofmeasure);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductUnitOfMeasureRepository
    {
        IQueryable<ProductUnitOfMeasure> All { get; }
        IQueryable<ProductUnitOfMeasure> AllIncluding(params Expression<Func<ProductUnitOfMeasure, object>>[] includeProperties);
        ProductUnitOfMeasure Find(int id);
        void InsertOrUpdate(ProductUnitOfMeasure productunitofmeasure);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}