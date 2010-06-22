using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Facebook.Graph.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string CanvasRouteLink(this HtmlHelper html, string linkText, object routeValues)
        {
            return html.RouteLink(linkText, routeValues, new { target = "_top" });
        }

        public static string CanvasRouteLink(this HtmlHelper html, string linkText, string routeName)
        {
            return html.RouteLink(linkText, routeName, null, new { target = "_top" });
        }

        public static string CanvasRouteLink(this HtmlHelper html, string linkText, string routeName, object routeValues)
        {
            return html.RouteLink(linkText, routeName, routeValues, new { target = "_top" });
        }
    }
}
