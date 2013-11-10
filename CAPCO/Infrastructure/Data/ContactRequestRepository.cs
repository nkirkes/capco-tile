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
    public class ContactRequestRepository : RepositoryBase, IContactRequestRepository
    {
        public ContactRequestRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<ContactRequest> All
        {
            get { return context.ContactRequests; }
        }

        public IQueryable<ContactRequest> AllIncluding(params Expression<Func<ContactRequest, object>>[] includeProperties)
        {
            IQueryable<ContactRequest> query = context.ContactRequests;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ContactRequest Find(int id)
        {
            return context.ContactRequests.Find(id);
        }

        public void InsertOrUpdate(ContactRequest contactrequest)
        {
            if (contactrequest.Id == default(int)) {
                // New entity
                context.ContactRequests.Add(contactrequest);
            } else {
                // Existing entity
                context.Entry(contactrequest).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var contactrequest = context.ContactRequests.Find(id);
            context.ContactRequests.Remove(contactrequest);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IContactRequestRepository
    {
        IQueryable<ContactRequest> All { get; }
        IQueryable<ContactRequest> AllIncluding(params Expression<Func<ContactRequest, object>>[] includeProperties);
        ContactRequest Find(int id);
        void InsertOrUpdate(ContactRequest contactrequest);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}