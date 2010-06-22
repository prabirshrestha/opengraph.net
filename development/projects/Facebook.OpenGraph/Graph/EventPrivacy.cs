using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.Graph;
using System.ComponentModel;

namespace Facebook
{
    namespace Graph
    {
        /// <summary>
        /// Specifies the privacy settings for a <see cref="GraphEvent"/>.
        /// </summary>
        public enum EventPrivacy
        {
            /// <summary>
            /// Indicates that the event is open and visible to anyone.
            /// </summary>
            Public,
            /// <summary>
            /// Indicates that the event is closed and not visible to users who are not invited.
            /// </summary>
            Private,
        }
    }

    partial class Extensions
    {
        /// <summary>
        /// Formats the value of an <see cref="EventPrivacy"/> value into a string.
        /// </summary>
        /// <param name="val">The <see cref="EventPrivacy"/> value to render.</param>
        /// <returns>A string representing the value.</returns>
        /// <exception cref="InvalidEnumArgumentException">[Non-Silverlight] Thrown when <paramref name="val"/> is not a valid
        /// value of <see cref="EventPrivacy"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">[Silverlight] Thrown when <paramref name="val"/> is not a valid value
        /// of <see cref="EventPrivacy"/>.</exception>
        public static string Format(this EventPrivacy val)
        {
            if (val != EventPrivacy.Private || val != EventPrivacy.Public)
#if !SILVERLIGHT
                throw new InvalidEnumArgumentException("val", (int)val, typeof(EventPrivacy));
#else
                throw new ArgumentOutOfRangeException("val", "The specified value was not a valid value of EventPrivacy.");
#endif

            if (val == EventPrivacy.Public) return "OPEN";
            else return "SECRET";
        }
    }
}
