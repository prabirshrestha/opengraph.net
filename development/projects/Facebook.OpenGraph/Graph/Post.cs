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
    /// Represents an individual entry in a profile feed.
    /// </summary>
    [DebuggerDisplay("Post: from {Author.Name}, \"{Message}\"")]
    [GraphTypeName("feed")]
    public class Post : GraphEntity, IConnectedGraphEntity<Post>, ISearchableEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Post"/>.
        /// </summary>
        public Post()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Post"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Post(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the user who posted the message/
        /// </summary>
        [JsonProperty("from")]
        public Friend Author
        {
            get;
            set;
        }

        // The following property is commented out because it is specified by the API documentation 
        // at http://developers.facebook.com/docs/reference/api/post but is not observed in 
        // viewing the data sent over the wire.
        //[JsonProperty("to")]
        //public Friend[] ProfileTags { get; set; }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a link to the picture included with this post, if one is available.
        /// </summary>
        [JsonProperty("picture")]
        public string Picture { get; set; }

        /// <summary>
        /// Gets or sets the link URL, if one is available.
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the name of the link, if one is available.
        /// </summary>
        [JsonProperty("name")]
        public string LinkName { get; set; }

        /// <summary>
        /// Gets or sets a caption of the link, if one is available.
        /// </summary>
        [JsonProperty("caption")]
        public string LinkCaption { get; set; }

        /// <summary>
        /// Gets or sets a description of the link, if one is available.
        /// </summary>
        [JsonProperty("description")]
        public string LinkDescription { get; set; }

        /// <summary>
        /// Gets or sets the source link attached to this post, if any is available, such as a video or Flash file.
        /// </summary>
        [JsonProperty("source")]
        public string LinkSource { get; set; }

        /// <summary>
        /// Gets or sets the URL of an icon that represents this type of post.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the name of the application used to create this post.
        /// </summary>
        [JsonProperty("attribution")]
        public string PostedWith { get; set; }

        /// <summary>
        /// Gets or sets the number of likes this post has.
        /// </summary>
        /// <remarks>
        /// <para>If this post has never been liked, this property will return <see langword="null"/>.  However, if it has
        /// been liked and then un-liked, it will be <c>0</c>.</para>
        /// </remarks>
        [JsonProperty("likes")]
        public int? Likes { get; set; }

        /// <summary>
        /// Gets or sets the time at which this post was created.
        /// </summary>
        [JsonProperty("created_time")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the time at which this post was last updated.
        /// </summary>
        [JsonProperty("updated_time")]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets a link to the connection of comments available for this post.
        /// </summary>
        [JsonProperty("comments")]
        public Connection<Comment> Comments { get; set; }

        #region IConnectedGraphEntity<Post> Members

        /// <inheritdoc/>
        public Post GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<Post>();
#endif
        }

        /// <summary>
        /// When connected, asynchronously gets a reference to the real object represented by this connected entity.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        public void GetRealEntityAsync(Action<Post> callback)
        {
            base.GetRealEntityFromSourceAsync(callback);
        }

        #endregion

#if !SILVERLIGHT
        /// <summary>
        /// Comments on the post, from the user logged-in with the current session.
        /// </summary>
        /// <param name="message">The comment message to include.</param>
        public Comment Comment(string message)
        {
            return Session.PostToConnection<Comment>(ID, "comments", "message=" + message.UrlEncode());
        }
#endif

        /// <summary>
        /// Asynchronously comments on the post, from the currently-logged-on user.
        /// </summary>
        /// <param name="message">The comment message.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.</param>
        public void CommentAsync(string message, Action<Comment> callback)
        {
            Session.PostToConnectionAsync(ID, "comments", "message=" + message.UrlEncode(), callback);
        }
        
#if !SILVERLIGHT
        /// <summary>
        /// Likes the specified post on behalf of the user logged in with the current session.
        /// </summary>
        public void Like()
        {
            JToken token = Session.PostToConnection(ID, "likes", "");
        }
#endif

        /// <summary>
        /// Asynchronously likes the specified post on behalf of the user logged in with the current session.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null"/>.</param>
        public void LikeAsync(Action callback = null)
        {
            throw new NotSupportedException();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Un-likes the specified post.
        /// </summary>
        public void UnLike()
        {
            this.Session.DeleteLike(this.ID);
        }
#endif

        /// <summary>
        /// Asynchronously un-likes the specified post on behalf of the currently-logged-in user.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of the request.  This parameter may be <see langword="null"/>.</param>
        public void UnLikeAsync(Action callback = null)
        {
            throw new NotSupportedException();
        }
    }
}
