using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Facebook.Graph.Web.Mvc
{
    /// <summary>
    /// Represents the login status of the user currently viewing a canvas page.
    /// </summary>
    public class FacebookStatus
    {
        /// <summary>
        /// Gets whether the user is logged in.
        /// </summary>
        public bool IsLoggedIn { get; internal set; }

        /// <summary>
        /// Gets the user ID associated with the user.
        /// </summary>
        public string UserID { get; internal set; }

        /// <summary>
        /// Gets the session key, if the user is logged in.
        /// </summary>
        public string SessionKey
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets any extended permissions that user has granted the application.
        /// </summary>
        public ExtendedPermissions Permissions
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the Facebook locale string (for example, <c>en_US</c>).
        /// </summary>
        public string Locale
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets whether the user has authorized the application.
        /// </summary>
        public bool IsAppUser
        {
            get;
            internal set;
        }

#if DEBUG
        private Dictionary<string, string> props = new Dictionary<string, string>();

        public Dictionary<string, string> FbParameters
        {
            get { return props; }
        }
#endif
    }
}
