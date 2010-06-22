using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Facebook.Graph;
using Newtonsoft.Json.Linq;
using Facebook.Graph.Util;

namespace Facebook.OpenGraph.Web
{
    /// <summary>
    /// Provides a base implementation for real-time update handlers for Web Forms.
    /// </summary>
    /// <remarks>
    /// <para>To utilize this class, simply create a Generic Handler (*.ashx) file in your web application at the desired location and have it inherit from this class 
    /// instead of <see cref="IHttpHandler"/>.  Have it override the <see cref="ApplicationName"/> property and any of the entity-updated events such as 
    /// <see cref="OnUserUpdated"/>.</para>
    /// </remarks>
    /// <seealso href="fa89e31a-1526-4c15-bff1-4902902bc037.htm" target="_self">How to: Implement a Real-Time Update Handler with Web Forms</seealso>
    /// <seealso href="27c3f20c-0d79-4c01-8a2e-1e2c819937f5.htm" target="_self">How to: Register for Real-Time Updates</seealso>
    /// <seealso href="a608160d-239a-4ac9-84f0-d144746439de.htm" target="_self">Real-Time Updates</seealso>
    /// <seealso href="http://developers.facebook.com/docs/api/realtime" target="_blank">Real-time updates - Facebook Developers</seealso>
    public abstract class SubscriptionHandler : IHttpHandler
    {
        #region IHttpHandler Members

        /// <inheritdoc />
        public bool IsReusable
        {
            get { return true; }
        }

        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            var req = context.Request;

            string mode = req.QueryString["hub.mode"];
            string challenge = req.QueryString["hub.challenge"];
            string verifyToken = req.QueryString["hub.verify_token"];

            if (mode == "subscribe")
            {
                HandleSubscriptionVerification(context, challenge, verifyToken);
                return;
            }
            else
            {
                string text = req.InputStream.ReadToString();
                HandleContent(context, text);
                return;
            }
        }

        private void HandleSubscriptionVerification(HttpContext context, string challenge, string verifyToken)
        {
            if (verifyToken == "1")
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write(challenge);
                context.Response.Flush();
                return;
            }
            else
            {
                throw new OAuthException("Invalid verification token.");
            }
        }

        private void HandleContent(HttpContext context, string content)
        {
            JToken token = Fetcher.FromJsonText(content);
            string type = (string)token["object"];
            if (type == "user")
            {
                JArray entries = (JArray)token["entry"];
                for (int i = 0; i < entries.Count; i++)
                {
                    JToken user = entries[i];
                    string uid = user["uid"].ToString();
                    JArray fields = (JArray)user["changed_fields"];
                    string[] updFields = new string[fields.Count];
                    for (int j = 0; j < updFields.Length; j++)
                    {
                        updFields[j] = (string)fields[j];
                    }

                    OnUserUpdated(ApplicationName, uid, updFields);
                }
            }
            else if (type == "permissions")
            {
                OnPermissionsUpdated(ApplicationName, "", ExtendedPermissions.None);
            }
            else
            {

            }

            context.Response.Clear();
            context.Response.Write("true");
            context.Response.Flush();
        }

        #endregion

        /// <summary>
        /// When implemented in a derived class, gets the name of the application handling this request.  This value is passed into the handler methods.
        /// </summary>
        public abstract string ApplicationName { get; }

        /// <summary>
        /// Called when a User object is updated.
        /// </summary>
        /// <param name="applicationName">The name of the application, determined by the routing properties.</param>
        /// <param name="userID">The ID of the user who was updated.</param>
        /// <param name="updatedProperties">A list of properties that were updated for the user.</param>
        protected virtual void OnUserUpdated(string applicationName, string userID, string[] updatedProperties) { }

        /// <summary>
        /// Not yet supported.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userID"></param>
        /// <param name="permissions"></param>
        protected virtual void OnPermissionsUpdated(string applicationName, string userID, ExtendedPermissions permissions) { }
        /// <summary>
        /// Not yet supported.
        /// </summary>
        /// <param name="applicationName"></param>
        protected virtual void OnErrorsReported(string applicationName) { }
    }
}
