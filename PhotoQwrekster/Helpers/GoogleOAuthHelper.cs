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

namespace PhotoQwrekster.Helpers
{
    public class GoogleOAuthHelper : IOAuthHelper
    {
        public Uri GetLoginUrl()
        {
            return new Uri(string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}&response_type=code",
                                         GoogleConfigHelper.OAuthUrl,
                                         GoogleConfigHelper.ClientId, GoogleConfigHelper.RedirectUri, GoogleConfigHelper.Scope));
        }

        private static string GetRequestAccessTokenPostBody(bool isRefresh)
        {
            var body = String.Format("client_id={0}&client_secret={1}&", GoogleConfigHelper.ClientId,
                                     GoogleConfigHelper.ClientSecret);
            return isRefresh ? String.Format(body + "refresh_token={0}&grant_type=refresh_token", GoogleConfigHelper.RefreshToken) : String.Format(body + "code={0}&redirect_uri={1}&grant_type=authorization_code", GoogleConfigHelper.TokenCode, GoogleConfigHelper.RedirectUri);
        }

        public static void GetRequestTokenQuery(bool isRefresh, Action<List<object>> callback)
        {
            var postBody = GetRequestAccessTokenPostBody(isRefresh);
            var client = new RestClient { Timeout = GoogleConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.POST)
            {
                Resource = GoogleConfigHelper.OAuthTokenUrl
            };
            request.AddParameter("application/x-www-form-urlencoded", postBody, ParameterType.RequestBody);
            client.ExecuteAsync(request, response =>
            {
                try
                {
                    var o = JObject.Parse(response.Content);
                    GoogleConfigHelper.AccessToken = (string)o.SelectToken("access_token");
                    GoogleConfigHelper.TokenExpiry = (int)o.SelectToken("expires_in");
                    JToken j;
                    if (o.TryGetValue("refresh_token", out j))
                        GoogleConfigHelper.RefreshToken = j.Value<string>();
                    if (callback != null)
                        callback(new List<object> { GoogleConfigHelper.AccessToken });
                }
                catch (Exception)
                {
          //          MessageBox.Show("Some error while trying to retrieve OAuth token code.\n\n" + e.Message);
                }
            });
        }

        public static void RefreshTokenCode(Action<List<object>> callback)
        {
            try
            {

                //Console.WriteLine("RefreshTokenCode start");

                var a=((App)App.Current).SettingsData;
                var settings = a.Settings;

               // Console.WriteLine("setings resieved");



                if (settings.Contains("Timestamp" + GoogleConfigHelper.AccountKey))
                {
                    var secondsElapsed = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - (double)settings["Timestamp" + GoogleConfigHelper.AccountKey];
                    if (secondsElapsed > GoogleConfigHelper.TokenExpiry)
                        GetRequestTokenQuery(true, callback);
                    else
                        callback(new List<object> { new List<string>() });
                }
            }
            catch (Exception ex)
            {

                //Console.WriteLine("RefreshTokenCode exception" + ex.Message + " inner ex " + ex.InnerException + "stack trace " + ex.StackTrace);
    }
            
        }


        public void GetRequestTokenQueryNonStatic(bool isRefresh, Action<List<object>> callback)
        {
            GoogleOAuthHelper.GetRequestTokenQuery(isRefresh, callback);
        }

        public void RefreshTokenCodeNonStatic(Action<List<object>> callback)
        {
            GoogleOAuthHelper.RefreshTokenCode(callback);
        }

        public void SetTokenCode(string value)
        {
            string key = DateTime.Now.GetHashCode().ToString();
            GoogleConfigHelper.AccountKey = key;
            GoogleConfigHelper.TokenCode = value;
        }


        public string GetAccountKey()
        {
            return GoogleConfigHelper.AccountKey;
        }


        public string GetStorage()
        {
            return "Google";
        }


        public void SetAccountKey(string key)
        {
            GoogleConfigHelper.AccountKey = key;
        }
    }
}
