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
    /// Represents a connection that a user made in order to signify that the user liked another object.
    /// </summary>
    /// <remarks>
    /// <para>Likes do not have IDs.  In order to delete a like, you must call <see cref="GraphSession.DeleteLike(string)"/> on behalf of a user.</para>
    /// </remarks>
    [DebuggerDisplay("Like: {Name} (ID: {ID})")]
    public class Like : GraphEntity, IConnectedGraphEntity<Page>
    {
        /// <summary>
        /// Creates a new, empty <see cref="Like"/>.
        /// </summary>
        public Like()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Like"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Like(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the name of the liked entity.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the category of the liked entity.
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public string Category
        {
            get;
            set;
        }

        #region IConnectedGraphEntity<Page> Members

        /// <summary>
        /// Gets a <see cref="Page"/> connected to the entity.
        /// </summary>
        /// <returns>The <see cref="Page"/> with the ID corresponding to this Like's <see cref="GraphEntity.ID"/>.</returns>
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
