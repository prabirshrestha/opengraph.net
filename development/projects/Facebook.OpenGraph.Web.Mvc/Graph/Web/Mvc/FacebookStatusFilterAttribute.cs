using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Facebook.Graph.Web.Mvc
{
    /// <summary>
    /// Filters Facebook query string data into a status parameter.
    /// </summary>
    /// <remarks>
    /// <para>This attribute authorizes Canvas page action requests by verifying </para>
    /// </remarks>
    public sealed class FacebookStatusFilterAttribute : VerifyFacebookSignatureFilterAttribute
    {
        //private const string SESSION_KEY = "FacebookStatusFilterAttribute::SessionKey";

        /// <summary>
        /// Creates a new, empty <see cref="FacebookStatusFilterAttribute"/>.
        /// </summary>
        public FacebookStatusFilterAttribute() { }
        /// <summary>
        /// Creates a new <see cref="FacebookStatusFilterAttribute"/> that filters Facebook parameter data into a <see cref="FacebookStatus"/> object with the given parameter name.
        /// </summary>
        /// <param name="parameterName">The name of the paramter that should receive the status object.</param>
        public FacebookStatusFilterAttribute(string parameterName)
            : this()
        {
            ParameterName = parameterName;
        }

        /// <summary>
        /// Gets or sets the name of the parameter that should receive the status object.
        /// </summary>
        public string ParameterName { get; set; }

        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(ParameterName))
                throw new InvalidOperationException("Cannot feed Facebook status into a parameter without specifying the parameter name.");

            //var session = filterContext.HttpContext.Session;
            //var fbStatus = session[SESSION_KEY] as FacebookStatus;
            //if (fbStatus != null && fbStatus.IsLoggedIn)
            //{
            //    filterContext.ActionParameters[ParameterName] = fbStatus;
            //}
            //else
            //{
            var qs = filterContext.HttpContext.Request.QueryString;
            var fbStatus = new FacebookStatus();
            string user = qs["fb_sig_user"];
            if (user != null)
            {
                fbStatus.UserID = user;
                fbStatus.IsLoggedIn = true;

                string permissionSet = qs["fb_sig_ext_perms"] ?? string.Empty;
                fbStatus.Permissions = permissionSet.ParseIntoPermissions();
                fbStatus.SessionKey = qs["fb_sig_session_key"];
            }
            fbStatus.IsAppUser = "1".Equals(qs["fb_sig_added"], StringComparison.Ordinal);
            fbStatus.Locale = qs["fb_sig_locale"];

            filterContext.ActionParameters[ParameterName] = fbStatus;
            //session[SESSION_KEY] = fbStatus;
            //}


#if DEBUG
            fbStatus.FbParameters.Clear();
            foreach (var item in filterContext.HttpContext.Request.QueryString.Keys)
            {
                string key = item.ToString();
                fbStatus.FbParameters.Add(key, filterContext.HttpContext.Request.QueryString[key]);
            }
#endif

            base.OnActionExecuting(filterContext);
        }
    }
}
