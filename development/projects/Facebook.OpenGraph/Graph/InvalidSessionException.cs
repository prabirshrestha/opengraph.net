using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Facebook.Graph
{
    /// <summary>
    /// Thrown when an <see cref="GraphSession"/> attempts to retrieve an entity with an invalid session.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class InvalidSessionException
        : OAuthException
    {
        #region error messages
        private static Dictionary<InvalidSessionReason, string> DefinedErrorMessages = new Dictionary<InvalidSessionReason, string> 
        {
            { InvalidSessionReason.Unknown, "The OpenGraph session was faulty for an unknown reason." },
            { InvalidSessionReason.MissingAccessToken, "The OpenGraph session was missing the access token." },
            { InvalidSessionReason.SessionExpired, "The OpenGraph session expired." }
        };
        private static string GetReasonErrorMessage(InvalidSessionReason reason)
        {
            string result;
            if (!DefinedErrorMessages.TryGetValue(reason, out result))
            {
                result = DefinedErrorMessages[InvalidSessionReason.Unknown];
            }
            return result;
        }
        #endregion

        private InvalidSessionReason m_reason;

        #region constructors
        /// <summary>
        /// Creates a new <see cref="InvalidSessionException"/> with an unknown reason.
        /// </summary>
        public InvalidSessionException()
            : this(InvalidSessionReason.Unknown) { }

        /// <summary>
        /// Creates a new <see cref="InvalidSessionException"/> with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidSessionException(string message)
            : base(message) { }

        /// <summary>
        /// Creates a new <see cref="InvalidSessionException"/> with the specified error message and causing inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The causing inner exception.</param>
        public InvalidSessionException(string message, Exception inner)
            : base(message, inner) { }

        /// <summary>
        /// Creates a new <see cref="InvalidSessionException"/> with the specified error reason code.
        /// </summary>
        /// <param name="reason">The reason code.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown if <paramref name="reason"/> is not a valid value for <see cref="InvalidSessionReason"/>.</exception>
        public InvalidSessionException(InvalidSessionReason reason)
            : base(GetReasonErrorMessage(reason))
        {
            if (!Enum.IsDefined(typeof(InvalidSessionReason), reason))
#if !SILVERLIGHT
                throw new InvalidEnumArgumentException("reason", (int)reason, typeof(InvalidSessionReason));
#else
                throw new ArgumentOutOfRangeException("reason", "The specified reason was not defined.");
#endif

            m_reason = reason;
        }
        #endregion
        #region Serializable support
#if !SILVERLIGHT
        /// <summary>
        /// Creates a new <see cref="InvalidSessionException"/> with the relevant serialization information.
        /// </summary>
        /// <param name="info">The serialization data store.</param>
        /// <param name="context">The serialization context.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        protected InvalidSessionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_reason = (InvalidSessionReason)info.GetInt32("m_reason");
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("m_reason", (int)m_reason);
        }
#endif
        #endregion

        /// <summary>
        /// Gets the reason code for the exception.
        /// </summary>
        public InvalidSessionReason Reason
        {
            get { return m_reason; }
        }   
    }

    /// <summary>
    /// Specifies the kinds of reasons for which an <see cref="InvalidSessionException"/> might be thrown.
    /// </summary>
    public enum InvalidSessionReason
    {
        /// <summary>
        /// Indicates that the session failure reason is unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// Indicates that the session is missing its access token.
        /// </summary>
        MissingAccessToken,
        /// <summary>
        /// Indicates that the session had expired.
        /// </summary>
        SessionExpired,
    }
}
