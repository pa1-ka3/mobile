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
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json.Linq;
using PhotoQwrekster.View;
using PhotoQwrekster.ViewModel;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace PhotoQwrekster.Helpers
{
    public class FlickrOAuthHelper : IOAuthHelper
    {

        public void GetRequestTokenQueryNonStatic(bool isRefresh, Action<System.Collections.Generic.List<object>> callback)
        {
            FlickrOAuthHelper.GetRequestTokenQuery(isRefresh, callback);
        }

        public void RefreshTokenCodeNonStatic(Action<System.Collections.Generic.List<object>> callback)
        {
            FlickrOAuthHelper.RefreshTokenCode(callback);
        }

        public Uri GetLoginUrl()
        {
            return new Uri(string.Format("{0}?",
                FlickrConfigHelper.ApiEndpoint));
        }

        public static Uri GetRequestTokenUrl(string method)
        {
            Dictionary<string, string> parameters;
            string oauthnonce;
            string oauthtimestamp;
            RequestTokenParameters(out parameters, out oauthnonce, out oauthtimestamp);
            string sig = OAuthCalculateSignature(method, FlickrConfigHelper.OAuthRequestTokenUrl, parameters, null);
            

            return new Uri(string.Format("{0}?oauth_callback={1}&oauth_consumer_key={2}&oauth_nonce={3}&oauth_timestamp={4}&perms={5}&oauth_signature_method={6}&oauth_signature={7}",
                                        FlickrConfigHelper.OAuthRequestTokenUrl,
                                        FlickrConfigHelper.RedirectUri,
                                        FlickrConfigHelper.ApiKey,
                                        oauthnonce,
                                        oauthtimestamp, 
                                        FlickrConfigHelper.Scope,
                                        "HMAC-SHA1",
                                        sig));
        }

        private static void RequestTokenParameters(out Dictionary<string, string> parameters, out string oauthnonce, out string oauthtimestamp)
        {
            parameters = new Dictionary<string, string>();

            parameters.Add("oauth_callback", FlickrConfigHelper.RedirectUri);
            parameters.Add("oauth_consumer_key", FlickrConfigHelper.ApiKey);
            oauthnonce = Guid.NewGuid().ToString("N");
            parameters.Add("oauth_nonce", oauthnonce);
            oauthtimestamp = HelperUtility.GetUnixTimestamp();
            parameters.Add("oauth_timestamp", oauthtimestamp);
            // parameters.Add("oauth_version", "1.0");
            parameters.Add("perms", FlickrConfigHelper.Scope);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
        }

        public static string GetRequestTokenPostData()
        {
            Dictionary<string, string> parameters;
            string oauthnonce;
            string oauthtimestamp;
            RequestTokenParameters(out parameters, out oauthnonce, out oauthtimestamp);
            string sig = OAuthCalculateSignature("POST", FlickrConfigHelper.OAuthRequestTokenUrl, parameters, null);
            parameters.Add("oauth_signature", sig);
            return OAuthCalculatePostData(parameters);
            
        }

        public static string OAuthCalculatePostData(Dictionary<string, string> parameters)
        {
            string data = String.Empty;
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                // Silverlight < 5 doesn't support modification of the Authorization header, so all data must be sent in post body.
#if SILVERLIGHT
                data += pair.Key + "=" + HelperUtility.EscapeOAuthString(pair.Value) + "&";
#else
                if (!pair.Key.StartsWith("oauth"))
                {
                    data += pair.Key + "=" + Uri.EscapeDataString(pair.Value) + "&";
                }
#endif
            }
            return data;
        }

        public static string OAuthCalculateSignature(string method, string url, Dictionary<string, string> parameters, string tokenSecret)
        {
            string baseString = "";
            string key = FlickrConfigHelper.ClientSecret + "&" + tokenSecret;
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

#if !SILVERLIGHT
            SortedList<string, string> sorted = new SortedList<string, string>();
            foreach (KeyValuePair<string, string> pair in parameters) { sorted.Add(pair.Key, pair.Value); }
#else
            var sorted = parameters.OrderBy(p => p.Key);
#endif

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in sorted)
            {
                sb.Append(pair.Key);
                sb.Append("=");
                sb.Append(HelperUtility.EscapeOAuthString(pair.Value));
                sb.Append("&");
            }

            sb.Remove(sb.Length - 1, 1);

            baseString = method + "&" + HelperUtility.EscapeOAuthString(url) + "&" + HelperUtility.EscapeOAuthString(sb.ToString());

#if WindowsCE
            FlickrNet.Security.Cryptography.HMACSHA1 sha1 = new FlickrNet.Security.Cryptography.HMACSHA1(keyBytes);
#else
            System.Security.Cryptography.HMACSHA1 sha1 = new System.Security.Cryptography.HMACSHA1(keyBytes);
#endif

            byte[] hashBytes = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(baseString));

            string hash = Convert.ToBase64String(hashBytes);

            Debug.WriteLine("key  = " + key);
            Debug.WriteLine("base = " + baseString);
            Debug.WriteLine("sig  = " + hash);

            return hash;
        }


        public void SetTokenCode(string value)
        {
            string key = DateTime.Now.GetHashCode().ToString();
            FlickrConfigHelper.TokenCode = value;
        }

        public string GetAccountKey()
        {
            return FlickrConfigHelper.AccountKey;
        }

        public void SetAccountKey(string key)
        {
        }

        public string GetStorage()
        {
            return "Flickr";
        }

        private static string GetRequestAccessTokenPostBody(bool isRefresh)
        {
            var body = String.Format("client_id={0}&client_secret={1}&", FlickrConfigHelper.ApiKey,
                                     FlickrConfigHelper.ClientSecret);
            return String.Format(body + "code={0}&redirect_uri={1}", FlickrConfigHelper.TokenCode, FlickrConfigHelper.RedirectUri);
        }

        public static void GetRequestTokenQuery(bool isRefresh, Action<List<object>> callback)
        {
            var postBody = GetRequestAccessTokenPostBody(isRefresh);
            var client = new RestClient { Timeout = FlickrConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.POST)
            {
                Resource = FlickrConfigHelper.OAuthTokenUrl
            };
            request.AddParameter("application/x-www-form-urlencoded", postBody, ParameterType.RequestBody);
            client.ExecuteAsync(request, response =>
            {
                try
                {
                    int accessTokenStart = "access_token=".Length;
                    int accessTokenEnd = response.Content.IndexOf("&expires=");
                    int expiresStart = accessTokenEnd + "&expires=".Length;
                    FlickrConfigHelper.AccessToken = response.Content.Substring(accessTokenStart, accessTokenEnd - accessTokenStart);
                    string expires = response.Content.Substring(expiresStart);
                    int expiresValue;
                    int.TryParse(expires, out expiresValue);
                    FlickrConfigHelper.TokenExpiry = expiresValue;

                    if (callback != null)
                        callback(new List<object> { FlickrConfigHelper.AccessToken });
                }
                catch (Exception e)
                {
                    MessageBox.Show("Some error while trying to retrieve OAuth token code.\n\n" + e.Message);
                }
            });
        }

        public static void RefreshTokenCode(Action<List<object>> callback)
        {
            try
            {

                //Console.WriteLine("RefreshTokenCode start");

                var a = ((App)App.Current).SettingsData;
                var settings = a.Settings;

                // Console.WriteLine("setings resieved");



                if (settings.Contains("Timestamp" + FlickrConfigHelper.AccountKey))
                {
                    var secondsElapsed = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - (double)settings["Timestamp" + FlickrConfigHelper.AccountKey];
                    if (secondsElapsed > FlickrConfigHelper.TokenExpiry)
                    {
                        //GetRequestTokenQuery(true, callback);
                        OAuthLogin.InspectTokenCallback = callback;
                        INavigationService navigation = App.Container.Resolve<INavigationService>();

                        navigation.NavigateTo(new Uri(ViewModelLocator.OAuthLogin, UriKind.RelativeOrAbsolute));

                    }
                    else
                        callback(new List<object> { new List<string>() });
                }
            }
            catch (Exception ex)
            {

                //Console.WriteLine("RefreshTokenCode exception" + ex.Message + " inner ex " + ex.InnerException + "stack trace " + ex.StackTrace);
            }

        }
    }
}
