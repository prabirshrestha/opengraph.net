using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.OpenGraph.Metadata;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Facebook.Graph.Util;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a status message on a user's wall.
    /// </summary>
    [DebuggerDisplay("Status: {Author.Name} - {Message}")]
    [GraphTypeName("status")]
    public class Status : GraphEntity, IConnectedGraphEntity<Status>
    {
        /// <summary>
        /// Creates a new, empty <see cref="Status"/>.
        /// </summary>
        public Status()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Status"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Status(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the person who authored this status update.
        /// </summary>
        [JsonProperty("from")]
        public Friend Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the message.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which this status was posted.
        /// </summary>
        [JsonProperty(PropertyName = "updated_time")]
        public DateTime Posted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection link to comments on this status update.
        /// </summary>
        [JsonProperty(PropertyName = "comments")]
        public Connection<Comment> Comments
        {
            get;
            set;
        }

        #region IConnectedGraphEntity<Status> Members

        /// <inheritdoc/>
        public Status GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<Status>();
#endif
        }

        /// <summary>
        /// When connected, asynchronously gets a reference to the real object represented by this connected entity.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        public void GetRealEntityAsync(Action<Status> callback)
        {
            base.GetRealEntityFromSourceAsync(callback);
        }

        #endregion
    }
}
