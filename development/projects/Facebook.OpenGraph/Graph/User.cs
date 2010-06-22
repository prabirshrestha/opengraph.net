using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.OpenGraph.Metadata;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Facebook.Graph.Util;
using System.Globalization;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a Graph User Profile.
    /// </summary>
    [DebuggerDisplay("User: {Name} (ID: {ID})")]
    [GraphTypeName("user")]
    public class User : GraphEntity, ISubscribableEntity, ISearchableEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="User"/>.
        /// </summary>
        public User()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="User"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="obj">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public User(JToken obj, GraphSession session)
            : base(obj, session)
        {
            
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// The user's last name.
        /// </summary>
        [JsonProperty(PropertyName = "last_name")]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// The user's full name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// A link to the user's profile.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/> based on the user's privacy settings.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "link")]
        public string Link
        {
            get;
            set;
        }

        /// <summary>
        /// The user's blurb that appears beneath their profile picture.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/> based on the user's privacy settings.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "about")]
        public string About
        {
            get;
            set;
        }

        /// <summary>
        /// The user's representation of his or her birthday.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/> based on the user's privacy settings.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "birthday")]
        public string Birthday
        {
            get;
            set;
        }

        /// <summary>
        /// A list of the user's work history from the user's profile.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/> based on the user's privacy settings.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "work")]
        public Employment[] Work
        {
            get;
            set;
        }

        /// <summary>
        /// A list of the education history from the user's profile.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/> based on the user's privacy settings.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "education")]
        public Education[] Education
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user's relationship status.
        /// </summary>
        [JsonProperty(PropertyName = "relationship_status")]
        public string RelationshipStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets information about a significant other.
        /// </summary>
        [JsonProperty(PropertyName = "significant_other")]
        public Friend SignificantOther
        {
            get;
            set;
        }

        /// <summary>
        /// The proxied or contact email address granted by the user.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/> based on the user's privacy settings.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// A link to the user's personal website.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/> based on the user's privacy settings.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "website")]
        public string Website
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user's timezone.
        /// </summary>
        [JsonProperty(PropertyName = "timezone")]
        public int Timezone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time at which the user's profile was last updated.
        /// </summary>
        [JsonProperty(PropertyName = "updated_time")]
        public DateTime LastUpdated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a reference to the user's hometown.
        /// </summary>
        [JsonProperty("hometown")]
        public Like Hometown
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a connection to the user's home page.
        /// </summary>
        [JsonProperty(PropertyName = "home")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadStream)]
        public Connection<Post> Home
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's feed.
        /// </summary>
        [JsonProperty(PropertyName = "feed")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadStream)]
        public Connection<Post> Feed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the photos, videos, and posts in which this user is tagged.
        /// </summary>
        [JsonProperty(PropertyName = "tagged")]
        [RequiresPermission(Permission = ExtendedPermissions.PhotoVideoTags, IsOptional = true)]
        public Connection<GraphEntity> Tagged
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's posts.
        /// </summary>
        [JsonProperty(PropertyName = "posts")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadStream)]
        public Connection<Post> Posts
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's friends.
        /// </summary>
        [JsonProperty(PropertyName = "friends")]
        public Connection<Friend> Friends
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's family members.
        /// </summary>
        [JsonProperty(PropertyName = "family")]
        public Connection<Friend> Family
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's preferred activities.
        /// </summary>
        [JsonProperty(PropertyName = "activities")]
        public Connection<Like> Activities
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's interests.
        /// </summary>
        [JsonProperty(PropertyName = "interests")]
        public Connection<Like> Interests
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's preferred music.
        /// </summary>
        [JsonProperty(PropertyName = "music")]
        public Connection<Like> Music
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's preferred books.
        /// </summary>
        [JsonProperty(PropertyName = "books")]
        public Connection<Like> Books
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's preferred movies.
        /// </summary>
        [JsonProperty(PropertyName = "movies")]
        public Connection<Like> Movies
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the user's preferred television series.
        /// </summary>
        [JsonProperty(PropertyName = "television")]
        public Connection<Like> Television
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the objects a user has liked.
        /// </summary>
        [JsonProperty(PropertyName = "likes")]
        [RequiresPermission(Permission = ExtendedPermissions.Likes, IsOptional = true)]
        public Connection<Like> Likes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the photos the user has posted.
        /// </summary>
        [JsonProperty(PropertyName = "photos")]
        [RequiresPermission(Permission = ExtendedPermissions.Photos, IsOptional = true)]
        public Connection<Photo> Photos
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the photo albums the user has posted.
        /// </summary>
        [JsonProperty(PropertyName = "albums")]
        [RequiresPermission(Permission = ExtendedPermissions.Photos, IsOptional = true)]
        public Connection<Album> Albums
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the videos the user has posted.
        /// </summary>
        [JsonProperty(PropertyName = "videos")]
        [RequiresPermission(Permission = ExtendedPermissions.Videos, IsOptional = true)]
        public Connection<Video> Videos
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the groups to which the user belongs.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        [RequiresPermission(Permission = ExtendedPermissions.Groups, IsOptional = true)]
        public Connection<Group> Groups
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a user's posted status messages.
        /// </summary>
        [JsonProperty(PropertyName = "statuses")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadStream)]
        public Connection<Status> Statuses
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a user's posted links.
        /// </summary>
        [JsonProperty(PropertyName = "links")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadStream)]
        public Connection<Link> Links
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a user's notes.
        /// </summary>
        [JsonProperty(PropertyName = "notes")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadStream)]
        public Connection<Note> Notes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a user's events.
        /// </summary>
        [JsonProperty(PropertyName = "events")]
        [RequiresPermission(Permission = ExtendedPermissions.Events, IsOptional = true)]
        public Connection<GraphEvent> Events
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a user's received private messages.
        /// </summary>
        [JsonProperty(PropertyName = "inbox")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadMailbox)]
        public Connection<Message> Inbox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a user's sent private messages.
        /// </summary>
        [JsonProperty(PropertyName = "outbox")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadMailbox)]
        public Connection<Message> Outbox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to a user's status updates.
        /// </summary>
        [JsonProperty(PropertyName = "updates")]
        [RequiresPermission(Permission = ExtendedPermissions.ReadMailbox)]
        public Connection<Message> Updates
        {
            get;
            set;
        }


        #region methods
        #region WriteFeed overloads
#if !SILVERLIGHT
        /// <summary>
        /// Writes a new <see cref="Post"/> to the user's feed.
        /// </summary>
        /// <param name="message">The post message.</param>
        /// <param name="pictureUrl">A URL of an optional picture associated with the post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <param name="linkUrl">A URL of an optional link to be associated with this post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <param name="linkName">The name of the optional link to be associated with this post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <param name="linkDescription">The description of the optional link associated with this post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <returns>The <see cref="Post"/> just posted to the current user's feed.</returns>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is <see langword="null"/> or empty.</exception>
        public Post WriteFeed(string message, string pictureUrl = null, string linkUrl = null, string linkName = null, string linkDescription = null)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            Session.ValidateAndRefresh();

            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/feed?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            Dictionary<string, string> par = new Dictionary<string, string>();
            par.Add("message", message);
            if (pictureUrl != null)
                par.Add("picture", pictureUrl);
            if (linkUrl != null)
            {
                par.Add("link", linkUrl);
                if (linkName != null)
                    par.Add("name", linkName);
                if (linkDescription != null)
                    par.Add("description", linkDescription);
            }

            string postContent = par.ToPostString();

            string jsonResponse = Fetcher.Post(url, postContent);
            JToken result = Fetcher.FromJsonText(jsonResponse);
            if (result.HasValues && result["error"] != null)
                throw ExceptionParser.Parse(result["error"]);

            return Session.Request<Post>(result["id"].ToString());
        }
#endif

        /// <summary>
        /// Writes a new <see cref="Post"/> to the user's feed.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon successful completion of the request.  This parameter should accept a single 
        /// <see cref="Post"/> parameter.</param>
        /// <param name="message">The post message.</param>
        /// <param name="pictureUrl">A URL of an optional picture associated with the post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <param name="linkUrl">A URL of an optional link to be associated with this post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <param name="linkName">The name of the optional link to be associated with this post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <param name="linkDescription">The description of the optional link associated with this post.  The default value of this parameter is <see langword="null"/>.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is <see langword="null"/> or empty.</exception>
        public Post WriteFeedAsync(Action<Post> callback, string message, string pictureUrl = null, string linkUrl = null, string linkName = null, string linkDescription = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region WriteLink overloads
#if !SILVERLIGHT
        /// <summary>
        /// Writes a link to the user's profile.
        /// </summary>
        /// <param name="linkUrl">The URL of the link</param>
        /// <param name="message">A descriptive message with the link.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="linkUrl"/> or <paramref name="message"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <returns>The <see cref="Link"/> just posted to the current user's profile.</returns>
        public Link WriteLink(string linkUrl, string message)
        {
            if (string.IsNullOrEmpty(linkUrl))
                throw new ArgumentNullException("linkUrl");
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            Session.ValidateAndRefresh();

            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/links?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            Dictionary<string, string> par = new Dictionary<string, string>();
            par.Add("message", message);
            par.Add("link", linkUrl);

            string postContent = par.ToPostString();

            string jsonResponse = Fetcher.Post(url, postContent);
            JToken result = Fetcher.FromJsonText(jsonResponse);
            if (result.HasValues && result["error"] != null)
                throw ExceptionParser.Parse(result["error"]);

            return Session.Request<Link>(result["id"].ToString());
        }
#endif

        /// <summary>
        /// Asynchronously writes a link to the user's profile.
        /// </summary>
        /// <param name="linkUrl">The URL of the link</param>
        /// <param name="message">A descriptive message with the link.</param>
        /// <param name="callback">A reference to the method to call upon successful completion of the request.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="linkUrl"/> or <paramref name="message"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public void WriteLinkAsync(string linkUrl, string message, Action<Link> callback)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region CreateEvent overloads
#if !SILVERLIGHT
        /// <summary>
        /// Creates a new event on behalf of this user.
        /// </summary>
        /// <param name="name">The name of the event to create.</param>
        /// <param name="starts">The time that the event starts.</param>
        /// <param name="ends">The time that the event ends.</param>
        /// <returns>The <see cref="GraphEvent"/> created on behalf of the user.</returns>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <see langword="null"/> or empty.</exception>
        public GraphEvent CreateEvent(string name, DateTime starts, DateTime ends)
        {
            const string ISO8601_FORMAT = @"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz";
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Session.ValidateAndRefresh();

            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/events?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            Dictionary<string, string> par = new Dictionary<string,string>();
            par.Add("name", name);
            par.Add("start_time", starts.ToString(ISO8601_FORMAT));
            par.Add("end_time", ends.ToString(ISO8601_FORMAT));

            string postContent = par.ToPostString();

            string jsonResponse = Fetcher.Post(url, postContent);
            JToken result = Fetcher.FromJsonText(jsonResponse);
            if (result.HasValues && result["error"] != null) throw ExceptionParser.Parse(result["error"]);

            return Session.Request<GraphEvent>(result["id"].ToString());
        }
#endif

        /// <summary>
        /// Asynchronously creates a new event on behalf of this user.
        /// </summary>
        /// <param name="name">The name of the event to create.</param>
        /// <param name="starts">The time that the event starts.</param>
        /// <param name="ends">The time that the event ends.</param>
        /// <param name="callback">A reference to the method to call upon completion of the request.</param>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        public void CreateEventAsync(string name, DateTime starts, DateTime ends, Action<GraphEvent> callback)
        {
            throw new NotSupportedException();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Creates a new event on behalf of this user.
        /// </summary>
        /// <param name="name">The name of the event to create.</param>
        /// <param name="starts">The time that the event starts.</param>
        /// <param name="ends">The time that the event ends.</param>
        /// <param name="description">The event's description.</param>
        /// <param name="privacy">The event's privacy setting.  Valid values are <c>"OPEN"</c>, <c>"CLOSED"</c>, and <c>"SECRET"</c>.</param>
        /// <returns>The <see cref="GraphEvent"/> created on behalf of the user.</returns>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <see langword="null"/> or empty.</exception>
        public GraphEvent CreateEvent(string name, string description, string privacy, DateTime starts, DateTime ends)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Session.ValidateAndRefresh();

            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/events?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            Dictionary<string, string> par = new Dictionary<string, string>();
            par.Add("name", name);
            par.Add("start_time", UnixTime.FromDateTime(starts.ToUniversalTime()).ToString());
            par.Add("end_time", UnixTime.FromDateTime(ends.ToUniversalTime()).ToString());
            par.Add("description", description);
            par.Add("privacy", privacy);

            string postContent = par.ToPostString();

            string jsonResponse = Fetcher.Post(url, postContent);
            JToken result = Fetcher.FromJsonText(jsonResponse);
            if (result.HasValues && result["error"] != null) throw ExceptionParser.Parse(result["error"]);

            return Session.Request<GraphEvent>(result["id"].ToString());
        }
#endif

        /// <summary>
        /// Asynchronously creates a new event on behalf of this user.
        /// </summary>
        /// <param name="name">The name of the event to create.</param>
        /// <param name="starts">The time that the event starts.</param>
        /// <param name="ends">The time that the event ends.</param>
        /// <param name="description">The event's description.</param>
        /// <param name="privacy">The event's privacy setting.  Valid values are <c>"OPEN"</c>, <c>"CLOSED"</c>, and <c>"SECRET"</c>.</param>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <see langword="null"/> or empty.</exception>
        public void CreateEventAsync(string name, string description, string privacy, DateTime starts, DateTime ends, Action<GraphEvent> callback)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region CreateNote overloads
#if !SILVERLIGHT
        /// <summary>
        /// Creates a <see cref="Note"/> on behalf of the user.
        /// </summary>
        /// <param name="subject">The title of the Note.</param>
        /// <param name="message">The Note's contents.</param>
        /// <returns>The <see cref="Note"/> just posted to the user's profile.</returns>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="subject"/> or <paramref name="message"/> is <see langword="null"/> or empty.</exception>
        public Note CreateNote(string subject, string message)
        {
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException("subject");
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            var content = new Dictionary<string, string> { { "subject", subject }, { "message", message } };

            return Session.PostToConnection<Note>(ID, "notes", content.ToPostString());
        }
#endif

        /// <summary>
        /// Asynchronously creates a <see cref="Note"/> on behalf of the user.
        /// </summary>
        /// <param name="subject">The title of the Note.</param>
        /// <param name="message">The Note's contents.</param>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="subject"/> or <paramref name="message"/> is <see langword="null"/> or empty.</exception>
        public void CreateNoteAsync(string subject, string message, Action<Note> callback)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region CreateAlbum overloads
#if !SILVERLIGHT
        /// <summary>
        /// Creates a new photo album on behalf of the user.
        /// </summary>
        /// <param name="title">The title of the album.</param>
        /// <param name="message">An initial remark about the album.</param>
        /// <returns>The <see cref="Album"/> just posted to the user's profile.</returns>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="title"/> is <see langword="null"/> or empty.</exception>
        public Album CreateAlbum(string title, string message)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");

            var content = new Dictionary<string, string> { { "name", title } };
            if (!string.IsNullOrEmpty(message)) content.Add("message", message);
            
            return Session.PostToConnection<Album>(ID, "albums", content.ToPostString());
        }
#endif

        /// <summary>
        /// Creates a new photo album on behalf of the user.
        /// </summary>
        /// <param name="title">The title of the album.</param>
        /// <param name="message">An initial remark about the album.</param>
        /// <param name="callback">A reference to the method to call upon successful completion of this request.</param>
        /// <exception cref="NotSupportedException">Thrown immediately every time because this API is not yet implemented.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server fails to perform the request.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="title"/> is <see langword="null"/> or empty.</exception>
        public void CreateAlbumAsync(string title, string message, Action<Album> callback)
        {
            throw new NotSupportedException();
        }
        #endregion
        #endregion
    }
}
