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
    public class StoreLocationRepository : RepositoryBase, IStoreLocationRepository
    {
        public StoreLocationRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<StoreLocation> All
        {
            get { return context.StoreLocations; }
        }

        public IQueryable<StoreLocation> AllIncluding(params Expression<Func<StoreLocation, object>>[] includeProperties)
        {
            IQueryable<StoreLocation> query = context.StoreLocations;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public StoreLocation Find(int id)
        {
            return context.StoreLocations.Find(id);
        }

        public void InsertOrUpdate(StoreLocation storelocation)
        {
            if (storelocation.Id == default(int)) {
                // New entity
                context.StoreLocations.Add(storelocation);
            } else {
                // Existing entity
                context.Entry(storelocation).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var storelocation = context.StoreLocations.Find(id);
            context.StoreLocations.Remove(storelocation);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IStoreLocationRepository
    {
        IQueryable<StoreLocation> All { get; }
        IQueryable<StoreLocation> AllIncluding(params Expression<Func<StoreLocation, object>>[] includeProperties);
        StoreLocation Find(int id);
        void InsertOrUpdate(StoreLocation storelocation);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}