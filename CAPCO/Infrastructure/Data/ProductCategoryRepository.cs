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
    public class ProductCategoryRepository : RepositoryBase, IProductCategoryRepository
    {
        public ProductCategoryRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductCategory> All
        {
            get { return context.ProductCategories; }
        }

        public IQueryable<ProductCategory> AllIncluding(params Expression<Func<ProductCategory, object>>[] includeProperties)
        {
            IQueryable<ProductCategory> query = context.ProductCategories;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductCategory Find(int id)
        {
            return context.ProductCategories.Find(id);
        }

        public void InsertOrUpdate(ProductCategory productcategory)
        {
            if (productcategory.Id == default(int)) {
                // New entity
                context.ProductCategories.Add(productcategory);
            } else {
                // Existing entity
                context.Entry(productcategory).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productcategory = context.ProductCategories.Find(id);
            context.ProductCategories.Remove(productcategory);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductCategoryRepository
    {
        IQueryable<ProductCategory> All { get; }
        IQueryable<ProductCategory> AllIncluding(params Expression<Func<ProductCategory, object>>[] includeProperties);
        ProductCategory Find(int id);
        void InsertOrUpdate(ProductCategory productcategory);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}