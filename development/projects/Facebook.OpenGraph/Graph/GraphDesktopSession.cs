using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Facebook.Graph.Util;
using System.Globalization;
using System.Security.Permissions;
using System.ComponentModel;
using System.Configuration;
#if !SILVERLIGHT
namespace Facebook.Graph
{
    /// <summary>
    /// Represents a Desktop session of the OpenGraph API.
    /// </summary>
    /// <remarks>
    /// <para>Developers seeking to leverage the OpenGraph API via a desktop application should utilize the 
    /// <see>OpenGraphDesktopSession</see> class.  While it allows a user's session token to be provided, it also 
    /// presents a callback to automatically refresh the session.  This allows the consuming application to present the
    /// Facebook authentication UI in its own, platform-specific way (thus eliminating any dependence on Windows Forms,
    /// WPF, or Gtk# within this library).</para>
    /// <para>Web server implementers should prefer to use the <see>OpenGraphAuthenticatedSession</see> class instead, 
    /// because of the single-threaded nature of the callback.  However, if you are using a session with the <c>offline_access</c>
    /// permission, because of its long-running nature and the unlikelihood that you'll be able to prevent the callback
    /// from returning in a web session.  For more information, view the conceptual help topic
    /// "Obtaining Authentication Tokens with a Web Application," which covers ASP.NET Web Forms, MVC 1.0, and MVC 2.0 projects.</para>
    /// </remarks>
    public class GraphDesktopSession
        : GraphSession
    {
        #region constructor overloads
        /// <summary>
        /// Creates a new <see cref="GraphDesktopSession"/> with the specified configuration name.
        /// </summary>
        /// <param name="applicationConfigurationName">The name of the application configuration element found
        /// in the application or web configuration file.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationConfigurationName"/> is 
        /// null or empty.</exception>
        /// <exception cref="ConfigurationErrorsException">Thrown if the application configuration entry with the specified
        /// name could not be located in the configuration file, or if the configuration file did not specify an 
        /// OpenGraph configuration section named <c>&lt;OpenGraph.NET&gt;</c>.</exception>
        public GraphDesktopSession(string applicationConfigurationName)
            : base(applicationConfigurationName)
        {

        }

        /// <summary>
        /// Creates a new <see cref="GraphDesktopSession"/> with the application configuration information specified.
        /// </summary>
        /// <param name="clientID">The application ID, provided by Facebook.</param>
        /// <param name="clientSecret">The application secret, provided by Facebook.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="clientID"/> or 
        /// <paramref name="clientSecret"/> is <see langword="null"/> or empty.</exception>
        public GraphDesktopSession(string clientID, string clientSecret)
            : base(clientID, clientSecret)
        {

        }

        /// <summary>
        /// Creates a new <see cref="GraphDesktopSession"/> with the application configuration information specified.
        /// </summary>
        /// <param name="clientID">The application ID, provided by Facebook.</param>
        /// <param name="clientSecret">The application secret, provided by Facebook.</param>
        /// <param name="authorizationCode">The authorization code provided by an already-executed Graph API request.</param>
        /// <param name="refreshUrl">The Redirect URL provided to the Graph API server when obtaining the authorization code.</param>
        /// <param name="isRefreshUrlEncoded">Whether <paramref name="refreshUrl"/> is URL-encoded.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="clientID"/>, <paramref name="clientSecret"/>, <paramref name="authorizationCode"/>, 
        /// or <paramref name="refreshUrl"/> is <see langword="null"/> or empty.</exception>
        public GraphDesktopSession(string clientID, string clientSecret, string authorizationCode, string refreshUrl, bool isRefreshUrlEncoded = false)
            : this(clientID, clientSecret)
        {
            if (!string.IsNullOrEmpty(authorizationCode) && string.IsNullOrEmpty(refreshUrl))
                throw new ArgumentNullException("refreshUrl", "If an authorization code is provided, then a redirect URL with which the authorization code was generated must be provided to regenerate the access token.");

            base.AuthorizationCode = authorizationCode;
            this.AccessTokenRefreshUrl = refreshUrl;
            this.IsAccessTokenRefreshUrlEncoded = isRefreshUrlEncoded;
        }

        /// <summary>
        /// Creates a new <see cref="GraphDesktopSession"/> with the application configuration information specified.
        /// </summary>
        /// <param name="clientID">The application ID, provided by Facebook.</param>
        /// <param name="clientSecret">The application secret, provided by Facebook.</param>
        /// <param name="authorizationCode">The authorization code provided by an already-executed Graph API request.</param>
        /// <param name="refreshUrl">The Redirect URL provided to the Graph API server when obtaining the authorization code.</param>
        /// <param name="isRefreshUrlEncoded">Whether <paramref name="refreshUrl"/> is URL-encoded.</param>
        /// <param name="accessToken">The pre-existing access token.</param>
        /// <param name="accessTokenExpiration">The expiration time of the access token.  If the access token does not expired (as requested with <see cref="ExtendedPermissions.OfflineAccess">OfflineAccess</see>, 
        /// then this should be set to <see cref="DateTime.MaxValue">DateTime.MaxValue</see>.</param>
        public GraphDesktopSession(string clientID, string clientSecret, string authorizationCode, string accessToken,
            DateTime accessTokenExpiration, string refreshUrl, bool isRefreshUrlEncoded = false)
            : this(clientID, clientSecret, authorizationCode, refreshUrl)
        {
            base.AccessToken = accessToken;
            base.AccessTokenExpiration = accessTokenExpiration;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the redirect URL for obtaining the authorization code and access token for OAuth.
        /// </summary>
        /// <remarks>
        /// <para>This property is automatically set based on the last results of the call to the <see cref="RefreshCallback"/> property, but can also be manually set when setting the other components
        /// of the authentication mechanism.</para>
        /// </remarks>
        public string AccessTokenRefreshUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="AccessTokenRefreshUrl"/> property is a URL-encoded value.
        /// </summary>
        /// <remarks>
        /// <para>This property is automatically set based on the last results of the call to the <see cref="RefreshCallback"/> property, but can also be manually set when setting the other components
        /// of the authentication mechanism.</para>
        /// </remarks>
        public bool IsAccessTokenRefreshUrlEncoded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the callback that is used to refresh the OAuth Access Token.
        /// </summary>
        public RetrieveAuthorizationCallback RefreshCallback
        {
            get;
            set;
        }

        private AuthorizationPromptStyle m_promptStyle = AuthorizationPromptStyle.Popup;
        /// <summary>
        /// Gets or sets the style that the authorization prompt should display.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException">Thrown if <paramref name="value"/> is not a valid value of the <see cref="AuthorizationPromptStyle"/> enumeration.</exception>
        public AuthorizationPromptStyle PromptStyle
        {
            get { return m_promptStyle; }
            set
            {
                if (!Enum.IsDefined(typeof(AuthorizationPromptStyle), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(AuthorizationPromptStyle));

                m_promptStyle = value;
            }
        }
        #endregion

        /// <inheritdoc />
        protected override void RefreshSession()
        {
            if (string.IsNullOrEmpty(AccessTokenRefreshUrl))
                throw new InvalidOperationException("In order to refresh a session, the Access Token Refresh URL must be set.");

            if (string.IsNullOrEmpty(AuthorizationCode))
            {
                if (RefreshCallback == null)
                    throw new InvalidOperationException("The session may not be refreshed if no refresh callback is set and no authorization code is already set.");

                string callbackUrl = this.AccessTokenRefreshUrl;
                string url = Facebook.Graph.Util.AccessToken.DetermineUriForRequest(ApplicationID, AccessTokenRefreshUrl, IsAccessTokenRefreshUrlEncoded, RequiredPermissions, PromptStyle);

                var result = RefreshCallback(url);
                this.AccessTokenRefreshUrl = result.RedirectUrl;
                this.IsAccessTokenRefreshUrlEncoded = result.IsRedirectUrlEncoded;

                var responseResult = result.ResponseQueryString.SplitIntoDictionary();
                if (responseResult.ContainsKey("code"))
                    AuthorizationCode = responseResult["code"];
                else if (responseResult.ContainsKey("error_reason"))
                {
                    throw ExceptionParser.CreateForAuthErrorReason(responseResult["error_reason"]);
                }
            }

            if (string.IsNullOrEmpty(AuthorizationCode))
                throw new OAuthException("Unable to refresh the session due to an invalid authorization code.");

            RefreshSessionWithCode(AccessTokenRefreshUrl, IsAccessTokenRefreshUrlEncoded);
        }
    }
}
#endif