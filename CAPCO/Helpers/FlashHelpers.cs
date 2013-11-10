using System;
using System.Text;

namespace System.Web.Mvc
{
    public static class FlashHelpers
    {

        public static void FlashInfo(this Controller controller, string message)
        {
            controller.TempData["info"] = message;
        }
        public static void FlashWarning(this Controller controller, string message)
        {
            controller.TempData["warning"] = message;
        }
        public static void FlashError(this Controller controller, string message)
        {
            controller.TempData["error"] = message;
        }

        public static MvcHtmlString Flash(this HtmlHelper helper)
        {

            var message = "";
            var className = "";
            if (helper.ViewContext.TempData["info"] != null)
            {
                message = helper.ViewContext.TempData["info"].ToString();
                className = "info";
            }
            else if (helper.ViewContext.TempData["warning"] != null)
            {
                message = helper.ViewContext.TempData["warning"].ToString();
                className = "warning";
            }
            else if (helper.ViewContext.TempData["error"] != null)
            {
                message = helper.ViewContext.TempData["error"].ToString();
                className = "error";
            }
            
            if (className != "" && message != "")
            {
                var outer = new TagBuilder("div");
                outer.AddCssClass("span-22 append-1 prepend-1 top-margin");
                var inner = new TagBuilder("div");
                inner.AddCssClass(className);
                string closeLink = "";// "<div style='float:right;'><a href='' onclick='return toggleMessage();'>Close</a></div>";
                inner.InnerHtml = message + closeLink;
                outer.InnerHtml = inner.ToString();

                return MvcHtmlString.Create(outer.ToString());

            }
            else
            {
                return MvcHtmlString.Create("");
            }
        }
    }
}
