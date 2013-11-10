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
    public class ProductPriceCodeRepository : RepositoryBase, IProductPriceCodeRepository
    {
        public ProductPriceCodeRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductPriceCode> All
        {
            get { return context.ProductPriceCodes; }
        }

        public IQueryable<ProductPriceCode> AllIncluding(params Expression<Func<ProductPriceCode, object>>[] includeProperties)
        {
            IQueryable<ProductPriceCode> query = context.ProductPriceCodes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductPriceCode Find(int id)
        {
            return context.ProductPriceCodes.Find(id);
        }

        public void InsertOrUpdate(ProductPriceCode productpricecode)
        {
            if (productpricecode.Id == default(int)) {
                // New entity
                context.ProductPriceCodes.Add(productpricecode);
            } else {
                // Existing entity
                context.Entry(productpricecode).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productpricecode = context.ProductPriceCodes.Find(id);
            context.ProductPriceCodes.Remove(productpricecode);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductPriceCodeRepository
    {
        IQueryable<ProductPriceCode> All { get; }
        IQueryable<ProductPriceCode> AllIncluding(params Expression<Func<ProductPriceCode, object>>[] includeProperties);
        ProductPriceCode Find(int id);
        void InsertOrUpdate(ProductPriceCode productpricecode);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}