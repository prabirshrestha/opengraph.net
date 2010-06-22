using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Facebook.Graph;

namespace Facebook.OpenGraph.Configuration
{
    /// <summary>
    /// Contains information about an OpenGraph application.
    /// </summary>
    public class ApplicationConfigurationElement
#if !SILVERLIGHT
        : ConfigurationElement
#endif
    {
#if SILVERLIGHT
        private Dictionary<string, object> hashTable = new Dictionary<string,object>();
#endif

        private object Get(string key) 
        {
#if SILVERLIGHT
            object result;
            hashTable.TryGetValue(key, out result);
            return result;
#else
            return base[key];
#endif
        }

        private void Set(string key, object value)
        {
#if SILVERLIGHT
            hashTable[key] = value;
#else
            base[key] = value;
#endif
        }

        /// <summary>
        /// Gets or sets the friendly, unique name of the application.
        /// </summary>
        /// <remarks>
        /// <para>This property does not need to match the name of the application as it exists within Facebook.  It
        /// is simply a unique identifier within this configuration section.</para>
        /// </remarks>
        /// 
#if !SILVERLIGHT
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
#endif
        public string Name
        {
            get { return Get("Name") as string; }
            set { Set("Name", value); }
        }

        /// <summary>
        /// Gets or sets the Application ID, provided by Facebook.
        /// </summary>
#if !SILVERLIGHT
        [ConfigurationProperty("ID", IsRequired = true)]
#endif
        public string ID
        {
            get { return Get("ID") as string; }
            set { Set("ID", value); }
        }

        /// <summary>
        /// Gets or sets the application API key, provided by Facebook.
        /// </summary>
        /// <remarks>
        /// <para>For use of the Graph API, this value is not required.  However, the value could be used by other components and makes sense to include in the sample place as the application ID 
        /// and secret.</para>
        /// </remarks>
#if !SILVERLIGHT
        [ConfigurationProperty("ApiKey", IsRequired = false)]
#endif
        public string ApiKey
        {
            get { return Get("ApiKey") as string; }
            set { Set("ApiKey", value); }
        }

        /// <summary>
        /// Gets or sets the Application Secret, provided by Facebook.
        /// </summary>
#if !SILVERLIGHT
        [ConfigurationProperty("Secret", IsRequired = true)]
#endif
        public string Secret
        {
            get { return Get("Secret") as string; }
            set { Set("Secret", value); }
        }

        /// <summary>
        /// Gets or sets a URL that should be used when registering for object update notification subscriptions.
        /// </summary>
        /// <remarks>
        /// <para>For use of the Graph API, this value is not required.  The value is used by 
        /// the <see cref="RealTimeUpdateManager"/> class if you register for real-time update notifications.</para>
        /// </remarks>
#if !SILVERLIGHT
        [ConfigurationProperty("SubscriptionHandlerUrl", IsRequired = false)]
#endif
        public string SubscriptionHandlerUrl
        {
            get { return Get("SubscriptionHandlerUrl") as string; }
            set { Set("SubscriptionHandlerUrl", value); }
        }

        /// <summary>
        /// Gets or sets the URI of the canvas page root, if one exists.
        /// </summary>
        /// <remarks>
        /// <para>For use of the Graph API, this value is not required.  However, the value could be used by other components and makes sense to include in the sample place as the application ID 
        /// and secret.</para>
        /// </remarks>
#if !SILVERLIGHT
        [ConfigurationProperty("CanvasUri", IsRequired = false)]
#endif
        public string CanvasUri
        {
            get { return Get("CanvasUri") as string; }
            set { Set("CanvasUri", value); }
        }
    }
}
