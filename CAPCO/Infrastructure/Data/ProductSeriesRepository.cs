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
    public class ProductSeriesRepository : RepositoryBase, IProductSeriesRepository
    {
        public ProductSeriesRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductSeries> All
        {
            get { return context.ProductSeries; }
        }

        public IQueryable<ProductSeries> AllIncluding(params Expression<Func<ProductSeries, object>>[] includeProperties)
        {
            IQueryable<ProductSeries> query = context.ProductSeries;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductSeries Find(int id)
        {
            return context.ProductSeries.Find(id);
        }

        public void InsertOrUpdate(ProductSeries productseries)
        {
            if (productseries.Id == default(int)) {
                // New entity
                context.ProductSeries.Add(productseries);
            } else {
                // Existing entity
                context.Entry(productseries).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productseries = context.ProductSeries.Find(id);
            context.ProductSeries.Remove(productseries);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductSeriesRepository
    {
        IQueryable<ProductSeries> All { get; }
        IQueryable<ProductSeries> AllIncluding(params Expression<Func<ProductSeries, object>>[] includeProperties);
        ProductSeries Find(int id);
        void InsertOrUpdate(ProductSeries productseries);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}