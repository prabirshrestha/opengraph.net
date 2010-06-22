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
    /// Represents a user's Education.
    /// </summary>
    /// <remarks>
    /// <para>This class is not a real "entity" in that it cannot be individually requested from OpenGraph since it
    /// does not have an ID (the <see cref="GraphEntity.ID"/> property always is <see langword="null"/>).  It is simply a structured
    /// object that is contained as part of the <see cref="User"/> class.</para>
    /// </remarks>
    [DebuggerDisplay("Education: {School.Name} {Year}")]
    public class Education : GraphEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Education"/> object.
        /// </summary>
        public Education() { }

        /// <summary>
        /// Creates a new <see cref="Education"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="token">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Education(JToken token, GraphSession session)
            : base(token, session)
        {
            
        }

        /// <summary>
        /// Gets or sets the school at which the education took place.
        /// </summary>
        [JsonProperty(PropertyName = "school")]
        public Like School
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the year in which the education culminated.
        /// </summary>
        [JsonProperty(PropertyName = "year")]
        public Like Year
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an array of concentrations, or areas of study, that were studied during the education tenure.
        /// </summary>
        [JsonProperty(PropertyName = "concentration")]
        public Like[] Concentration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the degree that was achieved.
        /// </summary>
        [JsonProperty(PropertyName = "degree")]
        public Like Degree
        {
            get;
            set;
        }

        #region IConnectedGraphEntity<Education> Members

        /// <inheritdoc/>
        public Education GetRealEntity()
        {
#if SILVERLIGHT
            throw new NotSupportedException();
#else
            return base.GetRealEntityFromSource<Education>();
#endif
        }

        #endregion
    }
}
