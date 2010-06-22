using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Facebook.OpenGraph.Metadata;
using Newtonsoft.Json.Linq;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a tag of a friend in a photo.
    /// </summary>
    [DebuggerDisplay("Friend Tag: {Name} (ID: {ID}) ({X}, {Y})")]
    public class FriendTag : Friend
    {
        /// <summary>
        /// Creates a new, empty <see cref="FriendTag"/>.
        /// </summary>
        public FriendTag() { }

        /// <summary>
        /// Creates a new <see cref="FriendTag"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public FriendTag(JToken source, GraphSession session)
            : base(source, session)
        {

        }

        /// <summary>
        /// Gets or sets the horizontal offset percentage of the tag.
        /// </summary>
        [JsonProperty("x")]
        public float X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vertical offset percentage of the tag.
        /// </summary>
        [JsonProperty("y")]
        public float Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time the tag was created.
        /// </summary>
        [JsonProperty("created_time")]
        public DateTime Created
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override User GetRealEntity()
        {
            if (string.IsNullOrEmpty(ID))
                throw new InvalidOperationException("This tag does not have an associated ID.");

            return base.GetRealEntity();
        }
    }
}
