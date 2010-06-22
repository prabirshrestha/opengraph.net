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
    /// Represents a video uploaded by a user.
    /// </summary>
    /// <seealso href="http://developers.facebook.com/docs/reference/api/video" target="_blank">Video - Graph API Reference</seealso>
    [GraphTypeName("video")]
    [DebuggerDisplay("Video: {Title} (ID: {ID})")]
    public class Video : GraphEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Video"/>.
        /// </summary>
        public Video()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Video"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Video(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the user who uploaded this video.
        /// </summary>
        [JsonProperty("from")]
        public Friend Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the video title or caption.
        /// </summary>
        [JsonProperty("message")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the long-form HTML description of this video.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the length of this video, in seconds.
        /// </summary>
        [JsonProperty("length")]
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the time at which this video was initially uploaded.
        /// </summary>
        [JsonProperty("created_time")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the time at which this video was last updated.
        /// </summary>
        [JsonProperty("updated_time")]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets a connection to the comments about this video.
        /// </summary>
        [JsonProperty("comments")]
        public Connection<Comment> Comments { get; set; }
    }
}
