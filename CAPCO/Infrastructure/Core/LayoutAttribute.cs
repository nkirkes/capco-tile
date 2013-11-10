using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace CAPCO.Infrastructure.Core
{
    public class FileSizeAttribute : ValidationAttribute
    {
        public int MaxContentLength { get; set; }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;

            //this should be handled by [Required]
            if (file == null)
                return true;

            if (file.ContentLength > MaxContentLength)
            {
                return false;
            }

            return true;
        }
    }

    public class LayoutAttribute : ActionFilterAttribute
    {
        private readonly string _masterName;
        public LayoutAttribute(string masterName)
        {
            _masterName = masterName;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var result = filterContext.Result as ViewResult;
            if (result != null)
            {
                result.MasterName = _masterName;
            }
        }
    }
}