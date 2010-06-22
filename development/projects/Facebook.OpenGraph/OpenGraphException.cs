using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Facebook
{
    /// <summary>
    /// Represents an error that occurred within a Graph API request.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class OpenGraphException : Exception
    {
        /// <summary>
        /// Creates a new, empty <see cref="OpenGraphException"/>.
        /// </summary>
        public OpenGraphException() : base() { }

        /// <summary>
        /// Creates a new <see cref="OpenGraphException"/> with the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public OpenGraphException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="OpenGraphException"/> with the specified message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception that caused this exception.</param>
        public OpenGraphException(string message, Exception inner) : base(message, inner) { }

#if !SILVERLIGHT
        /// <summary>
        /// Creates a new <see cref="OpenGraphException"/> by deserialization.
        /// </summary>
        /// <param name="info">The serialization data store.</param>
        /// <param name="context">The serialization context.</param>
        protected OpenGraphException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
