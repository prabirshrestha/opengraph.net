using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.OpenGraph.Metadata
{
    /// <summary>
    /// When applied to a class, indicates the name of the Graph server API type name for an Graph API entity 
    /// represented in this library.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GraphTypeNameAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="GraphTypeNameAttribute"/> without a name defined.
        /// </summary>
        public GraphTypeNameAttribute() { }
        /// <summary>
        /// Creates a new <see cref="GraphTypeNameAttribute"/> with the specified type name.
        /// </summary>
        /// <param name="name">The name of the API type.</param>
        public GraphTypeNameAttribute(string name) { Name = name; }

        /// <summary>
        /// Gets or sets the name of the API type used on the OpenGraph server for a specific represented type.
        /// </summary>
        public string Name { get; set; }
    }
}
