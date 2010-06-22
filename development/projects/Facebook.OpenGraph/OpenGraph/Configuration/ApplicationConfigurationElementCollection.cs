using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Facebook.OpenGraph.Configuration
{
    /// <summary>
    /// Contains a collection of <see cref="ApplicationConfigurationElement"/> entries.
    /// </summary>
    public class ApplicationConfigurationElementCollection
#if !SILVERLIGHT
        : ConfigurationElementCollection, IEnumerable<ApplicationConfigurationElement>
#else 
        : List<ApplicationConfigurationElement>
#endif
    {
#if !SILVERLIGHT
        /// <inheritdoc/>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ApplicationConfigurationElement();
        }

        /// <inheritdoc/>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ApplicationConfigurationElement).Name;
        }
#endif

        /// <summary>
        /// Gets an <see cref="ApplicationConfigurationElement"/> by its 
        /// <see cref="ApplicationConfigurationElement.Name">application name</see>.
        /// </summary>
        /// <param name="key">The name of the application to find.</param>
        /// <returns>An <see cref="ApplicationConfigurationElement"/> with the specified application name.</returns>
        public 
#if !SILVERLIGHT
            new 
#endif
            ApplicationConfigurationElement this[string key]
        {
            get
            {
                return BaseGet(key) as ApplicationConfigurationElement;
            }
        }

#if SILVERLIGHT
        private ApplicationConfigurationElement BaseGet(string key)
        {
            return this.Where(ce => ce.Name == key).FirstOrDefault();
        }
#endif

        #region IEnumerable<ApplicationConfigurationElement> Members
#if !SILVERLIGHT
        /// <inheritdoc/>
        public new IEnumerator<ApplicationConfigurationElement> GetEnumerator()
        {
            foreach (object o in this)
            {
                yield return o as ApplicationConfigurationElement;
            }
        }
#endif
        #endregion
    }
}
