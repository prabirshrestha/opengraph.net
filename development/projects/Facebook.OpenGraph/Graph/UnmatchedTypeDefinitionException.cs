using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Facebook.Graph
{
    /// <summary>
    /// Indicates that, when requesting an entity, the entity did not specify a type that corresponded to a known type
    /// within the OpenGraph .NET API.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class UnmatchedTypeDefinitionException : OpenGraphException
    {
        /// <summary>
        /// Creates a new <see cref="UnmatchedTypeDefinitionException"/>.
        /// </summary>
        public UnmatchedTypeDefinitionException() { }

        /// <summary>
        /// Creates a new <see cref="UnmatchedTypeDefinitionException"/>.
        /// </summary>
        /// <param name="message">The informative message.</param>
        public UnmatchedTypeDefinitionException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="UnmatchedTypeDefinitionException"/>.
        /// </summary>
        /// <param name="message">The informative message.</param>
        /// <param name="inner">The inner exception that caused this exception.</param>
        public UnmatchedTypeDefinitionException(string message, Exception inner) : base(message, inner) { }

#if !SILVERLIGHT
        /// <summary>
        /// Creates a new <see cref="UnmatchedTypeDefinitionException"/>.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The deserialization streaming context.</param>
        protected UnmatchedTypeDefinitionException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
        }
#endif
    }
}
