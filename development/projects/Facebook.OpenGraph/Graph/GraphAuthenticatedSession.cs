using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Configuration;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a pre-authenticated session of the OpenGraph API.
    /// </summary>
    /// <permission cref="ReflectionPermission">When not using Silverlight, this class specifies a LinkDemand for ReflectionPermission, requiring
    /// <see cref="ReflectionPermissionAttribute.MemberAccess"/> and 
    /// <see cref="ReflectionPermissionAttribute.ReflectionEmit"/> in order to operate correctly.</permission>
#if !SILVERLIGHT
    [ReflectionPermission(SecurityAction.LinkDemand, ReflectionEmit = true, MemberAccess = true, RestrictedMemberAccess = false)]
    [ReflectionPermission(SecurityAction.InheritanceDemand, ReflectionEmit = true, MemberAccess = true, RestrictedMemberAccess = false)]
#endif
    public class GraphAuthenticatedSession
        : GraphSession
    {
        #region constructors
        /// <summary>
        /// Creates a new <see cref="GraphAuthenticatedSession"/>.
        /// </summary>
        /// <param name="applicationConfigurationName">The name of the application configuration element found
        /// in the application or web configuration file.</param>
        /// <param name="accessToken">The access token provided by authenticating to the OpenGraph API.</param>
        /// <param name="accessTokenExpiration">The time at which the access token expires.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationConfigurationName"/> is 
        /// null or empty.</exception>
        /// <exception cref="ConfigurationErrorsException">Thrown if the application configuration entry with the specified
        /// name could not be located in the configuration file, or if the configuration file did not specify an 
        /// OpenGraph configuration section named <c>&lt;OpenGraph.NET&gt;</c>.</exception>
        public GraphAuthenticatedSession(string applicationConfigurationName, string accessToken, 
            DateTime accessTokenExpiration)
            : base(applicationConfigurationName)
        {
            base.AccessToken = accessToken;
            base.AccessTokenExpiration = accessTokenExpiration;
        }

        /// <summary>
        /// Creates a new <see cref="GraphAuthenticatedSession"/>.
        /// </summary>
        /// <param name="applicationID">The application ID, provided by the Developers application page.</param>
        /// <param name="applicationSecret">The application Secret, provided by the Developers application page.</param>
        /// <param name="accessToken">The access token provided by authenticating to the OpenGraph API.</param>
        /// <param name="accessTokenExpiration">The time at which the access token expires.</param>
        /// <exception cref="ConfigurationErrorsException">Thrown if the application configuration entry with the specified
        /// name could not be located in the configuration file, or if the configuration file did not specify an 
        /// OpenGraph configuration section named <c>&lt;OpenGraph.NET&gt;</c>.</exception>
        public GraphAuthenticatedSession(string applicationID, string applicationSecret,
            string accessToken, DateTime accessTokenExpiration)
            : base(applicationID, applicationSecret)
        {
            base.AccessToken = accessToken;
            base.AccessTokenExpiration = accessTokenExpiration;
        }
        #endregion

        /// <summary>
        /// Always throws an <see cref="InvalidSessionException"/> because authenticated sessions may not be refreshed.
        /// </summary>
        protected override void RefreshSession()
        {
            throw new InvalidSessionException(InvalidSessionReason.SessionExpired);
        }
    }
}
