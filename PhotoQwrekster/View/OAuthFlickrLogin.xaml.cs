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
                NavigateToImageProcessingPage();
            }
        }

        private void PrepareRequestToken()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                
                if (e.Error != null)
                {                    
                //    return;
                }
                string res = e.Result;
               // Dispatcher.BeginInvoke(() => this.Browser.Navigate(oauthHelper.GetLoginUrl()));
               // return;
            };

          
            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {

                if (e.Error != null)
                {
                    //    return;
                }
               // string res = e.Result;
                // Dispatcher.BeginInvoke(() => this.Browser.Navigate(oauthHelper.GetLoginUrl()));
                // return;
                

            };

            client.DownloadStringAsync(FlickrOAuthHelper.GetRequestTokenUrl("GET"));

            //client.UploadStringAsync(new Uri(FlickrConfigHelper.OAuthRequestTokenUrl), FlickrOAuthHelper.GetRequestTokenPostData());
        }

        private void Browser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            connectTitle.Visibility = System.Windows.Visibility.Collapsed;
            Browser.Visibility = System.Windows.Visibility.Collapsed;
            ClearCookies();

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
            else if (e.Uri.ToString().Contains("code="))
            {
                e.Cancel = true;
                var val = e.Uri.Query.Substring(6, e.Uri.Query.Length - 6);
                Browser.Visibility = Visibility.Collapsed;
                oauthHelper.SetTokenCode(val);
                oauthHelper.GetRequestTokenQueryNonStatic(false, InspectTokenCallback);
            }
        }

        
        private void NavigateToImageProcessingPage()
        {
            NavigationService.Navigate(new Uri("/View/ImageProcessingView.xaml", UriKind.RelativeOrAbsolute));
        }

        private static Action<List<object>> _inspectTokenCallback;

        public static Action<List<object>> InspectTokenCallback
        {
            get { return _inspectTokenCallback; }
            set { _inspectTokenCallback = value; }
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigateToImageProcessingPage();
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            connectTitle.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}