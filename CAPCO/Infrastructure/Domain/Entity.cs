using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CAPCO.Infrastructure.Domain
{
    public abstract class Entity : EntityWithTypedId<int>
    {

    }

    public abstract class EntityWithTypedId<IdT>
    {
        [Key]
        public virtual IdT Id { get; set; }
        
        public virtual bool IsTransient()
        {
            return Id == null || Id.Equals(default(IdT));
        }

        public override bool Equals(object obj)
        {
            EntityWithTypedId<IdT> compareTo = obj as EntityWithTypedId<IdT>;

            if (ReferenceEquals(this, compareTo))
                return true;

            if (compareTo == null || !GetType().Equals(compareTo.GetTypeUnproxied()))
                return false;

            if (HasSameNonDefaultIdAs(compareTo))
                return true;

            return IsTransient() && compareTo.IsTransient();
        }

        public virtual Type GetTypeUnproxied()
        {
            return GetType();
        }

        public override int GetHashCode()
        {
            if (cachedHashcode.HasValue)
                return cachedHashcode.Value;

            if (IsTransient())
            {
                cachedHashcode = base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    int hashCode = GetType().GetHashCode();
                    cachedHashcode = (hashCode * HASH_MULTIPLIER) ^ Id.GetHashCode();
                }
            }

            return cachedHashcode.Value;
        }

        private bool HasSameNonDefaultIdAs(EntityWithTypedId<IdT> compareTo)
        {
            return !IsTransient() &&
                  !compareTo.IsTransient() &&
                  Id.Equals(compareTo.Id);
        }

        private int? cachedHashcode;
        private const int HASH_MULTIPLIER = 31;

        public virtual T As<T>() where T : Entity
        {
            return this as T;
        }

    }
}
