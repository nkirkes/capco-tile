using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly CAPCOContext _CapcoContext;
        public Repository(CAPCOContext context)
        {
            _CapcoContext = context;
        }

        public void Detach(object entity)
        {
            ((IObjectContextAdapter)_CapcoContext).ObjectContext.Detach(entity);
        }

        public IQueryable<TEntity> All
        {
            get { return _CapcoContext.Set<TEntity>(); }
        }

        public IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = All;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public TEntity Find(int id)
        {
            return _CapcoContext.Set<TEntity>().Find(id);
        }

        public bool InsertOrUpdate(TEntity entity)
        {
            try
            {
                _CapcoContext.Set<TEntity>().AddOrUpdate<TEntity>(entity);
            }
            catch
            {
                //TODO: Log exception
                return false;
            }
            return true;
        }

        public void Delete(int id)
        {
            TEntity entity = Find(id);
            _CapcoContext.Set<TEntity>().Remove(entity);
        }

        public void Save()
        {
            _CapcoContext.SaveChanges();
        }

        public IQueryable<TEntity> FindBySpecification(Specification<TEntity> specification)
        {
            if (specification == null)
                throw new ArgumentNullException("specification", "specification is null");

            var specs = new List<Specification<TEntity>> {specification};
            return FindBySpecification(specs.ToArray());
        }

        public IQueryable<TEntity> FindBySpecification(params Specification<TEntity>[] specifications)
        {
            if (specifications == null || specifications.Any(x => x == null))
                throw new ArgumentNullException("specifications", "specifications is null or collection contains a null specification");

            return specifications.Aggregate(All, (current, specification) => specification.SatisfyingElementsFrom(current));
        }
    }
}