using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Facebook.Graph
{
    /// <summary>
    /// Provides information about the objects that the application has registered for real-time updates.
    /// <see langword="sealed"/>
    /// </summary>
    /// <remarks>
    /// <para>You may request an array of these objects by calling 
    /// <see cref="RealTimeUpdateManager.GetRegisteredSubscriptions()"/>.</para>
    /// </remarks>
    [DebuggerDisplay("Subscription Registration: {GraphTypeName} calls back to {CallbackUrl}.  Active: {IsActive}")]
    public sealed class SubscriptionRegistration
    {
        internal SubscriptionRegistration(JToken source)
        {
            this.GraphTypeName = (string)source["object"];
            this.CallbackUrl = (string)source["callback_url"];
            this.IsActive = (bool)source["active"];

            JArray fields = (JArray)source["fields"];
            string[] fieldsList = new string[fields.Count];
            for (int i = 0; i < fields.Count; i++)
            {
                fieldsList[i] = (string)fields[i];
            }
            this.GraphFieldsList = new ReadOnlyCollection<string>(fieldsList);
        }

        /// <summary>
        /// Gets the Graph API type name of the object being registered.  In addition to normal entity types, this property
        /// may also specify <c>"permissions"</c> and <c>"errors"</c>.
        /// </summary>
        public string GraphTypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the callback URL registered for the update postings.
        /// </summary>
        public string CallbackUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a collection of strings listing the fields that are being monitored.
        /// </summary>
        public ReadOnlyCollection<string> GraphFieldsList
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether the subscription is active.
        /// </summary>
        public bool IsActive
        {
            get;
            private set;
        }
    }
}
