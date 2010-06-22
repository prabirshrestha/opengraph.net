using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Facebook.OpenGraph.Configuration;
using System.Configuration;
using Facebook.Graph.Util;

namespace Facebook.Graph.Web.Mvc
{
    /// <summary>
    /// Implements the basic behavior necessary for a <see cref="Controller"/> class to handle incoming OpenGraph 
    /// authorization requests.  For more information, see <a href="43fd5bf6-3117-42cc-955d-85e8a4e23c15.htm">How to: Create an 
    /// OpenGraph ASP.NET MVC Application</a>.
    /// </summary>
    public abstract class AuthorizationController
        : Controller
    {
        private const string UNKNOWN_RESPONSE = "unknown_response";

        /// <summary>
        /// Handles an authorization response from the OpenGraph API.  This is a handler 
        /// for a callback from the OpenGraph API, and should not be called directly from 
        /// your code; see the Remarks section for information about how to utilize this
        /// controller action.
        /// </summary>
        /// <param name="applicationName">The name of the application being approved.</param>
        /// <param name="code">The access code used to be exchanged for an access token.  This is provided by the callback mechanism.</param>
        /// <param name="errorCode">The error code from the OpenGraph API.</param>
        /// <param name="accessToken">The access token used to make requests on behalf of the user.</param>
        /// <param name="expiration">The expiration time, in seconds from present, of the access token.</param>
        /// <returns>An <see cref="ActionResult"/> for the controller action that should be taken next.</returns>
        /// <remarks>
        /// <para>This is a convenience utility method provided for developers using an ASP.NET MVC application.  In order to authorize your application's
        /// use of the OpenGraph API on behalf of a user, create a new Controller class that inherits from this class and implement the abstract methods
        /// defined in this class.  This method will invoke those methods based on the response data from the OpenGraph API.</para>
        /// <para>Once you have created your controller, add a route to the controller, passing in the application name either as a routing parameter 
        /// or as a string.  For more information, refer to the conceptual article <a href="43fd5bf6-3117-42cc-955d-85e8a4e23c15.htm">How to: Create an 
        /// OpenGraph ASP.NET MVC Application</a>.</para>
        /// <para>This method is called by the OpenGraph API infrastructure and is not intended to be called directly from your code.</para>
        /// </remarks>
        [AuthorizationParametersFilter]
        public ActionResult Authorize(string applicationName, string code, string errorCode, 
            string accessToken, long expiration)
        {
            var handlerConfig = GetHandlerConfig(applicationName);
            var appConfig = GetAppConfig(handlerConfig);

            if (!string.IsNullOrEmpty(errorCode))
            {
                return HandleErrorResponse(applicationName, DetermineErrorReasonKnownValue(errorCode));
            }

            DateTime expires = DateTime.Now;
            if (!string.IsNullOrEmpty(code))
            {
                accessToken = AccessToken.Retrieve(appConfig.ID, appConfig.Secret, handlerConfig.FullUri, code, out expires, false);
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                return HandleSuccess(applicationName, accessToken, expires);
            }

            return HandleErrorResponse(applicationName, UNKNOWN_RESPONSE);
        }

        private static string DetermineErrorReasonKnownValue(string errorCode)
        {
            // TODO: Implement via internals call
            throw new NotImplementedException();
        }

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
        /// <param name="applicationName">The name of the application requesting the access.</param>
        protected abstract ActionResult HandleErrorResponse(string applicationName, string errorReason);

        /// <summary>
        /// When implemented in a derived class, handles a successful authentication response and access token.
        /// </summary>
        /// <param name="applicationName">The name of the application that was approved for access.</param>
        /// <param name="accessToken">The token with which to create an authenticated session.</param>
        /// <param name="expiration">The time (relative to the application requesting the token) at which the token expires.  If the user allowed for the <c>offline_access</c> extended permission
        /// (represented by <see cref="ExtendedPermissions.OfflineAccess"/>), then this parameter will be <see cref="DateTime.MaxValue"/>.</param>
        protected abstract ActionResult HandleSuccess(string applicationName, string accessToken, DateTime expiration);

        /// <summary>
        /// Invokes <see cref="HandleSuccess(System.String, System.String, System.DateTime)"/> with a <see cref="DateTime"/> value of <see cref="DateTime.MaxValue" />, indicating an infinite session.
        /// </summary>
        /// <param name="applicationName">The name of the application that was approved for access.</param>
        /// <param name="accessToken">The access token provided by OAuth.</param>
        protected virtual ActionResult HandleSuccess(string applicationName, string accessToken)
        {
            return HandleSuccess(applicationName, accessToken, DateTime.MaxValue);
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

        private HandlerConfigurationElement GetHandlerConfig(string appName)
        {
            var webConfig = OpenGraphWebConfigurationSection.Instance;
            if (webConfig == null)
                throw new ConfigurationErrorsException("Missing section <OpenGraph.NET-Web> in configuration file.");

            var item = webConfig.Handlers[appName];
            if (item == null)
                throw new ConfigurationErrorsException("No <Handler> element exists for the application handler named '" + appName + "'.");

            return item;
        }
        #endregion
    }
}
