using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Facebook.OpenGraph.Configuration;
using System.Configuration;

namespace Facebook.Graph.Web.Mvc
{
    /// <summary>
    /// Provides functionality for verifying a Facebook signature.
    /// </summary>
    /// <remarks>
    /// <para>Apply this attribute to a Canvas page action method to verify that the action is being initiated via 
    /// Facebook's server.  Alternatively, you may apply the <see cref="FacebookStatusFilterAttribute"/> to the method 
    /// to obtain information about the user currently using the Canvas page.  If the user is not actually on a Facebook
    /// Canvas page, an <see cref="OAuthException"/> will be thrown prior to invoking the method.</para>
    /// </remarks>
    public class VerifyFacebookSignatureFilterAttribute : ActionFilterAttribute
    {
        private string m_appName;
        /// <summary>
        /// Gets or sets the name of the application that should be used for configuration data.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return m_appName;
            }
            set
            {
                m_appName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    OpenGraphConfigurationSection config = OpenGraphConfigurationSection.Instance;
                    var app = config.Applications[value];
                    if (app != null)
                    {
                        Secret = app.Secret;
                    }
                    else
                    {
                        throw new ConfigurationErrorsException("No matching application configuration was found named '" + value + "'.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the application secret key.
        /// </summary>
        public string Secret
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(Secret))
                throw new InvalidOperationException("Cannot verify signature without specifying a Secret.");

            var qs = filterContext.HttpContext.Request.QueryString;
            string fbSig = qs["fb_sig"];
            if (string.IsNullOrEmpty(fbSig))
                return;

            StringBuilder verifier = new StringBuilder();
            foreach (string key in qs.AllKeys.Where(key => key.StartsWith("fb_sig_")).OrderBy(key => key))
            {
                verifier.Append(key.Substring(7));
                verifier.Append('=');
                verifier.Append(qs[key]);
            }
            verifier.Append(Secret);

            string sig = verifier.ToString().ToMd5String();

            if (!fbSig.Equals(sig, StringComparison.OrdinalIgnoreCase))
                throw new OAuthException("Facebook signature verification failed.");
        }
    }
}
