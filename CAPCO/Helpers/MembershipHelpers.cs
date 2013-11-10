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
                return DependencyResolver.Current.GetService<IApplicationUserRepository>().All.FirstOrDefault(member => member.UserName == userName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ApplicationUser GetCurrentUser()
        {
            return Membership.GetUser().GetMember();
        }

        public static IList<Project> Projects(this ApplicationUser user)
        {
            try
            {
                var projectRepo = DependencyResolver.Current.GetService<IProjectRepository>();
                int expirationPeriodInDays = Convert.ToInt32(ConfigurationManager.AppSettings["ProjectExpirationInDays"]) * -1;
                var expirationDate = DateTime.Today.AddDays(expirationPeriodInDays);
                var projects = projectRepo.AllIncluding(x => x.Users).Where(x => x.LastModifiedOn >= expirationDate).ToList();
                return projects.Where(x => x.Users.Contains(user)).OrderByDescending(x => x.LastModifiedOn).ToList();
            }
            catch (Exception ex)
            {
                return new List<Project>();
            }
        }

        public static IList<Project> ArchivedProjects(this ApplicationUser user)
        {
            try
            {
                var projectRepo = DependencyResolver.Current.GetService<IProjectRepository>();
                int expirationPeriodInDays = Convert.ToInt32(ConfigurationManager.AppSettings["ProjectExpirationInDays"]) * -1;
                var expirationDate = DateTime.Today.AddDays(expirationPeriodInDays);
                var projects = projectRepo.AllIncluding(x => x.Users).Where(x => x.LastModifiedOn <= expirationDate).ToList();
                return projects.Where(x => x.Users.Contains(user)).OrderByDescending(x => x.LastModifiedOn).ToList();
            }
            catch (Exception ex)
            {
                return new List<Project>();
            }
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
                var repo = DependencyResolver.Current.GetService<IProductPriceCodeRepository>();
                var results = repo.All.FirstOrDefault(x => x.PriceGroup == product.PriceCodeGroup && x.PriceCode == retailCode);
                
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
                var repo = DependencyResolver.Current.GetService<IProductPriceCodeRepository>();
                
                var results = repo.All.FirstOrDefault(x => x.PriceGroup == product.PriceCodeGroup && x.PriceCode == priceCode);// product.PriceCodes().FirstOrDefault(x => x.PriceCode == priceCode).Price;
                return results != null ? results.Price : product.RetailPrice;
            }
            catch (Exception ex)
            {
                return product.RetailPrice;
            }
        }
    }
}