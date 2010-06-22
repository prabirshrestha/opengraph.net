using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Facebook.OpenGraph.Metadata;
using System.Diagnostics;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a photo uploaded by a <see cref="User"/> or <see cref="Page"/>
    /// </summary>
    /// <seealso href="http://developers.facebook.com/docs/reference/api/photo" target="_blank">Photo - OpenGraph API
    /// Reference</seealso>
    [GraphTypeName("photo")]
    [DebuggerDisplay("Photo: {Title} (ID: {ID})")]
    public class Photo : GraphEntity, IConnectedGraphEntity<Photo>
    {
        /// <summary>
        /// Creates a new, empty <see cref="Photo"/>.
        /// </summary>
        public Photo()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Photo"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Photo(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        internal override Dictionary<string, string> PreparePublishParameters()
        {
            var dict = base.PreparePublishParameters();
            dict.Add("name", Title);
            return dict;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the user who posted the photo.
        /// </summary>
        [JsonProperty("from")]
        public Friend Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an array of users who have been tagged in the photo.
        /// </summary>
        [JsonProperty("tags/data")]
        public FriendTag[] Tags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title of the photo.
        /// </summary>
        [JsonProperty("name")]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a URL pointing to the thumbnail image of the photo (as it is shown in albums).
        /// </summary>
        [JsonProperty("picture")]
        public string ThumbnailUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL pointing to the full-sized image of the photo.
        /// </summary>
        [JsonProperty("source")]
        public string FullSizeUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the photo, in pixels.
        /// </summary>
        [JsonProperty("height")]
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the photo, in pixels.
        /// </summary>
        [JsonProperty("width")]
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a URL to the photo on Facebook.
        /// </summary>
        [JsonProperty("link")]
        public string LinkUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time the photo was first uploaded.
        /// </summary>
        [JsonProperty("created_time")]
        public DateTime Created
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last time the photo was updated.
        /// </summary>
        [JsonProperty("updated_time")]
        public DateTime Updated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a list of comments associated with the photo.
        /// </summary>
        [JsonProperty("comments")]
        public Connection<Comment> Comments
        {
            get;
            set;
        }
        #endregion

        #region IConnectedGraphEntity<Photo> Members

        /// <inheritdoc/>
        public Photo GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<Photo>();
#endif
        }

        /// <summary>
        /// When connected, asynchronously gets a reference to the real object represented by this connected entity.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        public void GetRealEntityAsync(Action<Photo> callback)
        {
            base.GetRealEntityFromSourceAsync(callback);
        }

        #endregion
    }
}
