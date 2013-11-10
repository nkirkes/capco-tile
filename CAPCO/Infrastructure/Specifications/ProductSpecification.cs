using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAPCO.Infrastructure.Domain;
using CAPCO.Helpers;

namespace CAPCO.Infrastructure.Specifications
{
    public abstract class UserSpecificationBase : Specification<ApplicationUser>
    {
    }

    public class UsersByUserNameSpecification : UserSpecificationBase
    {

        private readonly string _Criteria;
        public UsersByUserNameSpecification(string criteria)
        {
            _Criteria = criteria;
        }

        public override IQueryable<ApplicationUser> SatisfyingElementsFrom(IQueryable<ApplicationUser> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            return from c in candidates
                   where c.UserName.Contains(_Criteria)
                   select c;
        }
    }

    public class UsersByUserNameOrCompanyNameSpecification : UserSpecificationBase
    {

        private readonly string _Criteria;
        public UsersByUserNameOrCompanyNameSpecification(string criteria)
        {
            _Criteria = criteria;
        }

        public override IQueryable<ApplicationUser> SatisfyingElementsFrom(IQueryable<ApplicationUser> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            return from c in candidates
                   where c.UserName.Contains(_Criteria) || c.CompanyName.Contains(_Criteria)
                   select c;
        }
    }

    public abstract class ProductSpecificationBase : Specification<Product>
    {
    }

    public class ProductsByRecentUpdatesSpecification : ProductSpecificationBase
    {

        private readonly DateTime _StartDate;
        public ProductsByRecentUpdatesSpecification(DateTime startDate)
        {
            _StartDate = startDate;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");
            
            return from c in candidates
                   where c.LastModifiedOn >= _StartDate
                   select c;
        }
    }

    public class ProductsByManufacturersSpecification : ProductSpecificationBase
    {

        private readonly IEnumerable<Manufacturer> _Manufacturers;
        public ProductsByManufacturersSpecification(IEnumerable<Manufacturer> manufacturers)
        {
            _Manufacturers = manufacturers;            
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            var results = new List<Product>();
            foreach (var mfg in _Manufacturers)
            {
                results.AddRange(from c in candidates
                                 where c.Section == mfg.Section
                                 select c);
            }

            return results.AsQueryable();
        }
    }

    public class ProductsByGroupSpecification : ProductSpecificationBase
    {
        private readonly int _GroupId;
        
        public ProductsByGroupSpecification(int groupId)
        {
            _GroupId = groupId;            
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            if (_GroupId > 0)
            {
                return from c in candidates
                       where c.Group != null && c.Group.Id == _GroupId
                       select c;
            }
            else 
            {
                return from c in candidates
                       where c.Group == null
                       select c;
            }
        }
    }

    public class ProductsByCategorySpecification : ProductSpecificationBase
    {
        private readonly int _Id;

        public ProductsByCategorySpecification(int id)
        {
            _Id = id;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            if (_Id > 0)
            {
                return from c in candidates
                       where c.Category != null && c.Category.Id == _Id
                       select c;
            }
            else
            {
                return from c in candidates
                       where c.Category == null
                       select c;
            }

        }
    }

    public class ProductsByTypeSpecification : ProductSpecificationBase
    {
        private readonly int _Id;

        public ProductsByTypeSpecification(int id)
        {
            _Id = id;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            if (_Id > 0)
            {
                return from c in candidates
                       where c.Type != null && c.Type.Id == _Id
                       select c;
            }
            else
            {
                return from c in candidates
                       where c.Type == null
                       select c;
            }
        }
    }

    public class ProductsByColorSpecification : ProductSpecificationBase
    {
        private readonly int _Id;

        public ProductsByColorSpecification(int id)
        {
            _Id = id;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");
            if (_Id > 0)
            {
                return from c in candidates
                       where c.Color != null && c.Color.Id == _Id
                       select c;
            }
            else 
            {
                return from c in candidates
                       where c.Color == null || c.Color.Id == 0
                       select c;
            }
        }
    }

    public class ProductsByDescriptionSpecification : ProductSpecificationBase
    {
        private readonly string _Criteria;

        public ProductsByDescriptionSpecification(string criteria)
        {
            _Criteria = criteria;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            return from c in candidates
                   where c.Description.Contains(_Criteria)
                   select c;
        }
    }

    public class ProductsBySeriesSpecification : ProductSpecificationBase
    {
        private readonly string _Criteria;

        public ProductsBySeriesSpecification(string criteria)
        {
            _Criteria = criteria;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            return from c in candidates
                where c.ProductSeries.Name.Contains(_Criteria)
                select c;
        }
    }

    public class ProductsByItemNumberSpecification : ProductSpecificationBase
    {
        private readonly string _Criteria;

        public ProductsByItemNumberSpecification(string criteria)
        {
            _Criteria = criteria;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            //var entities = candidates.ToList().AsQueryable();
            return from c in candidates
                where c.ItemNumber.Contains(_Criteria)
                select c;
        }
    }

    public class ProductsByFinishSpecification : ProductSpecificationBase
    {
        private readonly int _Id;

        public ProductsByFinishSpecification(int id)
        {
            _Id = id;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            if (_Id > 0)
            {
                return from c in candidates
                       where c.Finish != null && c.Finish.Id == _Id
                       select c;
            }
            else
            {
                return from c in candidates
                       where c.Finish == null
                       select c;
            }
        }
    }

    public class ProductsBySizeSpecification : ProductSpecificationBase
    {
        private readonly int _Id;

        public ProductsBySizeSpecification(int id)
        {
            _Id = id;
        }

        public override IQueryable<Product> SatisfyingElementsFrom(IQueryable<Product> candidates)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates", "candidates is null.");

            if (_Id > 0)
            {
                return from c in candidates
                       where c.Size != null && c.Size.Id == _Id
                       select c;
            }
            else
            {
                return from c in candidates
                       where c.Size == null
                       select c;
            }
        }
    }
}