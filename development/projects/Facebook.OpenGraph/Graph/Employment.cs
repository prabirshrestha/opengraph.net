using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Facebook.OpenGraph.Metadata;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a user's employment.
    /// </summary>
    /// <remarks>
    /// <para>This class is not a real "entity" in that it cannot be individually requested from OpenGraph since it
    /// does not have an ID (the <see cref="GraphEntity.ID"/> property always is <see langword="null"/>).  It is simply a structured
    /// object that is contained as part of the <see cref="User"/> class.  However, you can attempt to retrieve the 
    /// ID of the <see cref="Employer"/> property object and the <see cref="Position"/> object.</para>
    /// </remarks>
    [DebuggerDisplay("Employment: {Position.Name} at {Employer.Name}")]
    public class Employment : GraphEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="Employment"/> object.
        /// </summary>
        public Employment() { }

        /// <summary>
        /// Creates a new <see cref="Employment"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="token">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public Employment(JToken token, GraphSession session)
            : base(token, session)
        {

        }

        /// <summary>
        /// Gets or sets a reference to the user's employer.
        /// </summary>
        [JsonProperty("employer")]
        public Like Employer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a reference to the user's position.
        /// </summary>
        [JsonProperty("position")]
        public Like Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user-entered representation of their start date.
        /// </summary>
        /// <remarks><para>This property may return <see langword="null"/> if the user did not specify a start date.
        /// </para></remarks>
        [JsonProperty("start_date")]
        public string StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user-entered representation of their end date.
        /// </summary>
        /// <remarks><para>This property may return <see langword="null"/> if the user did not specify an end date.
        /// </para></remarks>
        [JsonProperty("end_date")]
        public string EndDate { get; set; }
    }
}
