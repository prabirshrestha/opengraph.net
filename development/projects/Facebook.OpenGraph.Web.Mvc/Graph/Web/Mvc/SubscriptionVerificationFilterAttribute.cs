using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Facebook.Graph.Util;

namespace Facebook.Graph.Web.Mvc
{
    internal sealed class SubscriptionVerificationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string mode = filterContext.HttpContext.Request.QueryString["hub.mode"];
            string challenge = filterContext.HttpContext.Request.QueryString["hub.challenge"];
            string verifyToken = filterContext.HttpContext.Request.QueryString["hub.verify_token"];

            if (mode == "subscribe")
            {
                filterContext.ActionParameters["challenge"] = challenge;
                filterContext.ActionParameters["verify_token"] = verifyToken;
            }
            else
            {
                string text = filterContext.HttpContext.Request.InputStream.ReadToString();
                JToken token = Fetcher.FromJsonText(text);
                filterContext.ActionParameters["content"] = token;
            }
        }
    }
}
