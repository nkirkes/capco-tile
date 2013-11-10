using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Data
{
    public abstract class RepositoryBase
    {
        protected readonly CAPCOContext context;
        public RepositoryBase(CAPCOContext context)
        {
            this.context = context;
        }

        public void Detach(object entity)
        {
            ((IObjectContextAdapter)context).ObjectContext.Detach(entity);
        }

        public void CleanCollection<TEntity>(int entityId, params string[] collectionNames) where TEntity : Entity
        {
            var entitySetProp = context.GetType().GetProperties().Where(x => x.PropertyType == typeof(DbSet<TEntity>)).FirstOrDefault();

            if (entitySetProp != null)
            {
                var entitySet = entitySetProp.GetValue(context, null) as DbSet<TEntity>;
                if (entitySet != null)
                {
                    var entity = entitySet.FirstOrDefault(x => x.Id == entityId);
                    try
                    {
                        foreach (string propName in collectionNames)
                        {
                            var prop = entity.GetType().GetProperty(propName);
                            if (typeof(ICollection<object>).IsAssignableFrom(prop.PropertyType))
                            {
                                var get = prop.GetGetMethod();
                                if (!get.IsStatic && get.GetParameters().Length == 0)
                                {
                                    var collection = (ICollection<object>)get.Invoke(entity, null);
                                    if (collection != null)
                                        collection.Clear();
                                }
                            }
                        }
                        context.SaveChanges();
                        this.Detach(entity);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}