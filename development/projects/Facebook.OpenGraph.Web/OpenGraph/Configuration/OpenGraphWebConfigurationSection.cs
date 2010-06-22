using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Facebook.OpenGraph.Configuration
{
    /// <summary>
    /// Contains the configuration data required for an OpenGraph web application.
    /// </summary>
    public class OpenGraphWebConfigurationSection
        : ConfigurationSection
    {
        private static OpenGraphWebConfigurationSection s_instance;

        /// <summary>
        /// Gets the shared instance of the <see cref="OpenGraphConfigurationSection"/> provided by the configuration file, in the section named <c>&lt;OpenGraph.NET-Web&gt;</c>.
        /// </summary>
        public static OpenGraphWebConfigurationSection Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = ConfigurationManager.GetSection("OpenGraph.NET-Web") as OpenGraphWebConfigurationSection;
                    if (s_instance == null)
                        s_instance = new OpenGraphWebConfigurationSection();
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="HandlerConfigurationElement"/> entries.
        /// </summary>
        [ConfigurationProperty("Handlers", IsRequired = true)]
        [ConfigurationCollection(typeof(HandlerConfigurationElement), AddItemName = "Handler", RemoveItemName = "RemoveHandler")]
        public HandlerConfigurationElementCollection Handlers
        {
            get { return base["Handlers"] as HandlerConfigurationElementCollection; }
        }
    }
}
