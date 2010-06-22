using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.OpenGraph.Configuration;
using System.Configuration;
using Facebook.Graph.Util;
using System.Net;
using System.IO;
using System.Globalization;
using System.Security.Permissions;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Facebook.Graph
{
    /// <summary>
    /// Contains the core functionality required for accessing data over the OpenGraph API.
    /// </summary>
    /// <remarks>
    /// <para>This class cannot be instantiated directly.  Instead, create an 
    /// <see cref="GraphDesktopSession"/> or <see cref="GraphAuthenticatedSession"/>.</para>
    /// </remarks>
    public abstract class GraphSession
    {
        private const string USER_AGENT = "OpenGraph.NET 0.9 / http://robpaveza.net/opengraph.net/";

        #region constructors
        /// <summary>
        /// Initializes a new <see>OpenGraphSession</see> using configuration data.
        /// </summary>
        /// <param name="applicationConfigurationName">The name of the application configuration element found
        /// in the application or web configuration file.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationConfigurationName"/> is 
        /// null or empty.</exception>
        /// <exception cref="ConfigurationErrorsException">Thrown if the application configuration entry with the specified
        /// name could not be located in the configuration file, or if the configuration file did not specify an 
        /// OpenGraph configuration section named <c>&lt;OpenGraph.NET&gt;</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown in Silverlight applications if the configuration entry
        /// is not registered in the configuration registry.</exception>
        protected GraphSession(string applicationConfigurationName)
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
        /// Initializes a new <see>OpenGraphSession</see>.
        /// </summary>
        /// <param name="applicationID">The application ID, provided by Facebook.</param>
        /// <param name="applicationSecret">The application secret, provided by Facebook.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationID"/> or 
        /// <paramref name="applicationSecret"/> is null or empty.</exception>
        protected GraphSession(string applicationID, string applicationSecret)
        {
            if (string.IsNullOrEmpty("applicationID"))
                throw new ArgumentNullException("applicationID");

            if (string.IsNullOrEmpty("applicationSecret"))
                throw new ArgumentNullException("applicationSecret");

            ApplicationID = applicationID;
            ClientSecret = applicationSecret;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the User-Agent header sent with requests made to the OpenGraph server API.
        /// </summary>
        public static string UserAgent
        {
            get { return USER_AGENT; }
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
        /// Gets, and in derived classes, sets, the authorization code that was requested to verify access to the 
        /// application for this session.
        /// </summary>
        public string AuthorizationCode
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets, and in derived classes, sets, the access token used to access the OpenGraph server API for this session.
        /// </summary>
        public string AccessToken
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the <see cref="ExtendedPermissions"/> required for this session.
        /// </summary>
        public ExtendedPermissions RequiredPermissions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets, and in derived classes sets, the expiration time of the access token used to access the OpenGraph server API 
        /// for this session.
        /// </summary>
        public DateTime AccessTokenExpiration
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets whether the session is currently valid for making requests to the OpenGraph server API.
        /// </summary>
        public bool IsValidSession
        {
            get
            {
                return ValidateSession();
            }
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
        /// <summary>
        /// When implemented in a derived class, prompts for a session refresh.
        /// </summary>
        /// <exception cref="InvalidSessionException">Thrown if the session refresh fails and the session is invalid.</exception>
        protected abstract void RefreshSession();

        /// <summary>
        /// When fired, indicates that an asynchronous call caused an exception during its callback.
        /// </summary>
        public event AsynchronousGraphExceptionEventHandler AsynchronousException;
        /// <summary>
        /// Raises the <see cref="AsynchronousException"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected internal virtual void OnAsynchronousException(AsynchronousGraphExceptionEventArgs e)
        {
            if (AsynchronousException != null)
                AsynchronousException(this, e);
        }


        /// <summary>
        /// Validates that the session has the necessary state to make a request.
        /// </summary>
        /// <returns><see langword="true" /> if the session is valid; otherwise <see langword="false" />.</returns>
        protected virtual bool ValidateSession()
        {
            if (string.IsNullOrEmpty(AccessToken))
                return false;

            if (DateTime.Now > AccessTokenExpiration)
                return false;

            return true;
        }

#if !SILVERLIGHT
        // TODO: Complete documenting
        /// <summary>
        /// Refreshes the 
        /// </summary>
        /// <remarks>
        /// <para>This method has the side effect of setting <see cref="ServicePointManager.Expect100Continue"/> to <see langword="true"/>.</para>
        /// </remarks>
        protected virtual void RefreshSessionWithCode(string loginRedirectUrl, bool isRedirectUrlEncoded = false)
        {
            DateTime expiration;
            string accessToken = Facebook.Graph.Util.AccessToken.Retrieve(ApplicationID, ClientSecret, loginRedirectUrl, AuthorizationCode, out expiration, isRedirectUrlEncoded);
            this.AccessToken = accessToken;
            this.AccessTokenExpiration = expiration;
        }
#endif

        #region Request overloads
#if !SILVERLIGHT
        /// <summary>
        /// Requests a Graph object by its ID.
        /// </summary>
        /// <remarks>
        /// <para>This method does not attempt to validate that the destination object matches the object being returned
        /// by the API.  Consequently, it could be possible to request an Album and fit it into a User object, although
        /// one might question the utility of such an action.  This behavior may change in a future release.</para>
        /// <para>This call is not supported in Silverlight.  Instead, use the <see cref="RequestAsync{TEntity}(string, Action{TEntity})"/> method.</para>
        /// </remarks>
        /// <typeparam name="TEntity">The type of entity to expect.  This type must derive from
        /// <see cref="GraphEntity"/>.</typeparam>
        /// <param name="entityID">The ID of the entity to request.</param>
        /// <param name="includeConnections">Whether to request additional connections metadata.  This parameter's default
        /// value is <see langword="true"/>.</param>
        /// <returns></returns>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        public virtual TEntity Request<TEntity>(string entityID, bool includeConnections = true)
            where TEntity : GraphEntity
        {
            if (!IsValidSession)
            {
                RefreshSession();
            }

            string uri = BaseUrl + entityID;
            uri += "?access_token=";
            uri += AccessToken;
            if (includeConnections)
                uri += "&metadata=1";
            var token = Fetcher.FetchToken(uri);

            return EntityFromTokenFactory<TEntity>.Create(token, this, false);
        }
#endif

        /// <summary>
        /// Asynchronously requests a Graph object by its ID.
        /// </summary>
        /// <remarks>
        /// <para>This method does not attempt to validate that the destination object matches the object being returned
        /// by the API.  Consequently, it could be possible to request an Album and fit it into a User object, although
        /// one might question the utility of such an action.  This behavior may change in a future release.</para>
        /// </remarks>
        /// <typeparam name="TEntity">The type of entity to expect.  This type must derive from
        /// <see cref="GraphEntity"/>.</typeparam>
        /// <param name="entityID">The ID of the entity to request.</param>
        /// <param name="callback">A reference to the method to invoke upon completion of the request.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is <see langword="null" /> or if 
        /// <paramref name="entityID"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        public void RequestAsync<TEntity>(string entityID, Action<TEntity> callback)
            where TEntity : GraphEntity
        {
            if (!IsValidSession)
                RefreshSession();

            string uri = string.Format("{0}{1}?access_token={2}&metadata=1", BaseUrl, entityID, AccessToken);
            Fetcher.FetchTokenAsync(uri,
                token => { callback(EntityFromTokenFactory<TEntity>.Create(token, this, false)); }, 
                token => { }); // TODO: Add error handling logic.
            // TODO: Version 1.0 - Implement 
        }

#if !SILVERLIGHT
        /// <summary>
        /// Requests a Graph object by its ID.
        /// </summary>
        /// <remarks>
        /// <para>This method attempts to determine the type of entity returned based on the <c>type</c> property returned by the server API.  If no matching type is found, 
        /// an <see cref="UnmatchedTypeDefinitionException"/> is thrown.</para>
        /// <para>This call is not supported in Silverlight.  No equivalent call exists.</para>
        /// </remarks>
        /// <param name="entityID">The ID of the entity to request.</param>
        /// <param name="includeConnections">Whether to request additional connections metadata.  This parameter's default
        /// value is <see langword="true"/>.</param>
        /// <returns></returns>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="UnmatchedTypeDefinitionException">Thrown if the object returned from the request did not 
        /// contain a known object type.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        public virtual GraphEntity Request(string entityID, bool includeConnections = true)
        {
            if (!IsValidSession)
            {
                RefreshSession();
            }

            string uri = BaseUrl + entityID;
            uri += "?access_token=";
            uri += AccessToken;
            if (includeConnections)
                uri += "&metadata=1";

            var token = Fetcher.FetchToken(uri);
            var type = (string)token["type"];
            if (string.IsNullOrEmpty(type))
                throw new UnmatchedTypeDefinitionException("Retrieved object did not specify a type.");

            GraphEntity result = EntityFromTokenFactory.Create(type, token, this, false);
            if (result == null)
                throw new UnmatchedTypeDefinitionException("Specified type '" + type + "' was not found.");

            return result;
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Requests the currently-logged-in user's profile.
        /// </summary>
        /// <remarks>
        /// <para>This call is not supported in Silverlight.  Instead, use the <see cref="GetMeAsync(Action{User})"/> method.</para>
        /// </remarks>
        /// <returns>A <see cref="User"/> containing the currently-logged-in user's profile.</returns>
        /// <exception cref="InvalidSessionException">Thrown if an established session has expired.</exception>
        /// <exception cref="OAuthException">Thrown if a session could not be established.</exception>
        /// <exception cref="OpenGraphException">Thrown if the request is not able to be completed successfully.</exception>
        public User GetMe()
        {
            return Request<User>("me");
        }
#endif
        /// <summary>
        /// Asynchronously requests the currently-logged-in user's profile.
        /// </summary>
        /// <param name="callback">A callback that accepts a <see cref="User"/> as a parameter and does not return a value,
        /// to be executed when the request is completed.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public void GetMeAsync(Action<User> callback)
        {
            RequestAsync("me", callback);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Selects a partial or full field-set of an object.
        /// </summary>
        /// <remarks>
        /// <para>You should be able to simply pass in a simple lambda expression into this method, where the compiler
        /// will automatically infer that you want to generate an expression tree.  However, the lambda expression 
        /// must be formed with specific requirements: it must accept a single parameter, of a type deriving from
        /// <see cref="GraphEntity"/> ; and it must either return exactly 
        /// that same entity, or it must return a new anonymous type comprised only of properties of the originating
        /// entity.  See the examples for these two scenarios.</para>
        /// <para>This call is not supported in Silverlight.  Instead, use the <see cref="GetMeAsync(Action{User})"/> method.</para>
        /// </remarks>
        /// <typeparam name="TSource">The source entity type to request.  This type must be derived from <see cref="GraphEntity"/>.</typeparam>
        /// <typeparam name="TResult">The result entity type to request.</typeparam>
        /// <param name="id">The ID of the entity to request.  This type must either be the same as <typeparamref name="TSource"/>, or it should
        /// be an anonymous type.</param>
        /// <param name="fieldSelector">An expression representing a partial or full field set selection.</param>
        /// <returns>A single entity of type <typeparamref name="TResult"/> containing the requested fields.</returns>
        /// <example>
        /// <para>The following code requests a user's name, email, and friends list:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void GetUserBasicInfo(OpenGraphSession session, string userID)
        /// {
        ///     var userData = session.Request( (User u) => new { u.Name, u.Email, u.Friends }, userID);
        ///     Console.WriteLine("{0} ({1}) has {2} friend(s).", userData.Name, userData.Email, userData.Friends.Count());
        /// }
        /// ]]>
        /// </code>
        /// <para>The following code requests all of a specific user's fields:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void GetUserInfo(OpenGraphSession session, string userID)
        /// {
        ///     User data = session.Request( (User u) => u, userID);
        ///     Console.WriteLine("{0} can be emailed at {1}", data.Name, data.Email);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public TResult Request<TSource, TResult>(Expression<Func<TSource, TResult>> fieldSelector, string id)
            where TSource : GraphEntity
        {
            if (!IsValidSession)
                RefreshSession();

            string entityType = null;
            string fieldsList = GraphExpressionParser.ParseRequestExpression(fieldSelector, out entityType);

            string uri = BaseUrl + id;
            uri += "?access_token=";
            uri += AccessToken;
            uri += "&metadata=1&fields=";
            uri += fieldsList;
            var token = Fetcher.FetchToken(uri);
            TSource result = EntityFromTokenFactory<TSource>.Create(token, this, false);

            var lambda = fieldSelector.Compile();
            return lambda(result);
        }
#endif
        // TODO: Provide async example

        /// <summary>
        /// Asynchronously selects a partial or full field-set of an object.
        /// </summary>
        /// <remarks>
        /// <para>You should be able to simply pass in a simple lambda expression into this method, where the compiler
        /// will automatically infer that you want to generate an expression tree.  However, the lambda expression 
        /// must be formed with specific requirements: it must accept a single parameter, of a type deriving from
        /// <see cref="GraphEntity"/> ; and it must either return exactly 
        /// that same entity, or it must return a new anonymous type comprised only of properties of the originating
        /// entity.  See the examples for these two scenarios.</para>
        /// </remarks>
        /// <typeparam name="TSource">The source entity type to request.  This type must be derived from <see cref="GraphEntity"/>.</typeparam>
        /// <typeparam name="TResult">The result entity type to request.</typeparam>
        /// <param name="id">The ID of the entity to request.  This type must either be the same as <typeparamref name="TSource"/>, or it should
        /// be an anonymous type.</param>
        /// <param name="fieldSelector">An expression representing a partial or full field set selection.</param>
        /// <param name="callback">The callback to execute upon completion of the request.</param>
        /// <returns>A single entity of type <typeparamref name="TResult"/> containing the requested fields.</returns>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <example>
        /// <para>The following code requests a user's name, email, and friends list:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void GetUserBasicInfo(OpenGraphSession session, string userID)
        /// {
        ///     var userData = session.Request( (User u) => new { u.Name, u.Email, u.Friends }, userID);
        ///     Console.WriteLine("{0} ({1}) has {2} friend(s).", userData.Name, userData.Email, userData.Friends.Count());
        /// }
        /// ]]>
        /// </code>
        /// <para>The following code requests all of a specific user's fields:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void GetUserInfo(OpenGraphSession session, string userID)
        /// {
        ///     User data = session.Request( (User u) => u, userID);
        ///     Console.WriteLine("{0} can be emailed at {1}", data.Name, data.Email);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void RequestAsync<TSource, TResult>(Expression<Func<TSource, TResult>> fieldSelector, string id,
            Action<TResult> callback)
            where TSource : GraphEntity
        {
            // TODO: Version 1.0 - Implement, update code sample for async code
            throw new NotSupportedException();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Requests a sequence of entities, with a partial or full field-set.
        /// </summary>
        /// <typeparam name="TSource">The source entity type to request.  This type must be derived from <see cref="GraphEntity"/>.</typeparam>
        /// <typeparam name="TResult">The target entity type to populate.  This type must either be the same as <typeparamref name="TSource"/>, or it should
        /// be an anonymous type.</typeparam>
        /// <param name="fieldSelector">An expression selecting the members of the source type into the target type.</param>
        /// <param name="ids">A list of entity IDs to request.  These IDs should all be of the same type.</param>
        /// <returns>An enumeration of <typeparamref name="TResult"/> objects with the specified IDs.</returns>
        /// <example>
        /// <para>The following code gets a list of mutual friends for two users:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void FindCommonFriends(OpenGraphSession session, string user1, string user2)
        /// {
        ///     var users = session.Request( (User u) => new { u.ID, u.Friends }, user1, user2).ToArray();
        ///     var otherFriendsIDs = users[1].Friends.Select(fr => fr.ID);
        ///     var commonFriends = from fr in users[0].Friends
        ///                         where otherFriendsIDs.Contains(fr.ID)
        ///                         select fr;
        ///     
        ///     foreach (var friend in commonFriends)
        ///     {
        ///         Console.WriteLine("{0}: {1}", friend.Name, friend.ID);
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>This call is not supported in Silverlight.  The equivalent call is 
        /// <see cref="RequestAsync{TSource, TResult}(Expression{Func{TSource, TResult}}, Action{TResult}, string[])"/>.</para>
        /// </remarks>
        public IEnumerable<TResult> Request<TSource, TResult>(Expression<Func<TSource, TResult>> fieldSelector, params string[] ids)
            where TSource : GraphEntity
        {
            if (!IsValidSession)
                RefreshSession();

            string entityType = null;
            string fieldsList = GraphExpressionParser.ParseRequestExpression(fieldSelector, out entityType);

            string uri = BaseUrl + "?ids=" + string.Join(",", ids);
            uri += "&access_token=";
            uri += AccessToken;
            uri += "&metadata=1&fields=";
            uri += fieldsList;
            var token = Fetcher.FetchToken(uri);

            var lambda = fieldSelector.Compile();

            foreach (string id in ids)
            {
                JToken item = token[id];
                if (item == null) continue;

                yield return lambda(EntityFromTokenFactory<TSource>.Create(item, this, false));
            }
        }
#endif

        /// <summary>
        /// Requests a sequence of entities, with a partial or full field-set.
        /// </summary>
        /// <typeparam name="TSource">The source entity type to request.  This type must be derived from <see cref="GraphEntity"/>.</typeparam>
        /// <typeparam name="TResult">The target entity type to populate.  This type must either be the same as <typeparamref name="TSource"/>, or it should
        /// be an anonymous type.</typeparam>
        /// <param name="callback">A reference to the callback function to invoke upon completion of the request.</param>
        /// <param name="fieldSelector">An expression selecting the members of the source type into the target type.</param>
        /// <param name="ids">A list of entity IDs to request.  These IDs should all be of the same type.</param>
        /// <returns>An enumeration of <typeparamref name="TResult"/> objects with the specified IDs.</returns>
        /// <example>
        /// <para>The following code gets a list of mutual friends for two users:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void FindCommonFriends(OpenGraphSession session, string user1, string user2)
        /// {
        ///     var users = session.Request( (User u) => new { u.ID, u.Friends }, user1, user2).ToArray();
        ///     var otherFriendsIDs = users[1].Friends.Select(fr => fr.ID);
        ///     var commonFriends = from fr in users[0].Friends
        ///                         where otherFriendsIDs.Contains(fr.ID)
        ///                         select fr;
        ///     
        ///     foreach (var friend in commonFriends)
        ///     {
        ///         Console.WriteLine("{0}: {1}", friend.Name, friend.ID);
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public IEnumerable<TResult> RequestAsync<TSource, TResult>(Expression<Func<TSource, TResult>> fieldSelector, 
            Action<TResult> callback, params string[] ids)
            where TSource : GraphEntity
        {
            throw new NotSupportedException();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Requests a series of entities with a complete field set.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to request.  This type must be derived from <see cref="GraphEntity"/>.</typeparam>
        /// <param name="ids">A list of entity IDs to request.  These IDs should all be of the same type of object.</param>
        /// <returns>An enumeration of <typeparamref name="TEntity"/> objects with the specified IDs.</returns>
        /// <remarks>
        /// <para>This call is not supported in Silverlight.  The equivalent API is 
        /// <see cref="RequestAsync{TEntity}(Action{IEnumerable{TEntity}}, string[])"/>.</para>
        /// </remarks>
        public IEnumerable<TEntity> Request<TEntity>(params string[] ids)
            where TEntity : GraphEntity
        {
            if (!IsValidSession)
            {
                RefreshSession();
            }

            string uri = BaseUrl + "?ids=" + string.Join(",", ids);
            uri += "&access_token=";
            uri += AccessToken;
            uri += "&metadata=1";
            var token = Fetcher.FetchToken(uri);

            foreach (string id in ids)
            {
                JToken item = token[id];
                if (item == null)
                    continue;

                yield return EntityFromTokenFactory<TEntity>.Create(item, this, false);
            }
        }
#endif

        /// <summary>
        /// Asynchronously equests a series of entities with a complete field set.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to request.  This type must be derived from <see cref="GraphEntity"/>.</typeparam>
        /// <param name="ids">A list of entity IDs to request.  These IDs should all be of the same type of object.</param>
        /// <param name="callback">Specifies a callback to invoke upon completion of the request.</param>
        /// <returns>An enumeration of <typeparamref name="TEntity"/> objects with the specified IDs.</returns>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public void RequestAsync<TEntity>(Action<IEnumerable<TEntity>> callback, params string[] ids)
            where TEntity : GraphEntity
        {
            throw new NotSupportedException();
        }
        #endregion

        #region Delete overloads
#if !SILVERLIGHT
        /// <summary>
        /// Deletes an object with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the object to delete.</param>
        /// <exception cref="OpenGraphException">Thrown if the user does not have adequate permissions to delete the requested object.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is <see langword="null" /> or empty.</exception>
        /// <exception cref="GraphMethodException">Thrown if the server API does not support deleting the requested object.</exception>
        public void Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            if (!IsValidSession)
            {
                RefreshSession();
            }

            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}?access_token={2}&method=delete", BaseUrl, id, AccessToken);
            string jsonResponse = Fetcher.Post(url, "");
            JToken response = Fetcher.FromJsonText(jsonResponse);
            if (response.HasValues && response["error"] != null)
                throw ExceptionParser.Parse(response["error"]);
        }
#endif

        /// <summary>
        /// Asynchronously deletes an object with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the object to delete.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null"/>.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="OpenGraphException">Thrown if the user does not have adequate permissions to delete the requested object.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is <see langword="null" /> or empty.</exception>
        /// <exception cref="GraphMethodException">Thrown if the server API does not support deleting the requested object.</exception>
        public void DeleteAsync(string id, Action callback = null)
        {
            throw new NotSupportedException();
        }

        #if !SILVERLIGHT
        /// <summary>
        /// Deletes the specified object from the Graph server API.
        /// </summary>
        /// <param name="item">The entity to delete.</param>
        /// <exception cref="OpenGraphException">Thrown if the server returns an error.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="item"/> is not a real Graph entity, identified by not having a set <see cref="GraphEntity.ID">ID</see>.</exception>
        public void Delete(GraphEntity item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (string.IsNullOrEmpty(item.ID))
                throw new InvalidOperationException("Cannot delete specified object as it does not represent a real Entity.");

            Delete(item.ID);
        }
#endif

        /// <summary>
        /// Deletes the specified object from the Graph server API.
        /// </summary>
        /// <param name="item">The entity to delete.</param>
        /// <param name="callback">A reference to a method to call upon completion of the request.  This parameter may be <see langword="null"/>.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server returns an error.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="item"/> is not a real Graph entity, identified by not having a set <see cref="GraphEntity.ID">ID</see>.</exception>
        public void DeleteAsync(GraphEntity item, Action callback = null)
        {
            throw new NotSupportedException();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Deletes the current user's Like of the specified object.
        /// </summary>
        /// <param name="objectID">The ID of the object to no longer Like.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="objectID"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server returns an error.</exception>
        public void DeleteLike(string objectID)
        {
            if (string.IsNullOrEmpty(objectID))
                throw new ArgumentNullException("objectID");

            if (!IsValidSession)
                RefreshSession();

            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/likes?access_token={2}&method=delete", BaseUrl, objectID, AccessToken);
            string jsonResponse = Fetcher.Post(url, "");
            JToken response = Fetcher.FromJsonText(jsonResponse);
            if (response.HasValues && response["error"] != null)
                throw ExceptionParser.Parse(response["error"]);
        }
#endif

        /// <summary>
        /// Asynchronously deletes the current user's Like of the specified object.
        /// </summary>
        /// <param name="objectID">The ID of the object to no longer Like.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="objectID"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server returns an error.</exception>
        /// <param name="callback">A reference to a method to call upon completion of the call.  This parameter may be <see langword="null"/>.</param>
        public void DeleteLikeAsync(string objectID, Action callback = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region SearchFor/SearchAsyncFor
#if !SILVERLIGHT
        /// <summary>
        /// Allows searching for the specified text over specific type of objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity that should be searched-for.  This type must derive from <see cref="GraphEntity"/> and also be marked by 
        /// the <see cref="ISearchableEntity"/> interface.</typeparam>
        /// <param name="query">The text to search for.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/> or empty.</exception>
        /// <returns>An enumeration of entities that matched the query.</returns>
        public SearchResult<TEntity> SearchFor<TEntity>(string query)
            where TEntity : GraphEntity, ISearchableEntity
        {
            return new SearchResult<TEntity>();
        }
#endif

        /// <summary>
        /// Asynchronously searches for the specified text over a specific type of object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity that should be searched-for.  This type must derive from <see cref="GraphEntity"/> and also be marked by 
        /// the <see cref="ISearchableEntity"/> interface.</typeparam>
        /// <param name="query">The text to search for.</param>
        /// <param name="callback">A reference to a method to call upon completion of the request, that should accept a search result as a parameter.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/> or empty, or if <paramref name="callback"/> is <see langword="null"/>.</exception>
        public void SearchAsyncFor<TEntity>(string query, Action<SearchResult<TEntity>> callback)
            where TEntity : GraphEntity, ISearchableEntity
        {
            throw new NotSupportedException();
        }
        #endregion
        #endregion

        #region internal utility methods
        internal string BaseUrl
        {
            get { return "https://graph.facebook.com/"; }
        }

        internal void ValidateAndRefresh()
        {
            if (!IsValidSession)
                RefreshSession();
        }

#if !SILVERLIGHT
        internal JToken PostToConnection(string ID, string connection, string postData)
        {
            if (!IsValidSession)
                RefreshSession();

            return Fetcher.FromJsonText(Fetcher.Post(string.Format("{0}{1}/{2}?access_token={3}", BaseUrl, ID, connection, AccessToken), postData));
        }

        internal TEntity PostToConnection<TEntity>(string ID, string connection, string postData)
            where TEntity : GraphEntity
        {
            JToken result = PostToConnection(ID, connection, postData);
            if (result.HasValues)
            {
                JToken error = result["error"];
                if (error != null)
                    throw ExceptionParser.Parse(error);
            }

            string id = result["id"].ToString();
            if (id[0] == '\"')
                id = id.Substring(1, id.Length - 2);
            return Request<TEntity>(id);
        }
#endif
        internal void PostToConnectionAsync(string ID, string connection, string postData, Action<JToken> callback)
        {
            if (!IsValidSession)
                RefreshSession();

            Fetcher.PostAsync(string.Format("{0}{1}/{2}?access_token={3}", BaseUrl, ID, connection, AccessToken), postData,
                s => { callback(Fetcher.FromJsonText(s)); },
                s => { }); // TODO: Update error handler
        }

        internal void PostToConnectionAsync<TEntity>(string ID, string connection, string postData, Action<TEntity> callback)
            where TEntity : GraphEntity
        {
            PostToConnectionAsync(ID, connection, postData,
                result =>
                {
                    if (result.HasValues)
                    {
                        JToken error = result["error"];
                        if (error != null)
                        {
                            OnAsynchronousException(new AsynchronousGraphExceptionEventArgs(ExceptionParser.Parse(error)));
                            return;
                        }
                    }

                    string id = result["id"].ToString();
                    if (id[0] == '\"')
                        id = id.Substring(1, id.Length - 2);

                    RequestAsync<TEntity>(id, (TEntity item) => { callback(item); });
                });
        }
        #endregion
    }
}
