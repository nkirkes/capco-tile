using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Data
{ 
    public class ProjectCommentRepository : RepositoryBase, IProjectCommentRepository
    {
        public ProjectCommentRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProjectComment> All
        {
            get { return context.ProjectComments; }
        }

        public IQueryable<ProjectComment> AllIncluding(params Expression<Func<ProjectComment, object>>[] includeProperties)
        {
            IQueryable<ProjectComment> query = context.ProjectComments;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProjectComment Find(int id)
        {
            return context.ProjectComments.Find(id);
        }

        public void InsertOrUpdate(ProjectComment projectcomment)
        {
            if (projectcomment.Id == default(int)) {
                // New entity
                context.ProjectComments.Add(projectcomment);
            } else {
                // Existing entity
                context.Entry(projectcomment).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var projectcomment = context.ProjectComments.Find(id);
            context.ProjectComments.Remove(projectcomment);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProjectCommentRepository
    {
        IQueryable<ProjectComment> All { get; }
        IQueryable<ProjectComment> AllIncluding(params Expression<Func<ProjectComment, object>>[] includeProperties);
        ProjectComment Find(int id);
        void InsertOrUpdate(ProjectComment projectcomment);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}