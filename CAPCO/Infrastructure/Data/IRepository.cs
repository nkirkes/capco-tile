using System;
using System.Linq;
using System.Linq.Expressions;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Data
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> FindBySpecification(Specification<TEntity> specification);
        IQueryable<TEntity> FindBySpecification(params Specification<TEntity>[] specifications);
        IQueryable<TEntity> All { get; }
        IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity Find(int id);
        bool InsertOrUpdate(TEntity entity);
        void Delete(int id);
        void Save();
        void Detach(object entity);
    }
}