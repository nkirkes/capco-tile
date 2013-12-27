using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Domain;
using System.Configuration;

namespace System.Web.Mvc
{
    public static class MembershipHelpers
    {
        private static readonly IRepository<Project> _ProjectRepo = DependencyResolver.Current.GetService<IRepository<Project>>();
        private static readonly IRepository<ProductPriceCode> _ProductPriceCodeRepo = DependencyResolver.Current.GetService<IRepository<ProductPriceCode>>();
        private static readonly IRepository<ApplicationUser> _AppUserRepo = DependencyResolver.Current.GetService<IRepository<ApplicationUser>>(); 


        public static ApplicationUser GetMember(this MembershipUser membershipUser)
        {
            if (membershipUser != null)
                return GetMember(membershipUser.UserName);

            return null;
        }

        public static ApplicationUser GetMember(string userName)
        {
            try
            {
                return _AppUserRepo.All.FirstOrDefault(member => member.UserName == userName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public static ApplicationUser GetMember(string userName)
        //{
        //    try
        //    {
        //        if (HttpContext.Current.Session["CurrentMember"] == null)
        //        {
        //            var member = _AppUserRepo.All.FirstOrDefault(x => x.UserName == userName);
        //            HttpContext.Current.Session.Add("CurrentMember", member);
        //        }
        //        return (ApplicationUser) HttpContext.Current.Session["CurrentMember"];
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public static ApplicationUser GetCurrentUser()
        {
            return Membership.GetUser().GetMember();
        }

        public static List<Project> Projects(this ApplicationUser user)
        {
            int expirationPeriodInDays = Convert.ToInt32(ConfigurationManager.AppSettings["ProjectExpirationInDays"]) * -1;
            var expirationDate = DateTime.Today.AddDays(expirationPeriodInDays);
            return _ProjectRepo.All
                .Where(x => x.LastModifiedOn >= expirationDate && x.Users.Contains(user)).OrderByDescending(x => x.LastModifiedOn).ToList();
        }

        public static IQueryable<Project> ArchivedProjects(this ApplicationUser user)
        {
            int expirationPeriodInDays = Convert.ToInt32(ConfigurationManager.AppSettings["ProjectExpirationInDays"]) * -1;
            var expirationDate = DateTime.Today.AddDays(expirationPeriodInDays);
            return _ProjectRepo.AllIncluding(x => x.Users).Where(x => x.LastModifiedOn <= expirationDate && x.Users.Contains(user)).OrderByDescending(x => x.LastModifiedOn);
        }

        public static Decimal ProviderRetail(this Product product, Project project)
        {
            return ProviderRetail(product, project.RetailCode);
        }

        public static Decimal ProviderRetail(this Product product, ApplicationUser user)
        {
            if (!Roles.IsUserInRole(user.UserName, UserRoles.ServiceProviders.ToString()))
                return product.RetailPrice;
            
            return ProviderRetail(product, user.RetailCode);

        }

        public static Decimal ProviderRetail(this Product product, string retailCode)
        {
            try
            {
                var results = _ProductPriceCodeRepo.All.FirstOrDefault(x => x.PriceGroup.Trim() == product.PriceCodeGroup.Trim() && x.PriceCode == retailCode);
                
                return results != null ? results.Price : product.RetailPrice;
            }
            catch (Exception ex)
            {
                return product.RetailPrice;
            }
        }

        public static Decimal ProviderCost(this Product product, Project project)
        {
            return ProviderCost(product, project.PriceCode);
        }

        public static Decimal ProviderCost(this Product product, ApplicationUser user)
        {
            if (!Roles.IsUserInRole(user.UserName, UserRoles.ServiceProviders.ToString()))
                return product.RetailPrice;

            if (user.PriceCode == null)
                return product.RetailPrice;

            return ProviderCost(product, user.PriceCode);

        }

        public static Decimal ProviderCost(this Product product, string priceCode)
        {
            try
            {
                var results = _ProductPriceCodeRepo.All.FirstOrDefault(x => x.PriceGroup.Trim() == product.PriceCodeGroup.Trim() && x.PriceCode == priceCode);
                return results != null ? results.Price : product.RetailPrice;
            }
            catch (Exception ex)
            {
                return product.RetailPrice;
            }
        }
    }
}