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
    public class ProductVariationRepository : RepositoryBase, IProductVariationRepository
    {
        public ProductVariationRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProductVariation> All
        {
            get { return context.ProductVariations; }
        }

        public IQueryable<ProductVariation> AllIncluding(params Expression<Func<ProductVariation, object>>[] includeProperties)
        {
            IQueryable<ProductVariation> query = context.ProductVariations;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProductVariation Find(int id)
        {
            return context.ProductVariations.Find(id);
        }

        public void InsertOrUpdate(ProductVariation productvariation)
        {
            if (productvariation.Id == default(int)) {
                // New entity
                context.ProductVariations.Add(productvariation);
            } else {
                // Existing entity
                context.Entry(productvariation).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productvariation = context.ProductVariations.Find(id);
            context.ProductVariations.Remove(productvariation);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProductVariationRepository
    {
        IQueryable<ProductVariation> All { get; }
        IQueryable<ProductVariation> AllIncluding(params Expression<Func<ProductVariation, object>>[] includeProperties);
        ProductVariation Find(int id);
        void InsertOrUpdate(ProductVariation productvariation);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}