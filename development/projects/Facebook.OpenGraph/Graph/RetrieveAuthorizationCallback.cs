using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Implemented by an application using an <see cref="GraphDesktopSession"/> in order to present a user interface
    /// to request OAuth authentication.
    /// </summary>
    /// <param name="authenticateUrl">The URL to display to the user to request authentication.</param>
    /// <returns>An <see cref="GraphAuthorizationResult"/> containing the results of the authentication request.</returns>
    /// <seealso href="f91221eb-2555-4772-814e-e418406aaa50.htm" target="_self">How to: Create a Graph API Desktop Client</seealso>
    public delegate GraphAuthorizationResult RetrieveAuthorizationCallback(string authenticateUrl);
}
