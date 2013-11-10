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
    public class ProjectRepository : RepositoryBase, IProjectRepository
    {
        public ProjectRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<Project> All
        {
            get { return context.ProductBundles; }
        }

        public IQueryable<Project> AllIncluding(params Expression<Func<Project, object>>[] includeProperties)
        {
            IQueryable<Project> query = context.ProductBundles;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Project Find(int id)
        {
            return context.ProductBundles.Find(id);
        }

        public void InsertOrUpdate(Project productbundle)
        {
            if (productbundle.Id == default(int)) {
                // New entity
                context.ProductBundles.Add(productbundle);
            } else {
                // Existing entity
                context.Entry(productbundle).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var productbundle = context.ProductBundles.Find(id);
            context.ProductBundles.Remove(productbundle);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProjectRepository
    {
        IQueryable<Project> All { get; }
        IQueryable<Project> AllIncluding(params Expression<Func<Project, object>>[] includeProperties);
        Project Find(int id);
        void InsertOrUpdate(Project productbundle);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}