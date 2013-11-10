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
    public class PickupLocationRepository : RepositoryBase, IPickupLocationRepository
    {
        public PickupLocationRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<PickupLocation> All
        {
            get { return context.PickupLocations; }
        }

        public IQueryable<PickupLocation> AllIncluding(params Expression<Func<PickupLocation, object>>[] includeProperties)
        {
            IQueryable<PickupLocation> query = context.PickupLocations;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public PickupLocation Find(int id)
        {
            return context.PickupLocations.Find(id);
        }

        public void InsertOrUpdate(PickupLocation pickuplocation)
        {
            if (pickuplocation.Id == default(int)) {
                // New entity
                context.PickupLocations.Add(pickuplocation);
            } else {
                // Existing entity
                context.Entry(pickuplocation).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var pickuplocation = context.PickupLocations.Find(id);
            context.PickupLocations.Remove(pickuplocation);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IPickupLocationRepository
    {
        IQueryable<PickupLocation> All { get; }
        IQueryable<PickupLocation> AllIncluding(params Expression<Func<PickupLocation, object>>[] includeProperties);
        PickupLocation Find(int id);
        void InsertOrUpdate(PickupLocation pickuplocation);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}