using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using CAPCO.Infrastructure.Services;
using CAPCO.Infrastructure.Domain;
using CAPCO.Infrastructure.Data;
using System.Web.Security;
using System.Configuration;
using StackExchange.Profiling;

namespace CAPCO
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            if (Request.IsLocal) { MiniProfiler.Start(); }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }

        static IContainer _container;
        public IContainer Container
        {
            get { return _container; }
        }

        protected void Application_Start()
        {
            MiniProfilerEF.Initialize();
            AreaRegistration.RegisterAllAreas();

            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.Register(c => new CAPCOContext()).InstancePerHttpRequest();
            builder.RegisterType<ApplicationUserRepository>().As<IApplicationUserRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductGroupRepository>().As<IProductGroupRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductTypeRepository>().As<IProductTypeRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductStatusRepository>().As<IProductStatusRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductSeriesRepository>().As<IProductSeriesRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductVariationRepository>().As<IProductVariationRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductUnitOfMeasureRepository>().As<IProductUnitOfMeasureRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductCategoryRepository>().As<IProductCategoryRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductColorRepository>().As<IProductColorRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductSizeRepository>().As<IProductSizeRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductFinishRepository>().As<IProductFinishRepository>().InstancePerHttpRequest();
            builder.RegisterType<ContentSectionRepository>().As<IContentSectionRepository>().InstancePerHttpRequest();
            builder.RegisterType<NotificationRepository>().As<INotificationRepository>().InstancePerHttpRequest();
            builder.RegisterType<PriceCodeRepository>().As<IPriceCodeRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProjectRepository>().As<IProjectRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductRepository>().As<IProductRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProjectCommentRepository>().As<IProjectCommentRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProjectInvitationRepository>().As<IProjectInvitationRepository>().InstancePerHttpRequest();
            builder.RegisterType<ApplicationUserService>().As<IApplicationUserService>().InstancePerHttpRequest();
            builder.RegisterType<ManufacturerRepository>().As<IManufacturerRepository>().InstancePerHttpRequest();
            builder.RegisterType<ContentService>().As<IContentService>().InstancePerHttpRequest();
            builder.RegisterType<PickupLocationRepository>().As<IPickupLocationRepository>().InstancePerHttpRequest();
            builder.RegisterType<DiscountCodeRepository>().As<IDiscountCodeRepository>().InstancePerHttpRequest();
            builder.RegisterType<StoreLocationRepository>().As<IStoreLocationRepository>().InstancePerHttpRequest();
            builder.RegisterType<ContactRequestRepository>().As<IContactRequestRepository>().InstancePerHttpRequest();
            builder.RegisterType<AccountRequestRepository>().As<IAccountRequestRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductUsageRepository>().As<IProductUsageRepository>().InstancePerHttpRequest();
            //builder.RegisterType<ProductCrossReferenceRepository>().As<IProductCrossReferenceRepository>().InstancePerHttpRequest();
            builder.RegisterType<RelatedProductSizeRepository>().As<IRelatedProductSizeRepository>().InstancePerHttpRequest();
            builder.RegisterType<RelatedAccentRepository>().As<IRelatedAccentRepository>().InstancePerHttpRequest();
            builder.RegisterType<RelatedTrimRepository>().As<IRelatedTrimRepository>().InstancePerHttpRequest();
            builder.RegisterType<ProductPriceCodeRepository>().As<IProductPriceCodeRepository>().InstancePerHttpRequest();
            builder.RegisterType<LinkRepository>().As<ILinkRepository>().InstancePerHttpRequest();
            
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            
            
        }

        
    }
}