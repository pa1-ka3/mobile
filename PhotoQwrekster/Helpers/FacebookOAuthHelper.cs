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
using System.Linq;


namespace PhotoQwrekster.Helpers
{
    public class FacebookOAuthHelper : IOAuthHelper
    {
        public void GetRequestTokenQueryNonStatic(bool isRefresh, Action<System.Collections.Generic.List<object>> callback)
        {
            FacebookOAuthHelper.GetRequestTokenQuery(isRefresh, callback);
        }

        public void RefreshTokenCodeNonStatic(Action<System.Collections.Generic.List<object>> callback)
        {
            FacebookOAuthHelper.RefreshTokenCode(callback);
        }

        public Uri GetLoginUrl()
        {
            return new Uri(string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}&response_type=code",
                                        FacebookConfigHelper.OAuthUrl,
                                        FacebookConfigHelper.ClientId, FacebookConfigHelper.RedirectUri,
                                        HttpUtility.UrlEncode(String.Join(" ", FacebookConfigHelper.scopes))));
        }

        public Uri GetLogoutUrl()
        {
            return new Uri(string.Format("{0}&access_token={1}",
                                        FacebookConfigHelper.LogoutUrl,
                                        FacebookConfigHelper.AccessToken ));
        }

        public void SetTokenCode(string value)
        {
            string key = DateTime.Now.GetHashCode().ToString();
            FacebookConfigHelper.TokenCode = value;
        }

        public string GetAccountKey()
        {
            return FacebookConfigHelper.AccountKey;
        }

        public void SetAccountKey(string key)
        {
        }

        public string GetStorage()
        {
            return "Facebook";
        }

        private static string GetRequestAccessTokenPostBody(bool isRefresh)
        {
            var body = String.Format("client_id={0}&client_secret={1}&", FacebookConfigHelper.ClientId,
                                     FacebookConfigHelper.ClientSecret);
            return String.Format(body + "code={0}&redirect_uri={1}", FacebookConfigHelper.TokenCode, FacebookConfigHelper.RedirectUri);
        }

        public static void GetRequestTokenQuery(bool isRefresh, Action<List<object>> callback)
        {
            var postBody = GetRequestAccessTokenPostBody(isRefresh);
            var client = new RestClient { Timeout = FacebookConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.POST)
            {
                Resource = FacebookConfigHelper.OAuthTokenUrl
            };
            request.AddParameter("application/x-www-form-urlencoded", postBody, ParameterType.RequestBody);
            client.ExecuteAsync(request, response =>
            {
                try
                {                    
                    int accessTokenStart = "access_token=".Length;
                    int accessTokenEnd = response.Content.IndexOf("&expires=");
                    int expiresStart = accessTokenEnd + "&expires=".Length;
                    FacebookConfigHelper.AccessToken = response.Content.Substring(accessTokenStart, accessTokenEnd - accessTokenStart);
                    string expires = response.Content.Substring(expiresStart);
                    int expiresValue;
                    int.TryParse(expires, out expiresValue);
                    FacebookConfigHelper.TokenExpiry = expiresValue;
                                        
                    if (callback != null)
                        callback(new List<object> { FacebookConfigHelper.AccessToken });
                    GetUserName();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Some error while trying to retrieve OAuth token code.\n\n" + e.Message);
                }
            });
        }

        private static void GetUserName()
        {
            var client = new RestClient { Timeout = FacebookConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.GET)
            {
                Resource = "https://graph.facebook.com/me?fields=name&access_token=" + FacebookConfigHelper.AccessToken
            };

            client.ExecuteAsync(request, response =>
            {
                try
                {
                    var o = JObject.Parse(response.Content);
                    string nameToken = (o["name"] == null) ? "" : o["name"].ToString();
                    if (!string.IsNullOrEmpty(nameToken))
                    {
                        ShareInNetViewModel shareViewModel = App.Container.Resolve<ShareInNetViewModel>();
                        var q = from item in shareViewModel.ActionsList
                                where item.ActionId == Model.ActionDestination.Facebook
                                select item;
                        Model.ActionDescriptionModel ad = q.FirstOrDefault();
                        ad.UserName = nameToken;
                        FacebookConfigHelper.UserName = nameToken;
                    }
                }
                finally
                {
                    Runner.Invoke(() =>
                    {
                        INavigationService navigation = App.Container.Resolve<INavigationService>();
                        navigation.NavigateTo(new Uri(ViewModelLocator.ShareInNet, UriKind.RelativeOrAbsolute));
                    });
                }
            });
        }

        public static void RefreshTokenCode(Action<List<object>> callback)
        {
            try
            {
                var a = ((App)App.Current).SettingsData;
                var settings = a.Settings;

                if (settings.Contains("Timestamp" + FacebookConfigHelper.AccountKey))
                {
                    var secondsElapsed = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - (double)settings["Timestamp" + FacebookConfigHelper.AccountKey];
                    if (secondsElapsed > FacebookConfigHelper.TokenExpiry)
                    {
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

        public void ClearAccessToken()
        {
            FacebookConfigHelper.AccessToken = string.Empty;
        }


        public bool IsCancelOrLogoutRedirectUrl(string url)
        {
            if(url.StartsWith(FacebookConfigHelper.LogoutHomeRedirectUrl) ||
                url.StartsWith("http://localhost.com") )
                return true;
            else return false;
        }
    }
}
