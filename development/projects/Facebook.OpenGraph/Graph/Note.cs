using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.OpenGraph.Metadata;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a Note, analogous to a blog entry written by a user.
    /// </summary>
    [DebuggerDisplay("Note from {AuthorID}: {Subject}")]
    public class Note : GraphEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Note"/>.
        /// </summary>
        public Note()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Note"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Note(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the ID of the user who wrote the note.
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public string AuthorID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title of the note.
        /// </summary>
        [JsonProperty(PropertyName = "subject")]
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HTML content of the note.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which the note had been created.
        /// </summary>
        [JsonProperty(PropertyName = "created_time")]
        public DateTime Created
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which the note had been updated.
        /// </summary>
        [JsonProperty(PropertyName = "updated_time")]
        public DateTime Updated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to comments about the note.
        /// </summary>
        [JsonProperty(PropertyName = "comments")]
        public Connection<Comment> Comments
        {
            get;
            set;
        }
    }
}
