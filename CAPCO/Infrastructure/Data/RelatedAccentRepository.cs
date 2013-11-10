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
    public class RelatedAccentRepository : RepositoryBase, IRelatedAccentRepository
    {
        public RelatedAccentRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<RelatedAccent> All
        {
            get { return context.RelatedAccents; }
        }

        public IQueryable<RelatedAccent> AllIncluding(params Expression<Func<RelatedAccent, object>>[] includeProperties)
        {
            IQueryable<RelatedAccent> query = context.RelatedAccents;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public RelatedAccent Find(int id)
        {
            return context.RelatedAccents.Find(id);
        }

        public void InsertOrUpdate(RelatedAccent relatedaccent)
        {
            if (relatedaccent.Id == default(int)) {
                // New entity
                context.RelatedAccents.Add(relatedaccent);
            } else {
                // Existing entity
                context.Entry(relatedaccent).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var relatedaccent = context.RelatedAccents.Find(id);
            context.RelatedAccents.Remove(relatedaccent);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IRelatedAccentRepository
    {
        IQueryable<RelatedAccent> All { get; }
        IQueryable<RelatedAccent> AllIncluding(params Expression<Func<RelatedAccent, object>>[] includeProperties);
        RelatedAccent Find(int id);
        void InsertOrUpdate(RelatedAccent relatedaccent);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}