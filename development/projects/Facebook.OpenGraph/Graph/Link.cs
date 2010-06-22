using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Facebook.OpenGraph.Metadata;
using System.Diagnostics;
using Facebook.Graph.Util;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a link shared on a user's wall.
    /// </summary>
    [GraphTypeName("link")]
    [DebuggerDisplay("Link: {Link} (ID: {ID})")]
    public class Link : GraphEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Link"/>.
        /// </summary>
        public Link()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Link"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Link(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the person who authored this link post.
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
        /// Gets or sets the actual link that was posted.
        /// </summary>
        [JsonProperty("link")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the time at which this link was posted.
        /// </summary>
        [JsonProperty(PropertyName = "updated_time")]
        public DateTime Posted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection link to comments on this link posting.
        /// </summary>
        [JsonProperty(PropertyName = "comments")]
        public Connection<Comment> Comments
        {
            get;
            set;
        }
    }
}
