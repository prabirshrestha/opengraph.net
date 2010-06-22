using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Facebook.OpenGraph.Configuration;
using System.Configuration;
using System.ComponentModel;
using Facebook.Graph.Util;

namespace Facebook.Graph.Web.Mvc
{
    public sealed class UnauthorizedLandingPageFilterAttribute : ActionFilterAttribute
    {
        public UnauthorizedLandingPageFilterAttribute()
        {
            PromptStyle = AuthorizationPromptStyle.Popup;
        }

        public UnauthorizedLandingPageFilterAttribute(string applicationConfiguration)
            : this()
        {
            InitializeWithConfiguration(applicationConfiguration);
        }

        private void InitializeWithConfiguration(string applicationConfiguration)
        {
            OpenGraphWebConfigurationSection webConfig = OpenGraphWebConfigurationSection.Instance;
            if (webConfig == null)
                throw new ConfigurationErrorsException("Missing <OpenGraph.NET-Web> configuration section.");

            var item = webConfig.Handlers[applicationConfiguration];
            if (item == null)
                throw new ConfigurationErrorsException("Missing <Handler> element specifying application handler for application named '" + applicationConfiguration + "'.");

            OpenGraphConfigurationSection config = OpenGraphConfigurationSection.Instance;
            if (config == null)
                throw new ConfigurationErrorsException("Missing <OpenGraph.NET> configuration section.");

            var appItem = config.Applications[applicationConfiguration];
            if (appItem == null)
                throw new ConfigurationErrorsException("Missing <Application> element specifying application information for application named '" + applicationConfiguration + "'.");

            ApplicationID = appItem.ID;
            RedirectUrl = item.FullUri;
        }

        /// <summary>
        /// Gets or sets the extended permissions that should be requested.
        /// </summary>
        [DefaultValue(ExtendedPermissions.None)]
        public ExtendedPermissions RequiredPermissions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Facebook prompt style.
        /// </summary>
        [DefaultValue(AuthorizationPromptStyle.Popup)]
        public AuthorizationPromptStyle PromptStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parameter that should receive the string URL for authorization redirection.
        /// </summary>
        public string ParameterName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Application ID that is to be requesting permissions.
        /// </summary>
        public string ApplicationID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the redirection URL for the authorization code.
        /// </summary>
        public string RedirectUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the application configuration elements.
        /// </summary>
        public string ApplicationConfigurationName
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(ParameterName))
                throw new InvalidOperationException("Cannot execute action without specifying destination URL parameter name.");

            if (!string.IsNullOrEmpty(ApplicationConfigurationName) && string.IsNullOrEmpty(ApplicationID))
                InitializeWithConfiguration(ApplicationConfigurationName);

            string url = AccessToken.DetermineUriForRequest(this.ApplicationID, RedirectUrl, false, RequiredPermissions, PromptStyle);
            filterContext.ActionParameters[ParameterName] = url;

            base.OnActionExecuting(filterContext);
        }
    }
}
