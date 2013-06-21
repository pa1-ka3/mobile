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
    public class TumblrOAuthHelper : IOAuthV1Helper
    {

        public void GetRequestTokenQueryNonStatic(bool isRefresh, Action<System.Collections.Generic.List<object>> callback)
        {
            TumblrOAuthHelper.GetRequestTokenQuery(isRefresh, callback);
        }

        public void RefreshTokenCodeNonStatic(Action<System.Collections.Generic.List<object>> callback)
        {
            TumblrOAuthHelper.RefreshTokenCode(callback);
        }

        public Uri GetLoginUrl()
        {
            return new Uri(string.Format("{0}?oauth_token={1}",
                TumblrConfigHelper.OAuthUrl,
                oauth_token));
        }

        public Uri GetRequestTokenUrl(string method)
        {
            Dictionary<string, string> parameters;
            string oauthnonce;
            string oauthtimestamp;
            RequestTokenParameters(out parameters, out oauthnonce, out oauthtimestamp);
            string sig = OAuthCalculateSignature(method, TumblrConfigHelper.OAuthRequestTokenUrl, parameters, null);


            return new Uri(string.Format("{0}?oauth_callback={1}&oauth_consumer_key={2}&oauth_nonce={3}&oauth_timestamp={4}&oauth_signature_method={5}&oauth_signature={6}",
                                        TumblrConfigHelper.OAuthRequestTokenUrl,
                                        TumblrConfigHelper.RedirectUri,
                                        TumblrConfigHelper.ApiKey,
                                        oauthnonce,
                                        oauthtimestamp,
                //     TumblrConfigHelper.Scope,
                                        "HMAC-SHA1",
                                        sig));
        }

        private static void RequestTokenParameters(out Dictionary<string, string> parameters, out string oauthnonce, out string oauthtimestamp)
        {
            parameters = new Dictionary<string, string>();

            parameters.Add("oauth_callback", TumblrConfigHelper.RedirectUri);
            parameters.Add("oauth_consumer_key", TumblrConfigHelper.ApiKey);
            oauthnonce = Guid.NewGuid().ToString("N");
            parameters.Add("oauth_nonce", oauthnonce);
            oauthtimestamp = HelperUtility.GetUnixTimestamp();
            parameters.Add("oauth_timestamp", oauthtimestamp);
            // parameters.Add("oauth_version", "1.0");
            // parameters.Add("perms", TumblrConfigHelper.Scope);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
        }

        public static string OAuthCalculateSignature(string method, string url, Dictionary<string, string> parameters, string tokenSecret)
        {
            string baseString = "";
            string key = TumblrConfigHelper.ClientSecret + "&" + tokenSecret;
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
            TumblrNet.Security.Cryptography.HMACSHA1 sha1 = new TumblrNet.Security.Cryptography.HMACSHA1(keyBytes);
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
            TumblrConfigHelper.TokenCode = value;
        }

        public string GetAccountKey()
        {
            return TumblrConfigHelper.AccountKey;
        }

        public void SetAccountKey(string key)
        {
        }

        public string GetStorage()
        {
            return "Tumblr";
        }

        public static void GetRequestTokenQuery(bool isRefresh, Action<List<object>> callback)
        {
            var client = new RestClient { Timeout = TumblrConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.GET)
            {
                Resource = TumblrConfigHelper.OAuthTokenUrl
            };

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            string oauthnonce = Guid.NewGuid().ToString("N");
            parameters.Add("oauth_nonce", oauthnonce);
            string oauthtimestamp = HelperUtility.GetUnixTimestamp();
            parameters.Add("oauth_timestamp", oauthtimestamp);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_consumer_key", TumblrConfigHelper.ApiKey);
            parameters.Add("oauth_verifier", oauth_verifier);
            parameters.Add("oauth_token", oauth_token);

            string sig = OAuthCalculateSignature("GET", TumblrConfigHelper.OAuthTokenUrl, parameters, oauth_token_secret);

            request.AddParameter("oauth_nonce", oauthnonce);
            request.AddParameter("oauth_timestamp", oauthtimestamp);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_consumer_key", TumblrConfigHelper.ApiKey);
            request.AddParameter("oauth_verifier", oauth_verifier);
            request.AddParameter("oauth_token", oauth_token);
            request.AddParameter("oauth_signature", sig);


            client.ExecuteAsync(request, response =>
            {
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        OAuthFlickrAccessToken accessTokenResult = OAuthFlickrAccessToken.ParseResponse(response.Content);
                        TumblrConfigHelper.AccessToken = accessTokenResult.Token;
                        TumblrOAuthHelper.oauth_token_secret = accessTokenResult.TokenSecret;

                        if (callback != null)
                            callback(new List<object> { TumblrConfigHelper.AccessToken });
                    }
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



                if (settings.Contains("Timestamp" + TumblrConfigHelper.AccountKey))
                {
                    var secondsElapsed = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - (double)settings["Timestamp" + TumblrConfigHelper.AccountKey];
                    if (secondsElapsed > TumblrConfigHelper.TokenExpiry)
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

        public static string oauth_token { get; set; }

        public static string oauth_token_secret { get; set; }

        public static string oauth_verifier { get; set; }

        public void SetOAuthTokenAndSecret(string token, string token_secret)
        {
            oauth_token = token;
            oauth_token_secret = token_secret;
        }

        public void SetVerifier(string verifier)
        {
            oauth_verifier = verifier;
        }
    }
}
