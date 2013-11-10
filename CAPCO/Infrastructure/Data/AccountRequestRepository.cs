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
    public class AccountRequestRepository : RepositoryBase, IAccountRequestRepository
    {
        public AccountRequestRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<AccountRequest> All
        {
            get { return context.AccountRequests; }
        }

        public IQueryable<AccountRequest> AllIncluding(params Expression<Func<AccountRequest, object>>[] includeProperties)
        {
            IQueryable<AccountRequest> query = context.AccountRequests;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public AccountRequest Find(int id)
        {
            return context.AccountRequests.Find(id);
        }

        public void InsertOrUpdate(AccountRequest accountrequest)
        {
            if (accountrequest.Id == default(int)) {
                // New entity
                context.AccountRequests.Add(accountrequest);
            } else {
                // Existing entity
                context.Entry(accountrequest).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var accountrequest = context.AccountRequests.Find(id);
            context.AccountRequests.Remove(accountrequest);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IAccountRequestRepository
    {
        IQueryable<AccountRequest> All { get; }
        IQueryable<AccountRequest> AllIncluding(params Expression<Func<AccountRequest, object>>[] includeProperties);
        AccountRequest Find(int id);
        void InsertOrUpdate(AccountRequest accountrequest);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}