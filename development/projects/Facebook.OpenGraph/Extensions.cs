using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.Graph.Util;
using System.Diagnostics;
using Facebook.OpenGraph.Metadata;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json.Linq;
using Facebook.Graph;

namespace Facebook
{
    /// <summary>
    /// Provides extension methods to types within the OpenGraph.NET API.
    /// </summary>
    public static partial class Extensions
    {
        private const long START_NON_FRIENDS_PERMISSIONS = ((long)(ExtendedPermissions.Friend)) << 1;
        private const long END_NON_FRIENDS_PERMISSIONS = ((long)ExtendedPermissions.Reserved);
        private const long START_FRIENDS_PERMISSIONS = ((long)ExtendedPermissions.Reserved) << 1;
        private const long END_FRIENDS_PERMISSIONS = 1 << 62;

        private static Dictionary<AuthorizationPromptStyle, string> PromptRenderings = new Dictionary<AuthorizationPromptStyle, string>() 
        {
            { AuthorizationPromptStyle.Page, "page" },
            { AuthorizationPromptStyle.Popup, "popup" },
            { AuthorizationPromptStyle.Wireless, "wap" },
            { AuthorizationPromptStyle.Touch, "touch" }
        };

        /// <summary>
        /// Renders a Graph API-compatible string representation of an <see cref="ExtendedPermissions"/> instance.
        /// </summary>
        /// <param name="permissions">The permissions to render.</param>
        /// <returns>A Graph API-compatible string representation of the permissions.</returns>
        public static string Print(this ExtendedPermissions permissions)
        {
            if (permissions == ExtendedPermissions.None)
                return string.Empty;

            long bitfield = (long)permissions;
            StringBuilder result = new StringBuilder();
            for (long bit = START_NON_FRIENDS_PERMISSIONS; bit < END_NON_FRIENDS_PERMISSIONS; bit <<= 1)
            {
                if ((bitfield & bit) == bit)
                {
                    string permissionName;
                    if (RequiresPermissionAttribute.DefinedPermissions.TryGetValue((ExtendedPermissions)bit, out permissionName))
                    {
                        if (result.Length > 0)
                            result.Append(",");

                        result.Append(permissionName);
                    }
                }
            }

            bool useFriend = ((bitfield & (long)ExtendedPermissions.Friend) == (long)ExtendedPermissions.Friend);

            for (long bit = START_FRIENDS_PERMISSIONS; bit < END_FRIENDS_PERMISSIONS; bit <<= 1)
            {
                if ((bitfield & bit) == bit)
                {
                    string permissionName;
                    if (RequiresPermissionAttribute.DefinedPermissions.TryGetValue((ExtendedPermissions)bit, out permissionName))
                    {
                        if (result.Length > 0)
                            result.Append(",");

                        result.Append(permissionName);
                    }

                    if (useFriend)
                    {
                        if (RequiresPermissionAttribute.DefinedPermissions.TryGetValue((ExtendedPermissions)(bit | (long)ExtendedPermissions.Friend), out permissionName))
                        {
                            if (result.Length > 0)
                                result.Append(",");

                            result.Append(permissionName);
                        }
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Renders an Graph API-compatible string of an <see cref="AuthorizationPromptStyle"/> instance.
        /// </summary>
        /// <param name="style">The style to render.</param>
        /// <returns>A Graph API-compatible rendering of the style.</returns>
        public static string Print(this AuthorizationPromptStyle style)
        {
            string result = "popup";
            PromptRenderings.TryGetValue(style, out result);

            return result;
        }

        /// <summary>
        /// Checks to see whether one or more permissions are set for a given permissions entry.
        /// </summary>
        /// <param name="permissions">The permissions to check.</param>
        /// <param name="toCheck">The permissions to see if they are set.</param>
        /// <returns><see langword="true"/> if the permissions are set within the <paramref name="permissions"/> parameter; otherwise <see langword="false" />.</returns>
        public static bool IsSet(this ExtendedPermissions permissions, ExtendedPermissions toCheck)
        {
            return ((permissions & toCheck) == toCheck);
        }

        /// <summary>
        /// Ensures that <paramref name="toAdd"/> permissions are included within the <paramref name="sourcePermissions"/> permissions set, and returns the result.
        /// </summary>
        /// <param name="sourcePermissions">The permissions to which to set the additional permissions.</param>
        /// <param name="toAdd">The additional permissions to add.</param>
        /// <remarks>
        /// <para>The contents of <paramref name="sourcePermissions"/> are not changed.  However, you could fluently add permissions by chaining these calls together:</para>
        /// <code lang="csharp">
        /// ExtendedPermissions result = ExtendedPermissions.Friend.Set(ExtendedPermissions.OfflineAccess).Set(ExtendedPermissions.ReadMailbox);
        /// </code>
        /// </remarks>
        /// <returns>A new <see cref="ExtendedPermissions"/> which combine the source and additional permissions.</returns>
        public static ExtendedPermissions Set(this ExtendedPermissions sourcePermissions, ExtendedPermissions toAdd)
        {
            return sourcePermissions | toAdd;
        }

        /// <summary>
        /// Ensures that the permissions within <paramref name="toRemove"/> are not included in the set of permissions.
        /// </summary>
        /// <param name="sourcePermissions">The permissions from which to clear the desired permissions.</param>
        /// <param name="toRemove">The permissions to remove.</param>
        /// <remarks>
        /// </remarks>
        /// <returns>A new <see cref="ExtendedPermissions"/> which removes the desired permissions from the source.</returns>
        public static ExtendedPermissions Clear(this ExtendedPermissions sourcePermissions, ExtendedPermissions toRemove)
        {
            ExtendedPermissions flags = (ExtendedPermissions)(-1);
            flags ^= toRemove;
            return (sourcePermissions & flags);
        }

        /// <summary>
        /// Parses a Graph API-compatible string of extended permissions into its equivalent 
        /// <see cref="ExtendedPermissions"/> enumeration instance.
        /// </summary>
        /// <param name="extendedPermissions">The text to parse.</param>
        /// <returns>An <see cref="ExtendedPermissions"/> instance with the corresponding flags set.</returns>
        public static ExtendedPermissions ParseIntoPermissions(this string extendedPermissions)
        {
            ExtendedPermissions result = ExtendedPermissions.None;
            string[] individualPermissions = extendedPermissions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string perm in individualPermissions)
            {
                ExtendedPermissions thisPerm;
                if (RequiresPermissionAttribute.PermissionMap.TryGetValue(perm, out thisPerm))
                {
                    result = result.Set(thisPerm);
                }
                else
                {
                    Debug.WriteLine(perm, "Unrecognized permission");
                }
            }

            return result;
        }

        internal static string UrlEncode(this string stringToEncode)
        {
            return Uri.EscapeDataString(stringToEncode);
        }

        internal static Dictionary<string, string> SplitIntoDictionary(this string dataString, char splitter = '&')
        {
            string[] tokens = dataString.Split(new char[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> result = new Dictionary<string, string>(tokens.Length);
            foreach (string token in tokens)
            {
                string[] component = token.Split(new char[] { '=' }, StringSplitOptions.None);
                result.Add(component[0], component[1]);
            }

            return result;
        }

        internal static byte[] HexToBytes(this string src)
        {
            if (src.Length % 2 == 1)
                src = src.PadLeft(src.Length + 1, '0');

            byte[] result = new byte[src.Length / 2];
            for (int i = 0, s = 0; i < result.Length; i++, s += 2)
            {
                result[i] = byte.Parse(src.Substring(s, 2), NumberStyles.HexNumber);
            }

            return result;
        }

        internal static string ToHex(this byte[] src)
        {
            StringBuilder sb = new StringBuilder(src.Length * 2);
            foreach (byte b in src)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
#if !SILVERLIGHT
        internal static string ToMd5String(this string src)
        {
            return src.ToMd5String(Encoding.ASCII);
        }

        internal static string ToMd5String(this string src, Encoding encoding)
        {
            using (MD5 hash = MD5CryptoServiceProvider.Create())
            {
                return hash.ComputeHash(encoding.GetBytes(src)).ToHex();
            }
        }
#endif
        internal static void CopyToStream(this Stream source, Stream destination)
        {
            if (!source.CanRead)
                throw new InvalidOperationException("Cannot read from source stream.");
            if (!destination.CanWrite)
                throw new InvalidOperationException("Cannot write to target stream.");

            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, bytesRead);
            }
        }

        internal static string ReadToString(this Stream source)
        {
            if (!source.CanRead)
                throw new InvalidOperationException("Cannot read from source stream.");
            if (!source.CanSeek)
                throw new InvalidOperationException("Cannot seek from source stream.");

            long position = source.Position;
            using (StreamReader sr = new StreamReader(source))
            {
                string result = sr.ReadToEnd();
                source.Seek(position, SeekOrigin.Begin);
                return result;
            }
        }

        internal static string ToPostString(this IDictionary<string, string> postData)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in postData.Keys)
            {
                if (sb.Length > 0)
                    sb.Append('&');

                sb.Append(key.UrlEncode());
                sb.Append('=');
                sb.Append(postData[key].UrlEncode());
            }
            return sb.ToString();
        }

        internal static JToken AsToken(this string src)
        {
            return Fetcher.FromJsonText(src);
        }
    }
}
