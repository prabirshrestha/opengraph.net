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
    /// Represents a Facebook Page.
    /// </summary>
    [GraphTypeName("page")]
    [DebuggerDisplay("Page: {Name} (ID: {ID})")]
    public class Page : GraphEntity, IConnectedGraphEntity<Page>, ISearchableEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Page"/>.
        /// </summary>
        public Page() { }

        /// <summary>
        /// Creates a new <see cref="Page"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Page(JToken source, GraphSession session)
            : base(source, session) { }

        /// <summary>
        /// Gets or sets the page's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the page's category.
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// Gets a connection to the Page's feed posts.
        /// </summary>
        [JsonProperty("feed")]
        public Connection<Post> Feed { get; set; }

        /// <summary>
        /// Gets or sets the photos, videos, and posts in which this page is tagged.
        /// </summary>
        [JsonProperty(PropertyName = "tagged")]
        public Connection<GraphEntity> Tagged
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the page's posted links.
        /// </summary>
        [JsonProperty("links")]
        public Connection<Link> Links
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the page's uploaded photos.
        /// </summary>
        [JsonProperty("photos")]
        public Connection<Photo> Photos { get; set; }

        /// <summary>
        /// Gets or sets a connection to the groups to which the Page belongs.
        /// </summary>
        [JsonProperty("groups")]
        public Connection<Group> Groups { get; set; }

        /// <summary>
        /// Gets or sets a connection to the photo albums the Page has uploaded.
        /// </summary>
        [JsonProperty("albums")]
        public Connection<Album> Albums { get; set; }

        /// <summary>
        /// Gets or sets a connection to the Page's status updated.
        /// </summary>
        [JsonProperty("statuses")]
        public Connection<Status> Statuses { get; set; }

        /// <summary>
        /// Gets or sets a connection to the videos the Page has uploaded.
        /// </summary>
        [JsonProperty("videos")]
        public Connection<Video> Videos { get; set; }

        /// <summary>
        /// Gets or sets a connection to the page's posted Notes.
        /// </summary>
        [JsonProperty("notes")]
        public Connection<Note> Notes { get; set; }

        /// <summary>
        /// Gets or sets a connection to the page's own posts.
        /// </summary>
        [JsonProperty("posts")]
        public Connection<Post> Posts { get; set; }
        
        /// <summary>
        /// Gets or sets a connection to the events to which the page is attending.
        /// </summary>
        [JsonProperty("events")]
        public Connection<GraphEvent> Events { get; set; }


        #region IConnectedGraphEntity<Page> Members


        /// <summary>
        /// Gets a reference to a complete and connected <see cref="Page"/> entity if this is being accessed via a
        /// connection.
        /// </summary>
        /// <returns>A complete <see cref="Page"/> based on the ID of this object.</returns>
        public Page GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<Page>();
#endif
        }

        /// <summary>
        /// When connected, asynchronously gets a reference to the real object represented by this connected entity.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        public void GetRealEntityAsync(Action<Page> callback)
        {
            base.GetRealEntityFromSourceAsync(callback);
        }

        #endregion
    }
}
