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
    public partial class OAuthLogin : PhoneApplicationPage
    {
        IOAuthHelper oauthHelper;
        public OAuthLogin()
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
                if (NavigationContext.QueryString.ContainsKey("logout"))
                    Dispatcher.BeginInvoke(() =>
                    {
                        this.Browser.Navigate(oauthHelper.GetLogoutUrl());
                        oauthHelper.ClearAccessToken();
                    });
                else
                    Dispatcher.BeginInvoke(() => this.Browser.Navigate(oauthHelper.GetLoginUrl()));
            }
            else
            {
                MessageBox.Show("no internet, please, try to sign in later.");
                NavigateToShareInNetPage();
            }
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
            else if (e.Uri.ToString().Contains("code="))
            {
                e.Cancel = true;
                var val = e.Uri.Query.Substring(6, e.Uri.Query.Length - 6);
                Browser.Visibility = Visibility.Collapsed;
                oauthHelper.SetTokenCode(val);
                oauthHelper.GetRequestTokenQueryNonStatic(false, InspectTokenCallback);
            }
            else if (oauthHelper.IsCancelOrLogoutRedirectUrl( e.Uri.ToString()))
            {
                NavigateToShareInNetPage();
            }
        }

        private void OnNavigatedFromAnimation()
        {
            MetroInMotionUtils.MetroInMotion.SlideDownOutAnimation(new FrameworkElement[] { Browser }.ToList<FrameworkElement>(), NavigateToShareInNetPage);
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