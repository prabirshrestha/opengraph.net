using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents an exception related to authentication.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class OAuthException : OpenGraphException
    {
        /// <summary>
        /// Creates a new <see cref="OAuthException"/> with a standard error message.
        /// </summary>
        public OAuthException() : base("An authorization error occured.") { }

        /// <summary>
        /// Creates a new <see cref="OAuthException"/>.
        /// </summary>
        /// <param name="message">The informative message.</param>
        public OAuthException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="OAuthException"/>.
        /// </summary>
        /// <param name="message">The informative message.</param>
        /// <param name="inner">The inner exception causing this exception.</param>
        public OAuthException(string message, Exception inner) : base(message, inner) { }

#if !SILVERLIGHT
        /// <summary>
        /// Creates a new <see cref="OAuthException"/>.
        /// </summary>
        /// <param name="info">Serialization data.</param>
        /// <param name="context">The deserialization streaming context.</param>
        protected OAuthException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
