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
    /// Represents a Graph API Group.
    /// </summary>
    [DebuggerDisplay("Group: {Name} (ID: {ID})")]
    [GraphTypeName("group")]
    public class Group : GraphEntity, IConnectedGraphEntity<Group>, ISearchableEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Group"/>.
        /// </summary>
        public Group()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Album"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Group(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }


        /// <summary>
        /// Gets or sets information about the group's owner.
        /// </summary>
        [JsonProperty("owner")]
        public Friend Owner { get; set; }

        /// <summary>
        /// Gets or sets the group's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the group's description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a link to the group's page.
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the street address of the group.
        /// </summary>
        [JsonProperty("venue/street")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the group's city.
        /// </summary>
        [JsonProperty("venue/city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the group's state.
        /// </summary>
        [JsonProperty("venue/state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the group.
        /// </summary>
        [JsonProperty("venue/zip")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the group's country.
        /// </summary>
        [JsonProperty("venue/country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the group, if available.
        /// </summary>
        [JsonProperty("venue/latitude")]
        public float? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the group, if available.
        /// </summary>
        [JsonProperty("venue/longitude")]
        public float? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the privacy settings of the group.
        /// </summary>
        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        /// <summary>
        /// Gets or sets when this group was last updated.
        /// </summary>
        [JsonProperty("updated_time")]
        public DateTime? LastUpdated { get; set; }


        #region IConnectedGraphEntity<Group> Members

        /// <summary>
        /// When connected, gets a reference to the real Group object.
        /// </summary>
        /// <returns>A <see cref="Group"/> object with the same ID as this one.</returns>
        public Group GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<Group>();
#endif
        }

        /// <summary>
        /// When connected, asynchronously gets a reference to the real object represented by this connected entity.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of this request.</param>
        public void GetRealEntityAsync(Action<Group> callback)
        {
            base.GetRealEntityFromSourceAsync(callback);
        }
        #endregion
    }
}
