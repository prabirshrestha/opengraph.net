using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Facebook.OpenGraph.Configuration
{
    /// <summary>
    /// Contains the configuration information for a web-based handler or controller.
    /// </summary>
    public class HandlerConfigurationElement 
        : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name of the application to which this handler corresponds.
        /// </summary>
        [ConfigurationProperty("For", IsRequired = true, IsKey = true)]
        public string For
        {
            get { return base["For"] as string; }
            set { base["For"] = value; }
        }

        /// <summary>
        /// Gets or sets the full URI of this handler, which will be used as a callback mechanism.
        /// </summary>
        [ConfigurationProperty("FullUri", IsRequired = true)]
        public string FullUri
        {
            get { return base["FullUri"] as string; }
            set { base["FullUri"] = value; }
        }
    }
}
