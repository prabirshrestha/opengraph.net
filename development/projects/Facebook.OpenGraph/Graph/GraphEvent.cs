using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Facebook.Graph.Util;
using System.Globalization;
using Facebook.OpenGraph.Metadata;
using System.Diagnostics;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a user or page event in the Graph API.
    /// </summary>
    [DebuggerDisplay("Event: {Name} hosted by {Owner.Name}")]
    public class GraphEvent : GraphEntity, ISearchableEntity
    {
        /// <summary>
        /// Creates a new, empty <see cref="GraphEvent"/>.
        /// </summary>
        public GraphEvent()
        {

        }

        /// <summary>
        /// Creates a new <see cref="GraphEvent"/> from the specified JSON object and <see cref="GraphSession"/>.
        /// </summary>
        /// <param name="source">The source JSON object.</param>
        /// <param name="session">The session that is creating the object.</param>
        public GraphEvent(JToken source, GraphSession session)
            : base(source, session)
        {
            
        }

        #region properties
        /// <summary>
        /// Gets or sets a reference to the event owner.
        /// </summary>
        [JsonProperty("owner")]
        public Friend Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the long-form HTML description of the event.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the start time of the event.
        /// </summary>
        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the event.
        /// </summary>
        [JsonProperty("end_time")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the location for this event.
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

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
        /// Gets or sets the events privacy.
        /// </summary>
        /// <remarks>
        /// <para>Expected values should include <c>"OPEN"</c>, <c>"CLOSED"</c>, or <c>"SECRET"</c>.</para>
        /// </remarks>
        [JsonProperty("privacy")]
        public string Privacy { get; set; }
        // TODO: Version 1.0: Update this to support Privacy as an enumeration.

        /// <summary>
        /// Gets or sets the time at which this event was last updated.
        /// </summary>
        [JsonProperty("updated_time")]
        public DateTime LastUpdated { get; set; }
        #endregion

        #region connections
        /// <summary>
        /// Gets or sets a connection to the event's wall feed.
        /// </summary>
        [JsonProperty("feed")]
        public Connection<Post> Feed { get; set; }

        /// <summary>
        /// Gets or sets a connection to the list of users who have been invited to the event.
        /// </summary>
        [JsonProperty("invited")]
        public Connection<Friend> InvitedUsers { get; set; }

        /// <summary>
        /// Gets or sets a connection to the list of users who have accepted the event invitation.
        /// </summary>
        [JsonProperty("attending")]
        public Connection<Friend> UsersAttending { get; set; }

        /// <summary>
        /// Gets or sets a connection to the list of users who have replied "Maybe" to the event invitation.
        /// </summary>
        [JsonProperty("maybe")]
        public Connection<Friend> UsersMaybeAttending { get; set; }

        /// <summary>
        /// Gets or sets a connection to the list of users who have not yet replied to the event invitation.
        /// </summary>
        [JsonProperty("noreply")]
        public Connection<Friend> UsersNotYetReplied { get; set; }

        /// <summary>
        /// Gets or sets a connection to the list of users who declined the event invitation.
        /// </summary>
        [JsonProperty("declined")]
        public Connection<Friend> UsersDeclined { get; set; }

        /// <summary>
        /// Gets or sets the URL of the picture.
        /// </summary>
        [JsonProperty("metadata/connection/picture")]
        public string PictureUrl { get; set; }
        #endregion

        #region actions
#if !SILVERLIGHT
        /// <summary>
        /// RSVPs to the event on behalf of the user who is currently logged in.
        /// </summary>
        /// <remarks>
        /// <para>This method is not available in Silverlight.  The corresponding API is 
        /// <see cref="AttendAsync(Action)"/>.</para>
        /// </remarks>
        public void Attend()
        {
            Session.ValidateAndRefresh();
            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/attending?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            string jsonResult = Fetcher.Post(url, string.Empty);
            JToken result = jsonResult.AsToken();
            if (result.HasValues && result["error"] != null) throw ExceptionParser.Parse(result["error"]);
        }
#endif

        /// <summary>
        /// Asynchronously RSVPs to the event on behalf of the user who is currently logged in.
        /// </summary>
        /// <param name="callback">A callback to execute on completion of the request.</param>
        public void AttendAsync(Action callback)
        {
            throw new NotSupportedException();
        }

#if !SILVERLIGHT
        /// <summary>
        /// RSVPs "Maybe" to the event on behalf of the user who is currently logged in.
        /// </summary>
        /// <remarks>
        /// <para>This method is not available in Silverlight.  The corresponding API is 
        /// <see cref="RsvpMaybeAsync(Action)"/>.</para>
        /// </remarks>
        public void RsvpMaybe()
        {
            Session.ValidateAndRefresh();
            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/maybe?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            string jsonResult = Fetcher.Post(url, string.Empty);
            JToken result = jsonResult.AsToken();
            if (result.HasValues && result["error"] != null) throw ExceptionParser.Parse(result["error"]);
        }
#endif

        /// <summary>
        /// Asynchronously RSVPs "Maybe" to the event on behalf of the user who is currently logged in.
        /// </summary>
        /// <param name="callback">A callback to execute on completion of the request.</param>
        public void RsvpMaybeAsync(Action callback)
        {
            throw new NotSupportedException();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Declines the event on behalf of the user who is currently logged-in.
        /// </summary>
        /// <remarks>
        /// <para>This method is not available in Silverlight.  The corresponding API is 
        /// <see cref="RsvpMaybeAsync(Action)"/>.</para>
        /// </remarks>
        public void Decline()
        {
            Session.ValidateAndRefresh();
            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}/declined?access_token={2}", Session.BaseUrl, ID, Session.AccessToken);
            string jsonResult = Fetcher.Post(url, string.Empty);
            JToken result = jsonResult.AsToken();
            if (result.HasValues && result["error"] != null) throw ExceptionParser.Parse(result["error"]);
        }
#endif

        /// <summary>
        /// Asynchronously declines the event on behalf of the user who is currently logged-in.
        /// </summary>
        /// <param name="callback">A callback to execute on completion of the request.</param>
        public void DeclineAsync(Action callback)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
