using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAPCO.Infrastructure.Domain
{
    public abstract class Specification<T>
    {
        public bool IsSatisfiedBy(T candidate)
        {
            return SatisfyingElementsFrom(new[] { candidate }).Any();
        }

        public IEnumerable<T> SatisfyingElementsFrom(IEnumerable<T> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            return SatisfyingElementsFrom(candidates.AsQueryable());
        }

        public abstract IQueryable<T> SatisfyingElementsFrom(IQueryable<T> candidates);
    }
}