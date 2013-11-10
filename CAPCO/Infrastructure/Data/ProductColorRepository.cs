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
    public class ProductColorRepository : RepositoryBase, IProductColorRepository
    {
        public ProductColorRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductColor> All
        {
            get { return context.ProductColors; }
        }

        public IQueryable<ProductColor> AllIncluding(params Expression<Func<ProductColor, object>>[] includeProperties)
        {
            IQueryable<ProductColor> query = context.ProductColors;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductColor Find(int id)
        {
            return context.ProductColors.Find(id);
        }

        public void InsertOrUpdate(ProductColor productcolor)
        {
            if (productcolor.Id == default(int)) {
                // New entity
                context.ProductColors.Add(productcolor);
            } else {
                // Existing entity
                context.Entry(productcolor).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productcolor = context.ProductColors.Find(id);
            context.ProductColors.Remove(productcolor);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductColorRepository
    {
        IQueryable<ProductColor> All { get; }
        IQueryable<ProductColor> AllIncluding(params Expression<Func<ProductColor, object>>[] includeProperties);
        ProductColor Find(int id);
        void InsertOrUpdate(ProductColor productcolor);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}