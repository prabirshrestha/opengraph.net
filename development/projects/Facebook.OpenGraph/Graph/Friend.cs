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
    /// Represents a user's friend, connected to a real <see cref="User"/> profile.
    /// </summary>
    [DebuggerDisplay("Friend: {Name} (ID: {ID})")]
    public class Friend : GraphEntity, IConnectedGraphEntity<User>
    {
        /// <summary>
        /// Creates a new, empty <see cref="Friend"/>.
        /// </summary>
        public Friend()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Friend"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Friend(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the friend's name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }


        #region IConnectedGraphEntity<User> Members
        /// <summary>
        /// Gets the <see cref="User"/> represented by this friend.
        /// </summary>
        /// <returns>A <see cref="User"/> with the ID represented by <see cref="GraphEntity.ID"/>.</returns>
        public virtual User GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<User>();
#endif
        }

        /// <summary>
        /// Asynchronously gets the <see cref="User"/> represented by this Friend.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        public virtual void GetRealEntityAsync(Action<User> callback)
        {
            base.GetRealEntityFromSourceAsync(callback);
        }

        #endregion
    }
}
