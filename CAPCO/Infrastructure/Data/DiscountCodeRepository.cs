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
    public class DiscountCodeRepository : RepositoryBase, IDiscountCodeRepository
    {
        public DiscountCodeRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<DiscountCode> All
        {
            get { return context.DiscountCodes; }
        }

        public IQueryable<DiscountCode> AllIncluding(params Expression<Func<DiscountCode, object>>[] includeProperties)
        {
            IQueryable<DiscountCode> query = context.DiscountCodes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public DiscountCode Find(int id)
        {
            return context.DiscountCodes.Find(id);
        }

        public void InsertOrUpdate(DiscountCode discountcode)
        {
            if (discountcode.Id == default(int)) {
                // New entity
                context.DiscountCodes.Add(discountcode);
            } else {
                // Existing entity
                context.Entry(discountcode).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var discountcode = context.DiscountCodes.Find(id);
            context.DiscountCodes.Remove(discountcode);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IDiscountCodeRepository
    {
        IQueryable<DiscountCode> All { get; }
        IQueryable<DiscountCode> AllIncluding(params Expression<Func<DiscountCode, object>>[] includeProperties);
        DiscountCode Find(int id);
        void InsertOrUpdate(DiscountCode discountcode);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}