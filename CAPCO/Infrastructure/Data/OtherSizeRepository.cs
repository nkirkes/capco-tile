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
    public class RelatedProductSizeRepository : RepositoryBase, IRelatedProductSizeRepository
    {
        public RelatedProductSizeRepository(CAPCOContext context)
            : base(context)
        {
        }

        public IQueryable<RelatedProductSize> All
        {
            get { return context.OtherSizes; }
        }

        public IQueryable<RelatedProductSize> AllIncluding(params Expression<Func<RelatedProductSize, object>>[] includeProperties)
        {
            IQueryable<RelatedProductSize> query = context.OtherSizes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public RelatedProductSize Find(int id)
        {
            return context.OtherSizes.Find(id);
        }

        public void InsertOrUpdate(RelatedProductSize othersize)
        {
            if (othersize.Id == default(int)) {
                // New entity
                context.OtherSizes.Add(othersize);
            } else {
                // Existing entity
                context.Entry(othersize).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var othersize = context.OtherSizes.Find(id);
            context.OtherSizes.Remove(othersize);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IRelatedProductSizeRepository
    {
        IQueryable<RelatedProductSize> All { get; }
        IQueryable<RelatedProductSize> AllIncluding(params Expression<Func<RelatedProductSize, object>>[] includeProperties);
        RelatedProductSize Find(int id);
        void InsertOrUpdate(RelatedProductSize othersize);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}