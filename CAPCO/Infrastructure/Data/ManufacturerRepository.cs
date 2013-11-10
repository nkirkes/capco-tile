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
    public class ManufacturerRepository : RepositoryBase, IManufacturerRepository
    {
        public ManufacturerRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<Manufacturer> All
        {
            get { return context.Manufacturers; }
        }

        public IQueryable<Manufacturer> AllIncluding(params Expression<Func<Manufacturer, object>>[] includeProperties)
        {
            IQueryable<Manufacturer> query = context.Manufacturers;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Manufacturer Find(int id)
        {
            return context.Manufacturers.Find(id);
        }

        public void InsertOrUpdate(Manufacturer manufacturer)
        {
            if (manufacturer.Id == default(int)) {
                // New entity
                context.Manufacturers.Add(manufacturer);
            } else {
                // Existing entity
                context.Entry(manufacturer).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var manufacturer = context.Manufacturers.Find(id);
            context.Manufacturers.Remove(manufacturer);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IManufacturerRepository
    {
        IQueryable<Manufacturer> All { get; }
        IQueryable<Manufacturer> AllIncluding(params Expression<Func<Manufacturer, object>>[] includeProperties);
        Manufacturer Find(int id);
        void InsertOrUpdate(Manufacturer manufacturer);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}