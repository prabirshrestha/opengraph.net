using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.Graph;

namespace Facebook.OpenGraph.Metadata
{
    /// <summary>
    /// When applied to a property of a class deriving from <see cref="GraphEntity"/>, enables automatic deserialization of a JSON object served by the OpenGraph server API.
    /// </summary>
    /// <remarks>
    /// <para>The following types of properties can be deserialized automatically:</para>
    /// <list type="bullet">
    ///     <item><see cref="Connection{TEntity}">Connection(Of TEntity)</see> - automatically creates an appropriate delay-loaded Connection to another entity type.</item>
    ///     <item><see cref="GraphEntity">OpenGraphEntity</see> or derived class (and arrays of these) - automatically creates an appropriate related object.</item>
    ///     <item><see cref="System.String">String</see> - any string property can be automatically deserialized to the appropriate value or <see langword="null"/>.</item>
    ///     <item><see cref="System.DateTime">DateTime</see> (or the <see cref="System.Nullable{T}">Nullable equivalent</see>) - a Date property can be automatically deserialized into the appropriate UTC value.</item>
    ///     <item><see langword="int">Integers</see> (or the <see cref="System.Nullable{T}">Nullable equivalent</see>) - an integer property can be automatically deserialized into the appropriate value.</item>
    /// </list>
    /// <para>Serialization is performed by dynamic methods generated internally by the JsonObjectManager class, which is generally much faster than other ways of performing the serialization, but has the side 
    /// effect of requiring code base updates to manage new types.  </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class JsonPropertyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see>JsonPropertyAttribute</see>.
        /// </summary>
        public JsonPropertyAttribute()
        {
            ShouldDefaultIfNull = true;
        }

        /// <summary>
        /// Creates a new <see cref="JsonPropertyAttribute"/> with the specified property name.
        /// </summary>
        /// <param name="propertyName">The name of the JSON object property.</param>
        public JsonPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the name of the JSON property to be deserialized.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets whether the parser should render the default value of this property if it comes back from the server as <see langword="null"/> or not included.  This property only applies
        /// to value-typed, non-nullable properties.  The default value of this property is <see langword="true"/>.
        /// </summary>
        public bool ShouldDefaultIfNull { get; set; }
    }
}
