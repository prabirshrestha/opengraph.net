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
    /// Represents a comment posted on a post, note, or other object.
    /// </summary>
    [DebuggerDisplay("Comment from {Author.Name}")]
    public class Comment : GraphEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Comment"/>.
        /// </summary>
        public Comment()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Comment"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Comment(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets a reference to the person who posted the comment.
        /// </summary>
        [JsonProperty("from")]
        public Friend Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text content of the comment.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the time at which the comment was posted.
        /// </summary>
        [JsonProperty("created_time")]
        public DateTime Posted { get; set; }
    }
}
