using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Facebook.Graph.Web.Mvc
{
    internal sealed class AuthorizationParametersFilterAttribute
        : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string errorReason = filterContext.HttpContext.Request["error_reason"];
            string accessToken = filterContext.HttpContext.Request["access_token"];
            string expiration = filterContext.HttpContext.Request["expires"];
            string code = filterContext.HttpContext.Request["code"];

            long expires = 0;
            if (!string.IsNullOrEmpty(expiration))
            {
                long.TryParse(expiration, out expires);
            }

            filterContext.ActionParameters["code"] = code;
            filterContext.ActionParameters["errorCode"] = errorReason;
            filterContext.ActionParameters["accessToken"] = accessToken;
            filterContext.ActionParameters["expiration"] = expires;

            base.OnActionExecuting(filterContext);
        }
    }
}
