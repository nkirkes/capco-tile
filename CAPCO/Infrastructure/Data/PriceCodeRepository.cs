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
    public class PriceCodeRepository : RepositoryBase, IPriceCodeRepository
    {
        public PriceCodeRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<PriceCode> All
        {
            get { return context.PriceCodes; }
        }

        public IQueryable<PriceCode> AllIncluding(params Expression<Func<PriceCode, object>>[] includeProperties)
        {
            IQueryable<PriceCode> query = context.PriceCodes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public PriceCode Find(int id)
        {
            return context.PriceCodes.Find(id);
        }

        public void InsertOrUpdate(PriceCode pricecode)
        {
            if (pricecode.Id == default(int)) {
                // New entity
                context.PriceCodes.Add(pricecode);
            } else {
                // Existing entity
                context.Entry(pricecode).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var pricecode = context.PriceCodes.Find(id);
            context.PriceCodes.Remove(pricecode);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IPriceCodeRepository
    {
        IQueryable<PriceCode> All { get; }
        IQueryable<PriceCode> AllIncluding(params Expression<Func<PriceCode, object>>[] includeProperties);
        PriceCode Find(int id);
        void InsertOrUpdate(PriceCode pricecode);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}