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
    public class RelatedTrimRepository : RepositoryBase, IRelatedTrimRepository
    {
        public RelatedTrimRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<RelatedTrim> All
        {
            get { return context.RelatedTrims; }
        }

        public IQueryable<RelatedTrim> AllIncluding(params Expression<Func<RelatedTrim, object>>[] includeProperties)
        {
            IQueryable<RelatedTrim> query = context.RelatedTrims;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public RelatedTrim Find(int id)
        {
            return context.RelatedTrims.Find(id);
        }

        public void InsertOrUpdate(RelatedTrim relatedtrim)
        {
            if (relatedtrim.Id == default(int)) {
                // New entity
                context.RelatedTrims.Add(relatedtrim);
            } else {
                // Existing entity
                context.Entry(relatedtrim).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var relatedtrim = context.RelatedTrims.Find(id);
            context.RelatedTrims.Remove(relatedtrim);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IRelatedTrimRepository
    {
        IQueryable<RelatedTrim> All { get; }
        IQueryable<RelatedTrim> AllIncluding(params Expression<Func<RelatedTrim, object>>[] includeProperties);
        RelatedTrim Find(int id);
        void InsertOrUpdate(RelatedTrim relatedtrim);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}