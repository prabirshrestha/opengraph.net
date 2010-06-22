using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Facebook.OpenGraph.Metadata;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using Facebook.Graph.Util;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a Graph photo album.
    /// </summary>
    /// <seealso href="http://developers.facebook.com/docs/reference/api/album" target="_blank">Album - Graph API 
    /// Reference</seealso>
    [DebuggerDisplay("Album: {Title} (ID: {ID}) - {Count} photo(s)")]
    [GraphTypeName("album")]
    public class Album : GraphEntity, IConnectedGraphEntity<Album>
    {
        /// <summary>
        /// Creates a new <see cref="Album"/>.
        /// </summary>
        public Album()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Album"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Album(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        internal override Dictionary<string, string> PreparePublishParameters()
        {
            var dict = base.PreparePublishParameters();
            
            string title = Title;
            if (!string.IsNullOrEmpty(title))
                dict["name"] = title;
            else
                throw new InvalidOperationException("Cannot serialize this album: its Title is not set and is required.");

            string desc = Description;
            if (!string.IsNullOrEmpty(desc)) dict["description"] = desc;

            string loc = Location;
            if (!string.IsNullOrEmpty(loc)) dict["location"] = loc;


            return dict;
        }

        #region properties
        /// <summary>
        /// Gets or sets an object representing the author.
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public Friend Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title (or name) of the album.
        /// </summary>
        /// <remarks>
        /// <para>This property may return <see langword="null"/>.  In this case, it has been observed that the album
        /// is the user's Profile Pictures album.</para>
        /// </remarks>
        [JsonProperty(PropertyName = "name")]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Descriptive text about the album.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location of the album.
        /// </summary>
        /// <remarks>
        /// <para>Although this property is specified in the API reference at http://developers.facebook.com/docs/reference/api/album, 
        /// experimentation suggests that this property is not always included.</para>
        /// </remarks>
        [JsonProperty("location")]
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// A URL link to the album.
        /// </summary>
        [JsonProperty("link")]
        public string Link
        {
            get;
            set;
        }

        /// <summary>
        /// The number of photos in the album.
        /// </summary>
        [JsonProperty("count")]
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// The date and time when the album was created.
        /// </summary>
        [JsonProperty("created_time")]
        public DateTime Created
        {
            get;
            set;
        }

        /// <summary>
        /// The date and time when the album was last updated.
        /// </summary>
        [JsonProperty("updated_time")]
        public DateTime Updated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the photos in the album.
        /// </summary>
        [JsonProperty("photos")]
        public Connection<Photo> Photos
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a connection to the comments in the album.
        /// </summary>
        [JsonProperty("comments")]
        public Connection<Comment> Comments
        {
            get;
            set;
        }
        #endregion

        #region IConnectedGraphEntity<Album> Members

        /// <inheritdoc />
        public Album GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<Album>();
#endif
        }

        /// <inheritdoc />
        public void GetRealEntityAsync(Action<Album> callback)
        {
            base.GetRealEntityFromSourceAsync(callback);
        }

        #endregion

        #region actions
#if !SILVERLIGHT
        /// <summary>
        /// Adds a photo to the current album.
        /// </summary>
        /// <param name="title">An optional name of hte photo.</param>
        /// <param name="pathToFile">The relative or absolute path of the file to upload.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pathToFile"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="pathToFile"/> does not point to a file.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server API rejects the request.</exception>
        /// <returns>The photo that was uploaded.</returns>
        public Photo AddPhoto(string title, string pathToFile)
        {
            if (string.IsNullOrEmpty(pathToFile))
                throw new ArgumentNullException("pathToFile");

            if (!File.Exists(pathToFile))
                throw new FileNotFoundException("The specified file did not exist.");

            using (FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return AddPhoto(title, Path.GetFileName(pathToFile), fs);
            }
        }

        /// <summary>
        /// Adds a photo to the current album.
        /// </summary>
        /// <param name="title">An optional name of the photo.</param>
        /// <param name="fileName">The name of the file that should be sent to the server.</param>
        /// <param name="contents">A <see cref="Stream"/> containing the file's contents.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileName"/> or <paramref name="contents"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="contents"/> is a stream that does not support seeking or reading.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server API rejects the request.</exception>
        /// <returns>The photo that was uploaded.</returns>
        public Photo AddPhoto(string title, string fileName, Stream contents)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (contents == null)
                throw new ArgumentNullException("contents");
            if (!contents.CanRead || !contents.CanSeek)
                throw new InvalidOperationException("Must be able to seek and read the input stream.");

            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/photos?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            Dictionary<string, string> postData = new Dictionary<string, string> { { "message", title ?? string.Empty } };
            string result = Fetcher.PostWithFile(url, postData, fileName, contents);
            JToken val = Fetcher.FromJsonText(result);
            if (val.HasValues && val["error"] != null)
                throw ExceptionParser.Parse(val["error"]);

            return Session.Request<Photo>(val["id"].ToString());
        }
#endif

        /// <summary>
        /// Adds a photo to the current album.
        /// </summary>
        /// <param name="title">An optional name of the photo.</param>
        /// <param name="fileName">The name of the file that should be sent to the server.</param>
        /// <param name="contents">A <see cref="Stream"/> containing the file's contents.</param>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileName"/> or <paramref name="contents"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="contents"/> is a stream that does not support seeking or reading.</exception>
        /// <exception cref="OpenGraphException">Thrown if the server API rejects the request.</exception>
        /// <exception cref="NotSupportedException">Thrown in all cases because this method is not yet implemented.</exception>
        public void AddPhotoAsync(string title, string fileName, Stream contents, Action<Photo> callback)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
