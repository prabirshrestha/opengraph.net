using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents an error that occurred because the Graph API does not support a desired activity.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GraphMethodException : OpenGraphException
    {
        /// <summary>
        /// Creates a new, default <see cref="GraphMethodException"/>.
        /// </summary>
        public GraphMethodException()
            : this("The requested method or activity is not supported.")
        {

        }

        /// <summary>
        /// Creates a new <see cref="GraphMethodException"/> with the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GraphMethodException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="GraphMethodException"/> with the specified message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception that caused this exception.</param>
        public GraphMethodException(string message, Exception inner)
            : base(message, inner)
        {

        }

#if !SILVERLIGHT
        /// <summary>
        /// Creates a new <see cref="GraphMethodException"/> by deserialization.
        /// </summary>
        /// <param name="info">The serialization data store.</param>
        /// <param name="context">The serialization context.</param>
        protected GraphMethodException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
#endif
    }
}
