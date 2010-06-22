using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Contains information that should be included when retrieving authorization via an <see cref="GraphDesktopSession"/>.
    /// This is returned by a callback specified by a <see cref="RetrieveAuthorizationCallback"/>.
    /// </summary>
    public class GraphAuthorizationResult
    {
        /// <summary>
        /// Gets or sets the query string that was returned to the authentication process, excluding the 
        /// question mark (<c>?</c>).
        /// </summary>
        public string ResponseQueryString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL that was used for the redirect request.
        /// </summary>
        public string RedirectUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether <see cref="RedirectUrl"/> is URL-encoded.
        /// </summary>
        public bool IsRedirectUrlEncoded
        {
            get;
            set;
        }
    }
}
