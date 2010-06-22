using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Facebook.OpenGraph.Configuration
{
    /// <summary>
    /// Contains information about OpenGraph.NET client applications.
    /// </summary>
    public class OpenGraphConfigurationSection
#if !SILVERLIGHT
        : ConfigurationSection
#endif
    {
        private static OpenGraphConfigurationSection s_instance;

#if !SILVERLIGHT
        /// <summary>
        /// Gets a collection of <see cref="ApplicationConfigurationElement"/> settings.
        /// </summary>
        [ConfigurationProperty("Applications", IsRequired = false)]
        [ConfigurationCollection(typeof(ApplicationConfigurationElement), AddItemName = "App", ClearItemsName = "Clear")]
        public ApplicationConfigurationElementCollection Applications
        {
            get { return base["Applications"] as ApplicationConfigurationElementCollection; }
            set { base["Applications"] = value; }
        }
#else
        private ApplicationConfigurationElementCollection m_appConfigs = new ApplicationConfigurationElementCollection();
        /// <summary>
        /// Gets a collection of <see cref="ApplicationConfigurationElement"/> settings.
        /// </summary>
        public ApplicationConfigurationElementCollection Applications
        {
            get { return m_appConfigs; }
            set { m_appConfigs = value; }
        }
#endif


        /// <summary>
        /// Gets the shared instance of the <see cref="OpenGraphConfigurationSection"/> provided by the configuration file, in the section named <c>&lt;OpenGraph.NET&gt;</c>.
        /// </summary>
        public static OpenGraphConfigurationSection Instance
        {
            get
            {
                if (s_instance == null)
                {
#if !SILVERLIGHT
                    s_instance = ConfigurationManager.GetSection("OpenGraph.NET") as OpenGraphConfigurationSection;
                    if (s_instance == null)
#endif
                        s_instance = new OpenGraphConfigurationSection();
                }

                return s_instance;
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Adds the specified <see cref="ApplicationConfigurationElement"/> to the configuration tree.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <remarks>
        /// <para>This method is only available in Silverlight builds.  The intent is to use configuration sections where
        /// they are available in full .NET releases, but to provide compatibility for Silverlight using the same internal
        /// framework.</para>
        /// </remarks>
        /// <example>
        /// <para>The following code is a sample of how to implement this in a Silverlight application:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        ///         private void Application_Startup(object sender, StartupEventArgs e)
        ///         {
        ///             this.RootVisual = new MainPage();
        ///
        ///             OpenGraphConfigurationSection.Instance.RegisterApplication(
        ///                 new ApplicationConfigurationElement
        ///                 {
        ///                     Name = "Flash Publish Test",
        ///                     ID = "<specify app id here>",
        ///                     Secret = "<specify secret here>",
        ///                     SubscriptionHandlerUrl = "<url to handle subscription callbacks>"
        ///                 });
        ///         }
        /// ]]>
        /// </code>
        /// </example>
        public void RegisterApplication(ApplicationConfigurationElement element)
        {
            this.Applications.Add(element);
        }

#endif
    }
}
