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
    public class ApplicationUserRepository : RepositoryBase, IApplicationUserRepository
    {
        public ApplicationUserRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ApplicationUser> All
        {
            get { return context.ApplicationUsers; }
        }

        public IQueryable<ApplicationUser> AllIncluding(params Expression<Func<ApplicationUser, object>>[] includeProperties)
        {
            IQueryable<ApplicationUser> query = context.ApplicationUsers;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ApplicationUser Find(int id)
        {
            return context.ApplicationUsers.Find(id);
        }

        public void InsertOrUpdate(ApplicationUser applicationuser)
        {
            if (applicationuser.Id == default(int)) {
                // New entity
                context.ApplicationUsers.Add(applicationuser);
            } else {
                // Existing entity
                context.Entry(applicationuser).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var applicationuser = context.ApplicationUsers.Find(id);
            context.ApplicationUsers.Remove(applicationuser);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public IQueryable<ApplicationUser> FindBySpecification(Specification<ApplicationUser> specification)
        {
            if (specification == null)
                throw new ArgumentNullException("specification", "specification is null");

            var specs = new List<Specification<ApplicationUser>>();
            specs.Add(specification);
            return FindBySpecification(specs.ToArray());
        }

        public IQueryable<ApplicationUser> FindBySpecification(params Specification<ApplicationUser>[] specifications)
        {
            if (specifications == null || specifications.Any(x => x == null))
                throw new ArgumentNullException("specifications", "specifications is null or collection contains a null specification");

            var entities = All;
            foreach (var specification in specifications)
            {
                entities = specification.SatisfyingElementsFrom(entities);
            }

            return entities.OrderBy(x => x.UserName);
        }
    }

    public interface IApplicationUserRepository
    {
        IQueryable<ApplicationUser> FindBySpecification(Specification<ApplicationUser> specification);
        IQueryable<ApplicationUser> FindBySpecification(params Specification<ApplicationUser>[] specifications);
        IQueryable<ApplicationUser> All { get; }
        IQueryable<ApplicationUser> AllIncluding(params Expression<Func<ApplicationUser, object>>[] includeProperties);
        ApplicationUser Find(int id);
        void InsertOrUpdate(ApplicationUser applicationuser);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}