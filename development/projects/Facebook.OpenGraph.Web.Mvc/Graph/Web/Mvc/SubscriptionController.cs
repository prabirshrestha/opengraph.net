using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace Facebook.Graph.Web.Mvc
{
    /// <summary>
    /// Implements the basic behaviors necessary for a <see cref="Controller"/> class to handle incoming requests 
    /// from the Graph server to handle subscription updates.
    /// </summary>
    public class SubscriptionController : Controller
    {
        // TODO: Implement handlers

        [SubscriptionVerificationFilter]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Handler(string challenge, string verify_token, object content, string applicationName)
        {
            if (verify_token == "1")
            {
                return Content(challenge, "text/plain");
            }

            JToken token = content as JToken;

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
                        updFields[j] = (string)fields[j];

                    OnUserUpdated(applicationName, uid, updFields);
                }
            }
            else if (type == "permissions")
            {
                OnPermissionsUpdated(applicationName, "", ExtendedPermissions.None);
            }
            return Json(true);
        }

        /// <summary>
        /// Called when a User object is updated.
        /// </summary>
        /// <param name="applicationName">The name of the application, determined by the routing properties.</param>
        /// <param name="userID">The ID of the user who was updated.</param>
        /// <param name="updatedProperties">A list of properties that were updated for the user.</param>
        protected virtual void OnUserUpdated(string applicationName, string userID, string[] updatedProperties) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userID"></param>
        /// <param name="permissions"></param>
        protected virtual void OnPermissionsUpdated(string applicationName, string userID, ExtendedPermissions permissions) { }
        protected virtual void OnErrorsReported(string applicationName) { }
    }
}
