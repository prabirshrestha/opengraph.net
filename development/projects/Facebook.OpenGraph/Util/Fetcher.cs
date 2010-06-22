using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Globalization;

namespace Facebook.Graph.Util
{
    internal static class Fetcher
    {
#if !SILVERLIGHT
        public static string Fetch(Uri uri)
        {
            return Fetch(uri.ToString());
        }

        public static string Fetch(string uri)
        {
            HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;
            req.Method = "GET";

            ServicePointManager.Expect100Continue = false;
            req.UserAgent = GraphSession.UserAgent;

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
#endif

        public static void FetchAsync(string url, Action<string> callback, Action<string> errorCallback)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "GET";
#if !SILVERLIGHT
            ServicePointManager.Expect100Continue = false;
            req.UserAgent = GraphSession.UserAgent;
#endif
            req.BeginGetResponse(state =>
            {
                try
                {
                    using (HttpWebResponse response = req.EndGetResponse(state) as HttpWebResponse)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        callback(reader.ReadToEnd());
                    }
                }
                catch (WebException we)
                {
                    using (HttpWebResponse response = we.Response as HttpWebResponse)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        errorCallback(reader.ReadToEnd());
                    }
                }
            }, null);
        }

#if !SILVERLIGHT
        public static string Post(string url, string postContent)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "POST";

            byte[] data = Encoding.UTF8.GetBytes(postContent);


            ServicePointManager.Expect100Continue = false;
            req.UserAgent = GraphSession.UserAgent;
            req.ContentLength = data.Length;

            req.ContentType = "application/x-www-form-urlencoded";
            req.GetRequestStream().Write(data, 0, data.Length);

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
#endif

        public static void PostAsync(string url, string postContent, Action<string> callback, Action<string> errorCallback)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "POST";

            byte[] data = Encoding.UTF8.GetBytes(postContent);

#if !SILVERLIGHT
            ServicePointManager.Expect100Continue = false;
            req.UserAgent = GraphSession.UserAgent;
            req.ContentLength = data.Length;
#endif
            req.ContentType = "application/x-www-form-urlencoded";
            req.BeginGetRequestStream(state =>
            {
                Stream s = req.EndGetRequestStream(state);
                s.Write(data, 0, data.Length);
            }, null);

            req.BeginGetResponse(state =>
            {
                try
                {
                    using (HttpWebResponse response = req.EndGetResponse(state) as HttpWebResponse)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        callback(reader.ReadToEnd());
                    }
                }
                catch (WebException we)
                {
                    using (HttpWebResponse response = we.Response as HttpWebResponse)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        errorCallback(reader.ReadToEnd());
                    }
                }
            }, null);

        }

#if !SILVERLIGHT
        public static string PostWithFile(string url, IDictionary<string, string> formData, string fileName, Stream fileContents)
        {
            Debug.Assert(url != null);
            Debug.Assert(formData != null);
            Debug.Assert(fileName != null);
            Debug.Assert(fileContents != null);
            Debug.Assert(fileContents.CanSeek && fileContents.CanRead);

            ServicePointManager.Expect100Continue = false;

            string boundaryText = "----------------------------" + DateTime.Now.Ticks.ToString("x16");

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.UserAgent = GraphSession.UserAgent;
            req.ContentType = "multipart/form-data; boundary=" + boundaryText;
            req.Method = "POST";

            MemoryStream ms = new MemoryStream();

            byte[] boundary = Encoding.UTF8.GetBytes("\r\n--" + boundaryText + "\r\n");

            string itemTemplate = "\r\n--" + boundaryText + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            foreach (string key in formData.Keys)
            {
                string itemText = string.Format(itemTemplate, key, formData[key]);
                byte[] item = System.Text.Encoding.UTF8.GetBytes(itemText);
                ms.Write(item, 0, item.Length);
            }

            ms.Write(boundary, 0, boundary.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";

            string fileHeader = string.Format(CultureInfo.InvariantCulture, headerTemplate, "file", fileName);
            byte[] headerBytes = Encoding.UTF8.GetBytes(fileHeader);
            ms.Write(headerBytes, 0, headerBytes.Length);

            fileContents.Seek(0, SeekOrigin.Begin);
            fileContents.CopyToStream(ms);
            ms.Write(boundary, 0, boundary.Length);

            req.ContentLength = ms.Length;
            Stream dest = req.GetRequestStream();
            byte[] tmp = ms.ToArray();
            dest.Write(tmp, 0, tmp.Length);

            dest.Close();
            ms.Close();

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string PostWithFile(string url, IDictionary<string, string> formData, string filePath, string fileNameSentToServer = null)
        {
            Debug.Assert(url != null);
            Debug.Assert(formData != null);
            Debug.Assert(filePath != null && File.Exists(filePath));

            ServicePointManager.Expect100Continue = false;

            if (fileNameSentToServer == null)
                fileNameSentToServer = Path.GetFileName(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return PostWithFile(url, formData, fileNameSentToServer, fs);
            }
        }
#endif
#if !SILVERLIGHT
        public static string Delete(string uri)
        {
            ServicePointManager.Expect100Continue = false;
            HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;
            req.Method = "DELETE";
            req.UserAgent = GraphSession.UserAgent;

            try
            {
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
#endif

        public static JToken FromJsonText(string json)
        {
            using (StringReader strReader = new StringReader(json))
            using (JsonTextReader jsonReader = new JsonTextReader(strReader))
            {
                return JToken.ReadFrom(jsonReader);
            }
        }

#if !SILVERLIGHT
        public static JToken FetchToken(string uri)
        {
            string json = Fetch(uri);
            return FromJsonText(json);
        }
#endif

        public static void FetchTokenAsync(string uri, Action<JToken> callback, Action<JToken> errorCallback)
        {
            FetchAsync(uri, s => callback(FromJsonText(s)), s => errorCallback(FromJsonText(s)));
        }

#if !SILVERLIGHT
        public static JToken FetchToken(Uri uri)
        {
            string json = Fetch(uri);
            return FromJsonText(json);
        }
#endif
    }
}
