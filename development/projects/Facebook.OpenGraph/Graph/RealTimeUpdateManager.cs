using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.Graph.Util;
using System.Globalization;
using System.Linq.Expressions;
using Facebook.OpenGraph.Configuration;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Security.Permissions;

namespace Facebook.Graph
{
    /// <summary>
    /// Contains methods for registering for real-time updates for a Graph API application.
    /// </summary>
#if !SILVERLIGHT
    [ReflectionPermission(SecurityAction.LinkDemand, ReflectionEmit = true, MemberAccess = true, RestrictedMemberAccess = false)]
    [ReflectionPermission(SecurityAction.InheritanceDemand, ReflectionEmit = true, MemberAccess = true, RestrictedMemberAccess = false)]
#endif
    public sealed class RealTimeUpdateManager
    {
        private const string APP_ID_ACCESS_TOKEN_URL_FMT = "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&type=client_cred";
        private const string SUBSCRIPTION_REG_URL_FMT = "https://graph.facebook.com/{0}/subscriptions?access_token={1}";
        private const string SUBSCRIPTION_DEL_SINGLE_URL_FMT = "https://graph.facebook.com/{0}/subscriptions?access_token={1}&object={2}";

        #region constructors
        /// <summary>
        /// Initializes a new <see>RealTimeUpdateManager</see> using configuration data.
        /// </summary>
        /// <param name="applicationConfigurationName">The name of the application configuration element found
        /// in the application or web configuration file.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationConfigurationName"/> is 
        /// null or empty.</exception>
        /// <exception cref="ConfigurationErrorsException">Thrown if the application configuration entry with the specified
        /// name could not be located in the configuration file, or if the configuration file did not specify an 
        /// OpenGraph configuration section named <c>&lt;OpenGraph.NET&gt;</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the application configuration name was not found in the
        /// Silverlight-based configuration registry.</exception>
        public RealTimeUpdateManager(string applicationConfigurationName)
        {
            if (string.IsNullOrEmpty(applicationConfigurationName))
                throw new ArgumentNullException("applicationConfigurationName");

            var config = OpenGraphConfigurationSection.Instance;
#if !SILVERLIGHT
            if (config == null)
                throw new ConfigurationErrorsException("The <OpenGraph.NET> configuration section is missing.  It must be created in the application's configuration file, specifying the type Facebook.Graph.OpenGraphConfigurationSection as its type.");
#endif

            var appConfig = config.Applications[applicationConfigurationName];
            if (appConfig == null)
#if !SILVERLIGHT
                throw new ConfigurationErrorsException("The specified application name, '" + applicationConfigurationName + "', was not found in the OpenGraph configuration section.");
#else
                throw new InvalidOperationException("The specified application name, '" + applicationConfigurationName + "', was not found in the configuration registry.");
#endif

            ApplicationID = appConfig.ID;
            ClientSecret = appConfig.Secret;
            SubscriptionUpdateHandlerUrl = appConfig.SubscriptionHandlerUrl;
        }

        /// <summary>
        /// Initializes a new <see>RealTimeUpdateManager</see>.
        /// </summary>
        /// <param name="applicationID">Specifies the application ID.</param>
        /// <param name="applicationSecret">Specifies the application secret.</param>
        /// <param name="callbackUrl">Specifies the callback URL to use for default callback registration.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationID"/> or 
        /// <paramref name="applicationSecret"/> is null or empty.</exception>
        public RealTimeUpdateManager(string applicationID, string applicationSecret, string callbackUrl)
        {
            if (string.IsNullOrEmpty("applicationID"))
                throw new ArgumentNullException("applicationID");

            if (string.IsNullOrEmpty("applicationSecret"))
                throw new ArgumentNullException("applicationSecret");

            ApplicationID = applicationID;
            ClientSecret = applicationSecret;
            SubscriptionUpdateHandlerUrl = callbackUrl;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the User-Agent header sent with requests made to the Graph API server.
        /// </summary>
        public static string UserAgent
        {
            get { return GraphSession.UserAgent; }
        }

        /// <summary>
        /// Gets the application ID used for this session.
        /// </summary>
        public string ApplicationID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application secret used for this session.
        /// </summary>
        public string ClientSecret
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets, and in derived classes, sets, the access token used to access the OpenGraph server API for this session.
        /// </summary>
        public string AccessToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the URL that should be used to access subscriptions for this session.
        /// </summary>
        public string SubscriptionUpdateHandlerUrl
        {
            get;
            set;
        }
        #endregion

        #region methods
#if !SILVERLIGHT
        private void GetAccessToken()
        {
            string url = string.Format(CultureInfo.InvariantCulture, APP_ID_ACCESS_TOKEN_URL_FMT, ApplicationID, ClientSecret);
            string data = Fetcher.Fetch(url);

            if (string.IsNullOrEmpty(data))
                throw new OAuthException("Could not retrieve application access token for subscription registration.");

            AccessToken = data.Substring(data.IndexOf('=') + 1);
        }
#endif

        #region RegisterSubscription<TSource, TResult> overloads
#if !SILVERLIGHT
        /// <summary>
        /// Registers an entity type for real-time update notifications from the Graph API.
        /// </summary>
        /// <remarks>
        /// <para>Not all types of entities can be registered for real-time update notifications from the Graph API; 
        /// as of May 6, 2010, only <see cref="User"/> entities may be registered, although your application may also 
        /// register for user permission set changes and for errors.  Additionally, not all connections may be 
        /// registered, although this method does not discriminate these subscriptions.</para>
        /// <para>You should be able to simply pass in a simple lambda expression into this method, where the compiler
        /// will automatically infer that you want to generate an expression tree.  However, the lambda expression 
        /// must be formed with specific requirements: it must accept a single parameter, of a type deriving both from
        /// <see cref="GraphEntity"/> and <see cref="ISubscribableEntity"/>; and it must either return exactly 
        /// that same entity, or it must return a new anonymous type comprised only of properties of the originating
        /// entity.  See the examples for these two scenarios.</para>
        /// </remarks>
        /// <typeparam name="TEntity">The entity type that is being registered.  This type must be derived from
        /// <see cref="GraphEntity"/> and must also be marked with the interface 
        /// <see cref="ISubscribableEntity"/>.</typeparam>
        /// <typeparam name="TResult">An implied type based on the return value of the expression specified 
        /// in <paramref name="fieldSelector"/>.</typeparam>
        /// <param name="fieldSelector">The expression specifying the object properties for which to receive real-time updates.</param>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="fieldSelector"/> expression contains a 
        /// non-supported expression type; see the Remarks for more information.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="callbackUrl"/> was null or empty, and 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> was also null or empty.</exception>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        /// <example>
        ///     <para>The following code requests that a <see cref="User"/>'s name, friends list, likes list, and e-mail
        ///     be registered for real-time updates:</para>
        ///     <code lang="csharp">
        ///     <![CDATA[
        /// static void RegisterUserUpdates(OpenGraphSession session)
        /// {
        ///     session.RegisterSubscription( (User u) => new { u.Name, u.Friends, u.Likes, u.Email } );
        /// }
        ///     ]]>
        ///     </code>
        ///     <para>The following code requests that all fields of a <see cref="User"/> object be registered for 
        ///     real-time updates:</para>
        ///     <code lang="csharp">
        ///     <![CDATA[
        /// static void RegisterUserUpdates(OpenGraphSession session) 
        /// {
        ///     session.RegisterSubscription( (User u) => u );
        /// }
        ///     ]]>
        ///     </code>
        /// </example>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void RegisterSubscription<TEntity, TResult>(Expression<Func<TEntity, TResult>> fieldSelector,
            string callbackUrl = null)
            where TEntity : GraphEntity, ISubscribableEntity
        {
            string url = callbackUrl ?? SubscriptionUpdateHandlerUrl;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The subscription handler URL has not been specified.");

            string fieldsList, entityType;
            fieldsList = GraphExpressionParser.ParseRequestExpression(fieldSelector, out entityType);

            RegisterSubscription(fieldsList, entityType, callbackUrl);
        }
#endif

        // TODO: Update for 1.0 - complete Async example

        /// <summary>
        /// Registers an entity type for real-time update notifications from the Graph API.
        /// </summary>
        /// <remarks>
        /// <para>Not all types of entities can be registered for real-time update notifications from the Graph API; 
        /// as of May 6, 2010, only <see cref="User"/> entities may be registered, although your application may also 
        /// register for user permission set changes and for errors.  Additionally, not all connections may be 
        /// registered, although this method does not discriminate these subscriptions.</para>
        /// <para>You should be able to simply pass in a simple lambda expression into this method, where the compiler
        /// will automatically infer that you want to generate an expression tree.  However, the lambda expression 
        /// must be formed with specific requirements: it must accept a single parameter, of a type deriving both from
        /// <see cref="GraphEntity"/> and <see cref="ISubscribableEntity"/>; and it must either return exactly 
        /// that same entity, or it must return a new anonymous type comprised only of properties of the originating
        /// entity.  See the examples for these two scenarios.</para>
        /// </remarks>
        /// <typeparam name="TEntity">The entity type that is being registered.  This type must be derived from
        /// <see cref="GraphEntity"/> and must also be marked with the interface 
        /// <see cref="ISubscribableEntity"/>.</typeparam>
        /// <typeparam name="TResult">An implied type based on the return value of the expression specified 
        /// in <paramref name="fieldSelector"/>.</typeparam>
        /// <param name="fieldSelector">The expression specifying the object properties for which to receive real-time updates.</param>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null" />.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="fieldSelector"/> expression contains a 
        /// non-supported expression type; see the Remarks for more information.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="callbackUrl"/> was null or empty, and 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> was also null or empty.</exception>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        /// <example>
        ///     <para>The following code requests that a <see cref="User"/>'s name, friends list, likes list, and e-mail
        ///     be registered for real-time updates:</para>
        ///     <code lang="csharp">
        ///     <![CDATA[
        /// static void RegisterUserUpdates(OpenGraphSession session)
        /// {
        ///     session.RegisterSubscription( (User u) => new { u.Name, u.Friends, u.Likes, u.Email } );
        /// }
        ///     ]]>
        ///     </code>
        ///     <para>The following code requests that all fields of a <see cref="User"/> object be registered for 
        ///     real-time updates:</para>
        ///     <code lang="csharp">
        ///     <![CDATA[
        /// static void RegisterUserUpdates(OpenGraphSession session) 
        /// {
        ///     session.RegisterSubscription( (User u) => u );
        /// }
        ///     ]]>
        ///     </code>
        /// </example>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void RegisterSubscriptionAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> fieldSelector, string callbackUrl = null, Action callback = null)
            where TEntity : GraphEntity, ISubscribableEntity
        {
            throw new NotSupportedException();
        }
        #endregion

        #region RegisterSubscription(string, string, string) overloads
#if !SILVERLIGHT
        /// <summary>
        /// Registers an entity type for real-time update notifications from the Graph API.
        /// </summary>
        /// <remarks>
        /// <para>Not all types of entities can be registered for real-time update notifications from the Graph API; 
        /// as of May 6, 2010, only <see cref="User"/> entities may be registered, although your application may also 
        /// register for user permission set changes and for errors.  Additionally, not all connections may be 
        /// registered, although this method does not discriminate these subscriptions.</para>
        /// <para>You should be able to simply pass in a simple lambda expression into this method, where the compiler
        /// will automatically infer that you want to generate an expression tree.  However, the lambda expression 
        /// must be formed with specific requirements: it must accept a single parameter, of a type deriving both from
        /// <see cref="GraphEntity"/> and <see cref="ISubscribableEntity"/>; and it must either return exactly 
        /// that same entity, or it must return a new anonymous type comprised only of properties of the originating
        /// entity.  See the examples for these two scenarios.</para>
        /// </remarks>
        /// <param name="fieldList">A comma-delimited list of Graph API object property names for the specified entity.</param>
        /// <param name="entityType">The Graph API-specified type name of the entity being registered.</param>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="callbackUrl"/> was null or empty, and 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> was also null or empty.</exception>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fieldList"/> or 
        /// <paramref name="entityType"> is <see langword="null"/> or empty.</paramref></exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        /// <example>
        ///     <para>The following code requests that a <see cref="User"/>'s name, friends list, likes list, and e-mail
        ///     be registered for real-time updates:</para>
        ///     <code lang="csharp">
        ///     <![CDATA[
        /// static void RegisterUserUpdates(OpenGraphSession session)
        /// {
        ///     session.RegisterSubscription("name,friends,likes,email");
        /// }
        ///     ]]>
        ///     </code>
        /// </example>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void RegisterSubscription(string fieldList, string entityType = "user", string callbackUrl = null)
        {
            if (string.IsNullOrEmpty(fieldList))
                throw new ArgumentNullException("fieldList");

            if (string.IsNullOrEmpty(entityType))
                throw new ArgumentNullException("entityType");

            string url = callbackUrl ?? SubscriptionUpdateHandlerUrl;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The subscription handler URL has not been specified.");

            if (string.IsNullOrEmpty(AccessToken))
                GetAccessToken();

            string targetUrl = string.Format(CultureInfo.InvariantCulture, SUBSCRIPTION_REG_URL_FMT, ApplicationID, AccessToken);
            string content = string.Format(CultureInfo.InvariantCulture, "object={0}&fields={1}&callback_url={2}&verify_token={3}", entityType.UrlEncode(), fieldList, url.UrlEncode(), 1);
            string result = Fetcher.Post(targetUrl, content);
            JToken token = Fetcher.FromJsonText(result);
            if (token.HasValues)
            {
                JToken error = token["error"];
                if (error != null)
                    throw ExceptionParser.Parse(error);
            }
        }
#endif

        /// <summary>
        /// Asynchronously registers an entity type for real-time update notifications from the Graph API.
        /// </summary>
        /// <remarks>
        /// <para>Not all types of entities can be registered for real-time update notifications from the Graph API; 
        /// as of May 6, 2010, only <see cref="User"/> entities may be registered, although your application may also 
        /// register for user permission set changes and for errors.  Additionally, not all connections may be 
        /// registered, although this method does not discriminate these subscriptions.</para>
        /// <para>You should be able to simply pass in a simple lambda expression into this method, where the compiler
        /// will automatically infer that you want to generate an expression tree.  However, the lambda expression 
        /// must be formed with specific requirements: it must accept a single parameter, of a type deriving both from
        /// <see cref="GraphEntity"/> and <see cref="ISubscribableEntity"/>; and it must either return exactly 
        /// that same entity, or it must return a new anonymous type comprised only of properties of the originating
        /// entity.  See the examples for these two scenarios.</para>
        /// </remarks>
        /// <param name="fieldList">A comma-delimited list of Graph API object property names for the specified entity.</param>
        /// <param name="entityType">The Graph API-specified type name of the entity being registered.</param>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be 
        /// <see langword="null" />.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="callbackUrl"/> was null or empty, and 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> was also null or empty.</exception>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fieldList"/> or 
        /// <paramref name="entityType"> is <see langword="null"/> or empty.</paramref></exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        /// <example>
        ///     <para>The following code requests that a <see cref="User"/>'s name, friends list, likes list, and e-mail
        ///     be registered for real-time updates:</para>
        ///     <code lang="csharp">
        ///     <![CDATA[
        /// static void RegisterUserUpdates(OpenGraphSession session)
        /// {
        ///     session.RegisterSubscription("name,friends,likes,email");
        /// }
        ///     ]]>
        ///     </code>
        /// </example>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void RegisterSubscription(string fieldList, string entityType = "user", string callbackUrl = null, Action callback = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region RegisterPermissionsSubscription overloads
#if !SILVERLIGHT
        /// <summary>
        /// Registers for a real-time update for when a user updates his or her application extended permissions.
        /// </summary>
        /// <param name="permissionsToMonitor">The permissions to monitor.</param>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="permissionsToMonitor"/> is equal to <see cref="ExtendedPermissions.None"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void RegisterPermissionsSubscription(ExtendedPermissions permissionsToMonitor, string callbackUrl = null)
        {
            if (permissionsToMonitor == ExtendedPermissions.None)
                throw new ArgumentOutOfRangeException("permissionsToMonitor", permissionsToMonitor, "Must specify at least one permission to monitor.");

            string url = callbackUrl ?? SubscriptionUpdateHandlerUrl;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The subscription handler URL has not been specified.");

            if (string.IsNullOrEmpty(AccessToken))
                GetAccessToken();

            string fieldList = permissionsToMonitor.Print();

            string targetUrl = string.Format(CultureInfo.InvariantCulture, SUBSCRIPTION_REG_URL_FMT, ApplicationID, AccessToken);
            string content = string.Format(CultureInfo.InvariantCulture, "object=permissions&fields={0}&callback_url={1}&verify_token={2}", fieldList, url.UrlEncode(), 1);
            string result = Fetcher.Post(targetUrl, content);
            JToken token = Fetcher.FromJsonText(result);
            if (token.HasValues)
            {
                JToken error = token["error"];
                if (error != null)
                    throw ExceptionParser.Parse(error);
            }
        }
#endif

        /// <summary>
        /// Asynchronously registers for a real-time update for when a user updates his or her application extended permissions.
        /// </summary>
        /// <param name="permissionsToMonitor">The permissions to monitor.</param>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null" />.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="permissionsToMonitor"/> is equal to <see cref="ExtendedPermissions.None"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void RegisterPermissionsSubscriptionAsync(ExtendedPermissions permissionsToMonitor, string callbackUrl = null, Action callback = null)
        {
            if (permissionsToMonitor == ExtendedPermissions.None)
                throw new ArgumentOutOfRangeException("permissionsToMonitor", "Must specify at least one permission to monitor.");

            throw new NotSupportedException();
        }
        #endregion

        #region RegisterErrorsSubscription overloads
#if !SILVERLIGHT
        /// <summary>
        /// Registers for canvas page errors reported to Facebook.
        /// </summary>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        public void RegisterErrorsSubscription(string callbackUrl = null)
        {
            string url = callbackUrl ?? SubscriptionUpdateHandlerUrl;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The subscription handler URL has not been specified.");

            if (string.IsNullOrEmpty(AccessToken))
                GetAccessToken();

            string fieldList = "canvas";

            string targetUrl = string.Format(CultureInfo.InvariantCulture, SUBSCRIPTION_REG_URL_FMT, ApplicationID, AccessToken);
            string content = string.Format(CultureInfo.InvariantCulture, "object=errors&fields={0}&callback_url={1}&verify_token={2}", fieldList, url.UrlEncode(), 1);
            string result = Fetcher.Post(targetUrl, content);
            JToken token = Fetcher.FromJsonText(result);
            if (token.HasValues)
            {
                JToken error = token["error"];
                if (error != null)
                    throw ExceptionParser.Parse(error);
            }
        }
#endif

        /// <summary>
        /// Registers for canvas page errors reported to Facebook.
        /// </summary>
        /// <param name="callbackUrl">A custom callback URL.  The default value of this parameter is <see langword="null"/>.  If 
        /// <see langword="null" /> is passed (or no value is provided), then the session will fall back to using the 
        /// <see cref="SubscriptionUpdateHandlerUrl"/> property.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null"/>.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public void RegisterErrorsSubscriptionAsync(string callbackUrl = null, Action callback = null)
        {
            string url = callbackUrl ?? SubscriptionUpdateHandlerUrl;
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The subscription handler URL has not been specified.");

            throw new NotSupportedException();
        }
        #endregion

        #region GetRegisteredSubscriptions overloads
#if !SILVERLIGHT
        /// <summary>
        /// Gets a series of registered subscriptions for the current application.
        /// </summary>
        /// <returns>An enumeration of registered subscriptions.</returns>
        public IEnumerable<SubscriptionRegistration> GetRegisteredSubscriptions()
        {
            if (string.IsNullOrEmpty(AccessToken))
                GetAccessToken();

            string targetUrl = string.Format(CultureInfo.InvariantCulture, SUBSCRIPTION_REG_URL_FMT, ApplicationID, AccessToken);
            var result = Fetcher.FetchToken(targetUrl);

            JArray items = (JArray)result["data"];
            for (int i = 0; i < items.Count; i++)
            {
                yield return new SubscriptionRegistration(items[i]);
            }
        }
#endif

        /// <summary>
        /// Gets a series of registered subscriptions for the current application.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This method should accept an enumeration of <see cref="SubscriptionRegistration"/> 
        /// objects as its only parameter.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public void GetRegisteredSubscriptionsAsync(Action<IEnumerable<SubscriptionRegistration>> callback)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region DeleteSubscription(string) overloads
#if !SILVERLIGHT
        /// <summary>
        /// Clears a subscription of a specified type.
        /// </summary>
        /// <param name="subscriptionType">Can be a Graph API entity type name, or one of the special subscription types
        /// (known currently as <c>"permissions"</c> or <c>"errors"</c>).  This parameter is not validated or URL-encoded.</param>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void DeleteSubscription(string subscriptionType)
        {
            if (string.IsNullOrEmpty(AccessToken))
                GetAccessToken();

            string targetUrl = string.Format(CultureInfo.InvariantCulture, SUBSCRIPTION_DEL_SINGLE_URL_FMT, ApplicationID, AccessToken, subscriptionType);
            string result = Fetcher.Delete(targetUrl);
            JToken token = Fetcher.FromJsonText(result);
            if (token.HasValues)
            {
                JToken error = token["error"];
                if (error != null)
                    throw ExceptionParser.Parse(error);
            }
        }
#endif

        /// <summary>
        /// Asynchronously clears a subscription of a specified type.
        /// </summary>
        /// <param name="subscriptionType">Can be a Graph API entity type name, or one of the special subscription types
        /// (known currently as <c>"permissions"</c> or <c>"errors"</c>).  This parameter is not validated or URL-encoded.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null" />.</param>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public void DeleteSubscriptionAsync(string subscriptionType, Action callback = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region ClearSubscriptions overloads
#if !SILVERLIGHT
        /// <summary>
        /// Clears all subscriptions.
        /// </summary>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        public void ClearSubscriptions()
        {
            if (string.IsNullOrEmpty(AccessToken))
                GetAccessToken();

            string targetUrl = string.Format(CultureInfo.InvariantCulture, SUBSCRIPTION_REG_URL_FMT, ApplicationID, AccessToken);
            targetUrl += "&method=delete";
            string result = Fetcher.Post(targetUrl, "");
            JToken token = Fetcher.FromJsonText(result);
            if (token.HasValues)
            {
                JToken error = token["error"];
                if (error != null)
                    throw ExceptionParser.Parse(error);
            }
        }
#endif

        /// <summary>
        /// Asynchronously clears all subscriptions.
        /// </summary>
        /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-Time Updates - Facebook Developers Documentation</seealso>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null"/>.</param>
        public void ClearSubscriptionsAsync(Action callback = null)
        {
            throw new NotSupportedException();
        }

        #endregion
        #endregion

        #region events
        /// <summary>
        /// When fired, indicates that an asynchronous call caused an exception during its callback.
        /// </summary>
        public event AsynchronousGraphExceptionEventHandler AsynchronousException;
        /// <summary>
        /// Raises the <see cref="AsynchronousException"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        internal void OnAsynchronousException(AsynchronousGraphExceptionEventArgs e)
        {
            if (AsynchronousException != null)
                AsynchronousException(this, e);
        }
        #endregion
    }

}
