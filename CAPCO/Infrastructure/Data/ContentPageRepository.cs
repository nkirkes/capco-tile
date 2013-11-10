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
    public class ContentPageRepository : RepositoryBase, IContentPageRepository
    {
        public ContentPageRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ContentPage> All
        {
            get { return context.ContentPages; }
        }

        public IQueryable<ContentPage> AllIncluding(params Expression<Func<ContentPage, object>>[] includeProperties)
        {
            IQueryable<ContentPage> query = context.ContentPages;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ContentPage Find(int id)
        {
            return context.ContentPages.Find(id);
        }

        public void InsertOrUpdate(ContentPage contentpage)
        {
            if (contentpage.Id == default(int)) {
                // New entity
                context.ContentPages.Add(contentpage);
            } else {
                // Existing entity
                context.Entry(contentpage).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var contentpage = context.ContentPages.Find(id);
            context.ContentPages.Remove(contentpage);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IContentPageRepository
    {
        IQueryable<ContentPage> All { get; }
        IQueryable<ContentPage> AllIncluding(params Expression<Func<ContentPage, object>>[] includeProperties);
        ContentPage Find(int id);
        void InsertOrUpdate(ContentPage contentpage);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}