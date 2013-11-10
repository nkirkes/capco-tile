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
    public class NotificationRepository : RepositoryBase, INotificationRepository
    {
        public NotificationRepository(CAPCOContext context) : base(context)
        {
        }

        public IQueryable<Notification> All
        {
            get { return context.Notifications; }
        }

        public IQueryable<Notification> AllIncluding(params Expression<Func<Notification, object>>[] includeProperties)
        {
            IQueryable<Notification> query = context.Notifications;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Notification Find(int id)
        {
            return context.Notifications.Find(id);
        }

        public void InsertOrUpdate(Notification notification)
        {
            if (notification.Id == default(int)) {
                // New entity
                context.Notifications.Add(notification);
            } else {
                // Existing entity
                context.Entry(notification).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var notification = context.Notifications.Find(id);
            context.Notifications.Remove(notification);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface INotificationRepository
    {
        IQueryable<Notification> All { get; }
        IQueryable<Notification> AllIncluding(params Expression<Func<Notification, object>>[] includeProperties);
        Notification Find(int id);
        void InsertOrUpdate(Notification notification);
        void Delete(int id);
        void Save();
		void Detach(object entity);
		void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity;
    }
}