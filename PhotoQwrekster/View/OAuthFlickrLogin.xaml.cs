using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.Windows.Navigation;
using PhotoQwrekster.Helpers;
using PhotoQwrekster.ViewModel;

namespace PhotoQwrekster.View
{
    /// <summary>
    /// Description for OAuthFlickrLogin.
    /// </summary>
    public partial class OAuthFlickrLogin : PhoneApplicationPage
    {
        IOAuthHelper oauthHelper;
        /// <summary>
        /// Initializes a new instance of the OAuthFlickrLogin class.
        /// </summary>
        public OAuthFlickrLogin()
        {
            InitializeComponent();
            oauthHelper = App.Container.Resolve<IOAuthHelper>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            if (NetworkChecker.CheckInternetConnection())
            {
                PrepareRequestToken();
            }
            else
            {
                MessageBox.Show("no internet, please, try to sign in later.");
                NavigateToShareInNetPage();
            }
        }

        private int retryDownloadRequestTokenCount = 0;

        private void PrepareRequestToken()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                
                if (e.Error != null)
                {
                    retryDownloadRequestTokenCount++;
                    if (retryDownloadRequestTokenCount < 5)
                        client.DownloadStringAsync(FlickrOAuthHelper.GetRequestTokenUrl("GET"));
                    else
                    {
                        MessageBox.Show("Unable to connect with Flickr");
                        NavigateToShareInNetPage();
                    }
                    return;
                }
                string res = e.Result;
                OAuthFlickrRequestToken requestToken = OAuthFlickrRequestToken.ParseResponse(res);
                if (string.Compare(requestToken.TokenConfirmed.ToLower(), "true") == 0)
                {
                    FlickrOAuthHelper.oauth_token = requestToken.Token;
                    FlickrOAuthHelper.oauth_token_secret = requestToken.TokenSecret;
                    Dispatcher.BeginInvoke(() => this.Browser.Navigate(oauthHelper.GetLoginUrl()));
                }
                return;
            };

            client.DownloadStringAsync(FlickrOAuthHelper.GetRequestTokenUrl("GET"));
        }

        private void Browser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            connectTitle.Visibility = System.Windows.Visibility.Collapsed;
            Browser.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ClearCookies()
        {
            try
            {
                CookieCollection cookies = this.Browser.GetCookies();
                foreach (Cookie cookie in cookies)
                {
                    cookie.Discard = true;
                    cookie.Expired = true;
                }
            }
            catch (Exception)
            {
            }
        }

        void Browser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().Contains("error_code="))
            {
                //just to skip
            }
            else if (e.Uri.ToString().Contains("oauth_verifier="))
            {
                e.Cancel = true;
                Dictionary<string, string> parameters = HelperUtility.StringToDictionary(e.Uri.Query.Substring(1, e.Uri.Query.Length - 1));
                FlickrOAuthHelper.oauth_verifier = parameters["oauth_verifier"];
                Browser.Visibility = Visibility.Collapsed;
                
                oauthHelper.GetRequestTokenQueryNonStatic(false, InspectTokenCallback);
            }
        }

        
        private void NavigateToShareInNetPage()
        {
            NavigationService.Navigate(new Uri("/View/ShareInNetView.xaml", UriKind.RelativeOrAbsolute));
        }

        private static Action<List<object>> _inspectTokenCallback;

        public static Action<List<object>> InspectTokenCallback
        {
            get { return _inspectTokenCallback; }
            set { _inspectTokenCallback = value; }
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigateToShareInNetPage();
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            connectTitle.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}