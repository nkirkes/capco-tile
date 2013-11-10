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
    public class ContentSectionRepository : RepositoryBase, IContentSectionRepository
    {
        public ContentSectionRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ContentSection> All
        {
            get { return context.ContentSections; }
        }

        public IQueryable<ContentSection> AllIncluding(params Expression<Func<ContentSection, object>>[] includeProperties)
        {
            IQueryable<ContentSection> query = context.ContentSections;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ContentSection Find(int id)
        {
            return context.ContentSections.Find(id);
        }

        public void InsertOrUpdate(ContentSection contentsection)
        {
            if (contentsection.Id == default(int)) {
                // New entity
                context.ContentSections.Add(contentsection);
            } else {
                // Existing entity
                context.Entry(contentsection).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var contentsection = context.ContentSections.Find(id);
            context.ContentSections.Remove(contentsection);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IContentSectionRepository
    {
        IQueryable<ContentSection> All { get; }
        IQueryable<ContentSection> AllIncluding(params Expression<Func<ContentSection, object>>[] includeProperties);
        ContentSection Find(int id);
        void InsertOrUpdate(ContentSection contentsection);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}