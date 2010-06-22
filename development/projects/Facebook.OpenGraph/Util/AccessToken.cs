using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net;
using System.IO;

namespace Facebook.Graph.Util
{
    internal static class AccessToken
    {
        private const string TOKEN_URL_FMT = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";
        private const string URL_ACCESS_TOKEN_REQUEST_FMT = "http://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&display={3}&scope={2}";

#if !SILVERLIGHT
        public static string Retrieve(string clientID, string clientSecret, string redirectUrl, 
            string authorizationCode, out DateTime expirationTime, bool isRedirectUrlEncoded = false)
        {
            if (!isRedirectUrlEncoded)
                redirectUrl = redirectUrl.UrlEncode();

            string url = string.Format(CultureInfo.CurrentCulture, TOKEN_URL_FMT, clientID, redirectUrl, clientSecret, authorizationCode);
            
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            ServicePointManager.Expect100Continue = false;
            req.AllowAutoRedirect = false;
            req.UserAgent = GraphSession.UserAgent;

            string accessToken = null;
            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd().Trim();
                    var pieces = result.SplitIntoDictionary();
                    accessToken = pieces["access_token"];
                    if (pieces.ContainsKey("expires"))
                    {
                        string expiration = pieces["expires"];
                        double seconds = double.Parse(expiration);

                        expirationTime = DateTime.Now.AddSeconds(seconds);
                    }
                    else
                    {
                        expirationTime = DateTime.MaxValue;
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string error = reader.ReadToEnd();
                    throw ExceptionParser.Parse(Fetcher.FromJsonText(error));
                }
            }

            return accessToken;
        }
#endif

        public static void RetrieveAsync(string clientID, string clientSecret, string redirectUrl, string authorizationCode,
            Action<string, DateTime> callback, Action<string> errorCallback, bool isRedirectUrlEncoded = false)
        {
            if (!isRedirectUrlEncoded)
                redirectUrl = redirectUrl.UrlEncode();

            string url = string.Format(CultureInfo.CurrentCulture, TOKEN_URL_FMT, clientID, redirectUrl, clientSecret, authorizationCode);

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;

            string accessToken = null;
            req.BeginGetResponse(state =>
            {
                try
                {
                    DateTime expirationTime = DateTime.MinValue;
                    using (HttpWebResponse response = req.EndGetResponse(state) as HttpWebResponse)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd().Trim();
                        var pieces = result.SplitIntoDictionary();
                        accessToken = pieces["access_token"];
                        if (pieces.ContainsKey("expires"))
                        {
                            string expiration = pieces["expires"];
                            double seconds = double.Parse(expiration);

                            expirationTime = DateTime.Now.AddSeconds(seconds);
                        }
                        else
                        {
                            expirationTime = DateTime.MaxValue;
                        }
                        callback(accessToken, expirationTime);
                    }
                }
                catch (WebException we)
                {
                    using (HttpWebResponse response = we.Response as HttpWebResponse)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string error = reader.ReadToEnd();
                        if (errorCallback != null)
                            errorCallback(error);
                    }
                }
            }, null);

        }

        public static string DetermineUriForRequest(string appID, string redirectUrl, bool isRedirectUrlEncoded = false, 
            ExtendedPermissions permissions = ExtendedPermissions.None, AuthorizationPromptStyle promptType = AuthorizationPromptStyle.Popup)
        {
            if (!isRedirectUrlEncoded)
                redirectUrl = redirectUrl.UrlEncode();

            string url = string.Format(CultureInfo.CurrentUICulture, URL_ACCESS_TOKEN_REQUEST_FMT, appID, redirectUrl, permissions.Print(), promptType.Print());
            return url;
        }
    }
}
