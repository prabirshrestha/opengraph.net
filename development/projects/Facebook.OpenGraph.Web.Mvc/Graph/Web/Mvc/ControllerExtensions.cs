using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Facebook.Graph.Web.Mvc
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Renders code to cause the outer frame to refresh and redirect to the specified route, relative to the 
        /// apps.facebook.com domain.
        /// </summary>
        /// <param name="controller">The controller from which to call this method.</param>
        /// <param name="applicationUri">The application URI root.</param>
        /// <param name="routeName">The name of the route.</param>
        /// <returns>An <see cref="ActionResult"/> that redirects the outer frame to the relative route.</returns>
        public static ActionResult CanvasIFrameRedirectToRoute(this Controller controller, string applicationUri, string routeName)
        {
            UrlHelper helper = new UrlHelper(controller.ControllerContext.RequestContext);
            string dest = string.Concat(applicationUri, helper.RouteUrl(routeName));
            return controller.CanvasIFrameRedirect(dest);
        }

        public static ActionResult CanvasIFrameRedirectToRoute(this Controller controller, string applicationUri, string routeName, object routeParameters)
        {
            UrlHelper helper = new UrlHelper(controller.ControllerContext.RequestContext);
            string dest = string.Concat(applicationUri, helper.RouteUrl(routeName, routeParameters));
            return controller.CanvasIFrameRedirect(dest);
        }

        public static ActionResult CanvasIFrameRedirectToRoute(this Controller controller, string applicationUri, object routeValues)
        {
            UrlHelper helper = new UrlHelper(controller.ControllerContext.RequestContext);
            string dest = string.Concat(applicationUri, helper.RouteUrl(routeValues));
            return controller.CanvasIFrameRedirect(dest);
        }

        /// <summary>
        /// Redirects the outer canvas frame to the specified URL.
        /// </summary>
        /// <param name="controller">The controller on which to invoke the action.</param>
        /// <param name="targetUrl">The URL to which to redirect.</param>
        /// <returns>An <see cref="ActionResult"/> that redirects the outer frame to the destination URL.</returns>
        public static ActionResult CanvasIFrameRedirect(this Controller controller, string targetUrl)
        {
            return new CanvasIFrameRedirectActionResult(targetUrl);
        }

        private class CanvasIFrameRedirectActionResult
            : ActionResult
        {
            public CanvasIFrameRedirectActionResult(string targetUrl)
            {
                if (string.IsNullOrEmpty(targetUrl))
                    throw new ArgumentNullException("targetUrl");

                TargetUrl = targetUrl;
            }

            public string TargetUrl
            {
                get;
                set;
            }


            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.Write(@"<html>
<head>
 <title>Redirecting...</title>
 <script type=""text/javascript"">
window.top.location = '" + TargetUrl.Replace("\'", "\\\'") + @"';
 </script>
</head>
<body>
    <p>Please wait...</p>
</body>
</html>");
            }
        }
    }
}
