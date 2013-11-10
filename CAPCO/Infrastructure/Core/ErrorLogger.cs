using System;
using System.Web;

namespace CAPCO.Infrastructure.Core
{
    public static class ErrorLogger
    {
        public static void LogError(Exception exception)
        {
            if (exception != null)
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
        }
    }
}
