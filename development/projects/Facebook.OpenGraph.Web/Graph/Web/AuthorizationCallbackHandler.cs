using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Globalization;
using Facebook.OpenGraph.Configuration;
using System.Configuration;

namespace Facebook.Graph.Web
{
    /// <summary>
    /// Provides the basic functionality for a custom HTTP handler used to support the OpenGraph OAuth protocol as a web server client.
    /// </summary>
    public abstract class AuthorizationCallbackHandler : IHttpHandler
    {
        private const string UNKNOWN_RESPONSE = "unknown_response";
        private static HashSet<string> KnownErrorReasons = new HashSet<string>() { "user_denied", "unknown_expiration", UNKNOWN_RESPONSE };
        #region IHttpHandler Members

        /// <inheritdoc />
        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        /// <inheritdoc />
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var qs = context.Request.QueryString;
            string error = qs["error_reason"];
            if (!string.IsNullOrEmpty(error))
            {
                HandleErrorResponse(DetermineErrorReasonKnownValue(error));
                return;
            }

            string code = qs["code"];
            if (!string.IsNullOrEmpty(code))
            {
                var handlerConfig = GetHandlerConfig();
                var appConfig = GetAppConfig(handlerConfig);
                // we're getting a code, now to retrieve the root session
                string tokenUrlFmt = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";

                string url = string.Format(CultureInfo.CurrentCulture, tokenUrlFmt, appConfig.ID, handlerConfig.FullUri, appConfig.Secret, code);
                context.Response.Redirect(url, true);
                return;
            }

            string accessToken = qs["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                string szExpires = qs["expires"];
                if (string.IsNullOrEmpty(szExpires))
                {
                    HandleSuccess(accessToken);
                }
                else
                {
                    int seconds;
                    if (!int.TryParse(szExpires, out seconds))
                    {
                        HandleErrorResponse("unknown_expiration");
                        return;
                    }
                    DateTime date = DateTime.Now.AddSeconds(seconds);
                    HandleSuccess(accessToken, date);
                }

                return;
            }

            HandleErrorResponse(UNKNOWN_RESPONSE);
        }

        #region config retrievals
        private ApplicationConfigurationElement GetAppConfig(HandlerConfigurationElement handlerConfig)
        {
            OpenGraphConfigurationSection section = OpenGraphConfigurationSection.Instance;
            if (section == null)
                throw new ConfigurationErrorsException("Missing section <OpenGraph.NET> in configuration file.");

            var item = section.Applications[handlerConfig.For];
            if (item == null)
                throw new ConfigurationErrorsException("No <Application> element exists for the application named '" + handlerConfig.For + "'.");

            return item;
        }

        private HandlerConfigurationElement GetHandlerConfig()
        {
            var webConfig = OpenGraphWebConfigurationSection.Instance;
            if (webConfig == null)
                throw new ConfigurationErrorsException("Missing section <OpenGraph.NET-Web> in configuration file.");

            var item = webConfig.Handlers[ApplicationName];
            if (item == null)
                throw new ConfigurationErrorsException("No <Handler> element exists for the application handler named '" + ApplicationName + "'.");

            return item;
        }
        #endregion

        private static string DetermineErrorReasonKnownValue(string errorReason)
        {
            string result = errorReason;
            if (!KnownErrorReasons.Contains(errorReason))
                result = UNKNOWN_RESPONSE;

            return result;
        }

        #endregion

        #region Main API
        /// <summary>
        /// When implemented in a derived class, handles an unsuccessful response.
        /// </summary>
        /// <param name="errorReason">
        /// <para>The error reason.  Known error reasons are:</para>
        /// <list type="bullet">
        ///     <item><c>user_denied</c> indicates that the user denied one or more extended permissions.</item>
        ///     <item><c>unknown_expiration</c> indicates that the server didn't recognize the expiration value that was included
        ///     in the access token.</item>
        ///     <item><c>unknown_response</c> indicates that all expected response parameters were not included.</item>
        /// </list>
        /// </param>
        protected abstract void HandleErrorResponse(string errorReason);

        /// <summary>
        /// When implemented in a derived class, handles a successful authentication response and access token.
        /// </summary>
        /// <param name="accessToken">The token with which to create an authenticated session.</param>
        /// <param name="expiration">The time (relative to the application requesting the token) at which the token expires.  If the user allowed for the <c>offline_access</c> extended permission
        /// (represented by <see cref="ExtendedPermissions.OfflineAccess"/>), then this parameter will be <see cref="DateTime.MaxValue"/>.</param>
        protected abstract void HandleSuccess(string accessToken, DateTime expiration);

        /// <summary>
        /// Invokes <see cref="HandleSuccess(System.String, System.DateTime)"/> with a <see cref="DateTime"/> value of <see cref="DateTime.MaxValue" />, indicating an infinite session.
        /// </summary>
        /// <param name="accessToken">The access token provided by OAuth.</param>
        protected virtual void HandleSuccess(string accessToken)
        {
            HandleSuccess(accessToken, DateTime.MaxValue);
        }

        /// <summary>
        /// When implemented in a derived class, specifies the name of the application, corresponding to the 
        /// <c>For</c> attribute of a Handler, and the <c>Name</c> attribute of an App, in the configuration file.
        /// </summary>
        /// <remarks>
        /// <para>For more information, see the conceptual topic "How to: Construct a Web-Based Authorization Handler".</para>
        /// </remarks>
        public abstract string ApplicationName
        {
            get;
        }
        #endregion
    }
}
