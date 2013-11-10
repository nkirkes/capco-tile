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
    public class ProjectInvitationRepository : RepositoryBase, IProjectInvitationRepository
    {
        public ProjectInvitationRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ProjectInvitation> All
        {
            get { return context.ProjectInvitations; }
        }

        public IQueryable<ProjectInvitation> AllIncluding(params Expression<Func<ProjectInvitation, object>>[] includeProperties)
        {
            IQueryable<ProjectInvitation> query = context.ProjectInvitations;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProjectInvitation Find(int id)
        {
            return context.ProjectInvitations.Find(id);
        }

        public void InsertOrUpdate(ProjectInvitation projectinvitation)
        {
            if (projectinvitation.Id == default(int)) {
                // New entity
                context.ProjectInvitations.Add(projectinvitation);
            } else {
                // Existing entity
                context.Entry(projectinvitation).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var projectinvitation = context.ProjectInvitations.Find(id);
            context.ProjectInvitations.Remove(projectinvitation);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IProjectInvitationRepository
    {
        IQueryable<ProjectInvitation> All { get; }
        IQueryable<ProjectInvitation> AllIncluding(params Expression<Func<ProjectInvitation, object>>[] includeProperties);
        ProjectInvitation Find(int id);
        void InsertOrUpdate(ProjectInvitation projectinvitation);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}