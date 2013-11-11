using System.Web;
using System.Web.Mvc;
using CAPCO.Controllers;

namespace CAPCO.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public abstract class BaseAdminController : ApplicationController
    {

        protected void SetSearchCookie(string action, string criteria, int page)
        {
            // cache the search/paging parts of the model
            HttpCookie searchCookie;
            if (Request.Cookies["SearchCriteria"] != null)
            {
                searchCookie = Request.Cookies["SearchCriteria"];
                searchCookie.Values.Clear();
            }
            else
            {
                searchCookie = new HttpCookie("SearchCriteria");
            }

            searchCookie.Values.Add("Action", action);
            searchCookie.Values.Add("Criteria", criteria);
            searchCookie.Values.Add("Page", page.ToString());
        }

    }
}
