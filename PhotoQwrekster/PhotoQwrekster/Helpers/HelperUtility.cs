using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace PhotoQwrekster.Helpers
{
    public class HelperUtility
    {
        public static string GetUnixTimestamp()
        {
            return ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public static string EscapeOAuthString(string text)
        {
            string value = text;

            value = Uri.EscapeDataString(value).Replace("+", "%20");

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = System.Text.RegularExpressions.Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value.Replace("(", "%28").Replace(")", "%29").Replace("$", "%24").Replace("!", "%21").Replace(
                "*", "%2A").Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;

            
        }

        /// <summary>
        /// Converts a URL parameter encoded string into a dictionary.
        /// </summary>
        /// <remarks>
        /// e.g. ab=cd&amp;ef=gh will return a dictionary of { "ab" => "cd", "ef" => "gh" }.</remarks>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDictionary(string response)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string[] parts = response.Split('&');

            foreach (string part in parts)
            {
#if WindowsCE || SILVERLIGHT
                string[] bits = part.Split('=');
#else
                string[] bits = part.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
#endif
                if (bits.Length == 1)
                    dic.Add(bits[0], "");
                else
                    dic.Add(bits[0], Uri.UnescapeDataString(bits[1]));
            }

            return dic;
        }

    }
}
