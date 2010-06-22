using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.OpenGraph.Metadata;
using Facebook.Graph.Util;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a basic OpenGraph object.
    /// </summary>
    /// <remarks>
    /// <para>This class cannot be instantiated directly.</para>
    /// </remarks>
    public class GraphEntity
    {
        private const string FACEBOOK_OPEN_GRAPH_URI = "https://graph.facebook.com/";
        private JToken m_source;

        /// <summary>
        /// Creates a new, empty <see cref="GraphEntity"/>.
        /// </summary>
        protected GraphEntity() { }

        /// <summary>
        /// Creates a new <see cref="GraphEntity"/> using the specified source token and session.
        /// </summary>
        /// <param name="source">The source JSON token.</param>
        /// <param name="session">The source session.</param>
        protected GraphEntity(JToken source, GraphSession session)
        {
            m_source = source;
            this.Session = session;

            DynamicJsonManager.PopulateProperties(source, session, this);
            //JsonObjectManager.PopulateProperties(source, session, this);
        }

        /// <summary>
        /// Gets or sets the ID of the entity.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whether this object is a connected object; that is, whether this object was retrieved via metadata from
        /// a parent object, and consequently, does not have set connections itself.
        /// </summary>
        public bool IsConnection
        {
            get;
            internal set;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Used in derived classes to get a complete entity based on this entity's ID.
        /// </summary>
        /// <typeparam name="TEntity">The type to apply to the new entity.</typeparam>
        /// <returns>The complete entity.</returns>
        protected virtual TEntity GetRealEntityFromSource<TEntity>()
            where TEntity : GraphEntity
        {
            if (Session == null)
                throw new InvalidOperationException("No session is associated with this entity.");

            return Session.Request<TEntity>(ID, true);
        }
#endif

        /// <summary>
        /// Used in derived classes to asynchronously get a complete entity based on this entity's ID.
        /// </summary>
        /// <typeparam name="TEntity">The type to apply to the new entity.  This type parameter must be derived from 
        /// <see cref="GraphEntity"/>.</typeparam>
        /// <param name="callback">The method to call upon completion of this method.</param>
        protected virtual void GetRealEntityFromSourceAsync<TEntity>(Action<TEntity> callback)
            where TEntity : GraphEntity
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (Session == null)
                throw new InvalidOperationException("No session is associated with this entity.");

            Session.RequestAsync(ID, (TEntity e) => { callback(e); });
        }

        /// <summary>
        /// Gets or sets the session associated with this object.
        /// </summary>
        public GraphSession Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the base URI of the object represented by this instance.
        /// </summary>
        public Uri BaseUri
        {
            get
            {
                return new Uri(FACEBOOK_OPEN_GRAPH_URI + ID);
            }
        }

        /// <summary>
        /// Allows an object to be serialized back to JSON format.
        /// </summary>
        /// <returns>The root object being serialized.</returns>
        [Obsolete("The Serialize method is considered to be obsolete because it exposes internal implementation details to user code, and does not utilize the same POST-supporting semantics required for publishing anyway.  It will be removed in version 1.0.", false)]
        public virtual JObject Serialize()
        {
            JObject obj = new JObject();
            obj["id"] = ID;

            return obj;
        }

        internal virtual Dictionary<string, string> PreparePublishParameters()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string id = ID;
            if (id != null)
                result.Add("id", id);
            return result;
        }
    }
}
