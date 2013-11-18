using System.Web.Routing;
using RestfulRouting;
using CAPCO.Controllers;
//using CAPCO.Areas.Admin.Controllers;
using CAPCO.Infrastructure.Domain;
using CAPCO.Areas.Admin.Controllers;

[assembly: WebActivator.PreApplicationStartMethod(typeof(CAPCO.Routes), "Start")]

namespace CAPCO
{
    public class Routes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.DebugRoute("routedebug");
            /* Root Routes */
            map.Root<HomeController>(x => x.Index(""));
            map.Resources<RootController>(x => x.Only("index"));
            map.Path("about/{section}").To<HomeController>(x => x.About(""));
            map.Path("about").To<HomeController>(x => x.About(""));
            map.Path("access").To<HomeController>(x => x.Access());
            map.Path("showroom").To<HomeController>(x => x.Showroom());
            map.Path("locations").To<HomeController>(x => x.Locations());
            map.Path("contact").To<HomeController>(x => x.Contact());
            map.Path("links").To<HomeController>(x => x.Links());
            
            map.Resources<HomeController>(x => {
                x.As("home");
                x.Only("index");
                x.Member(y => y.Get("index"));
                x.Collection(y => y.Get("privacy"));
                x.Collection(y => y.Post("contactrequest"));
            });

            map.Resources<ErrorController>(x =>
            {
                x.As("error");
                x.Only("index");
                x.Collection(y => y.Get("notfound"));
            });


            /* Accounts */
            map.Resources<AccountController>(account =>
            {
                account.As("account");
                account.Only("index");
                account.Collection(x => x.Get("register"));
                account.Collection(x => x.Post("register"));
                account.Collection(x => x.Get("login"));
                account.Collection(x => x.Post("login"));
                account.Collection(x => x.Get("logoff"));
                account.Collection(x => x.Get("changepassword"));
                account.Collection(x => x.Post("changepassword"));
                account.Collection(x => x.Get("changepasswordsuccess"));
                account.Collection(x => x.Get("activate"));
                account.Collection(x => x.Get("resendactivation"));
                account.Collection(x => x.Get("resetpassword"));
                account.Collection(x => x.Post("resetpassword"));
            });

            /* Price Lists */
            map.Resources<PriceListController>(list => {
                list.Only("index", "show");
                list.Collection(x => x.Get("all"));
                list.Collection(x => x.Get("bysection"));
                list.Collection(x => x.Post("bysection"));
                list.Collection(x => x.Get("byupdates"));
                list.Collection(x => x.Post("byupdates"));
                list.Collection(x => x.Get("print"));
                list.Collection(x => x.Get("data"));
                list.Collection(x => x.Get("export"));
            });

            /* Products */
            map.Area<CAPCO.Controllers.ProductsController>("", products => {
                products.Resources<CAPCO.Controllers.ProductsController>(x =>
                {
                    x.Only("index", "show");
                    x.Collection(r => r.Get("search"));
                    x.Collection(r => r.Post("search"));
                    x.Member(r => r.Post("addtoproject"));
                    x.Collection(r => r.Get("slabs"));
                });
            });

            /* Profiles */
            map.Resources<ProfileController>(profile => {
                profile.As("profile");
                profile.Only("index", "show", "edit", "update");
                profile.Collection(x => x.Get("requestaccount"));
                profile.Collection(x => x.Post("requestaccount"));
            });

            /* Projects (aka ProductBundles) */
            map.Resources<ProjectsController>(projects => {
                projects.Member(x => x.Post("newcomment"));
                projects.Member(x => x.Get("delete"));
                projects.Member(x => x.Get("copy"));
                projects.Member(x => x.Get("removeproduct"));
                projects.Member(x => x.Get("deletecomment"));
                projects.Member(x => x.Put("invite"));
                projects.Member(x => x.Get("invite"));
                projects.Collection(x => x.Get("archives"));
                projects.Member(x => x.Get("removemember"));
                projects.Member(x => x.Get("removeinvite"));
                projects.Member(x => x.Post("updateitemcomment"));
            });

            /* Request Account */
            map.Resources<RequestAccountController>(requests => {
                requests.Only("show");
                requests.Collection(x => x.Post("requestaccount"));
            });

            /* Admin */
            map.Area<DashboardController>("admin", admin =>
            {
                admin.Root<DashboardController>(x => x.Index());
                admin.Resources<CAPCO.Areas.Admin.Controllers.ProductsController>(product =>
                {
                    product.Member(x => x.Get("delete"));
                    product.Collection(x => x.Post("search"));
                });
                admin.Resources<ContentController>(content =>
                {
                    content.Member(x => x.Get("delete"));
                    content.Except("new", "create");
                });
                admin.Resources<ContactRequestsController>(content =>
                {
                    content.Member(x => x.Get("delete"));
                    content.Except("new", "create");
                });
                admin.Resources<AccountRequestsController>(content =>
                {
                    content.Member(x => x.Get("delete"));
                    content.Except("new", "create");
                });
                //admin.Resources<PriceCodesController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductPriceCodesController>(pc => {
                    pc.Member(x => x.Get("delete"));
                    pc.Collection(x => x.Post("search"));
                });
                admin.Resources<ProductGroupsController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductFinishesController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductCategoriesController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductTypesController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductColorsController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductSizesController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductStatusController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductSeriesController>(pc => {
                    pc.Member(x => x.Get("delete"));
                    pc.Collection(x => x.Post("search"));
                });
                admin.Resources<ProductVariationsController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductUnitOfMeasuresController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<UsersController>(pc =>
                {
                    pc.Member(x => x.Get("delete"));
                    pc.As("users");
                    pc.Member(x => x.Get("resetpassword"));
                    pc.Member(x => x.Get("unlockuser"));
                    pc.Collection(x => x.Post("search"));
                });
                admin.Resources<PickupLocationsController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<DiscountCodesController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<StoreLocationsController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ManufacturersController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<ProductUsagesController>(pc => { pc.Member(x => x.Get("delete")); });
                admin.Resources<LinksController>(pc => {
                    pc.Member(x => x.Get("delete"));
                    pc.Collection(x => x.Post("reorder"));
                });
                admin.Resources<SliderController>(pc =>
                {
                    pc.Member(x => x.Get("delete"));
                    pc.Collection(x => x.Post("reorder"));
                });
                admin.Resources<ImportController>(pc => {
                    pc.Only("index");
                    pc.As("import");
                    pc.Collection(x => x.Post("products"));
                    pc.Collection(x => x.Post("productpricecodes"));
                    pc.Collection(x => x.Post("relatedproducts"));
                    pc.Collection(x => x.Post("productseries"));
                    //pc.Collection(x => x.Post("customers"));
                });
                admin.Resources<ElmahController>(elmah =>
                {
                    elmah.As("elmah");
                    elmah.Only("index");
                    elmah.Collection(coll => coll.Get("stylesheet"));
                    elmah.Collection(coll => coll.Get("rss"));
                    elmah.Collection(coll => coll.Get("digestrss"));
                    elmah.Collection(coll => coll.Get("about"));
                    elmah.Collection(coll => coll.Get("detail"));
                    elmah.Collection(coll => coll.Get("download"));
                });
            });
        }

        public static void Start()
        {
            var routes = RouteTable.Routes;
            routes.MapRoutes<Routes>();
        }
    }
}