using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Facebook.OpenGraph.Configuration
{
    /// <summary>
    /// Contains a number of <see cref="HandlerConfigurationElement"/> entries.
    /// </summary>
    public class HandlerConfigurationElementCollection 
        : ConfigurationElementCollection, IEnumerable<HandlerConfigurationElement>
    {
        /// <summary>
        /// Gets a <see cref="HandlerConfigurationElement"/> by its application name.
        /// </summary>
        /// <param name="key">The application name.</param>
        /// <returns>A <see cref="HandlerConfigurationElement"/> with the specified application name.</returns>
        public new HandlerConfigurationElement this[string key]
        {
            get
            {
                return BaseGet(key) as HandlerConfigurationElement;
            }
        }

        #region IEnumerable<HandlerConfigurationElement> Members

        /// <inheritdoc />
        public new IEnumerator<HandlerConfigurationElement> GetEnumerator()
        {
            foreach (object o in this)
            {
                yield return o as HandlerConfigurationElement;
            }
        }

        #endregion

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new HandlerConfigurationElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HandlerConfigurationElement)element).For;
        }
    }
}
