using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using CAPCO.Infrastructure.Services;
using CAPCO.Infrastructure.Data;
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
            if (Request.IsLocal) { MiniProfiler.Stop(); }
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

            builder.Register(c => new CAPCOContext()).SingleInstance();
            builder.RegisterGeneric(typeof (Repository<>)).As(typeof (IRepository<>)).InstancePerHttpRequest();
            builder.RegisterType<ApplicationUserService>().As<IApplicationUserService>().InstancePerHttpRequest();
            builder.RegisterType<ContentService>().As<IContentService>().InstancePerHttpRequest();
            
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
            
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Database.SetInitializer<CAPCOContext>(null);
        }

        
    }
}