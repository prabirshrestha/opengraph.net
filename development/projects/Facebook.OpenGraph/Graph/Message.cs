using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Facebook.Graph.Util;
using Facebook.OpenGraph.Metadata;
using System.Diagnostics;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a Facebook private message.
    /// </summary>
    [DebuggerDisplay("Inbox Message from {Author.Name}: {Subject}")]
    public class Message : GraphEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Message"/>.
        /// </summary>
        public Message()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Message"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Message(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets a reference to the author of this thread.
        /// </summary>
        [JsonProperty("from")]
        public Friend Author { get; set; }

        /// <summary>
        /// Gets or sets the recipient list.
        /// </summary>
        [JsonProperty("to/data")]
        public Friend[] Recipients { get; set; }

        /// <summary>
        /// Gets or sets the subject of this inbox thread.
        /// </summary>
        [JsonProperty("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the latest message in this inbox thread.
        /// </summary>
        [JsonProperty("message")]
        public string MessageText { get; set; }

        /// <summary>
        /// Gets or sets the last update time of this inbox thread.
        /// </summary>
        [JsonProperty("updated_time")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets a connection to the previous message history associated with this inbox message.
        /// </summary>
        [JsonProperty("comments")]
        public Connection<Comment> History { get; set; }
    }
}
