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

namespace PhotoQwrekster.Helpers
{
    public class TumblrConfigHelper
    {
          #region Public Properties

        public const string OAuthRequestTokenUrl = "http://www.tumblr.com/oauth/request_token";
        public const string OAuthUrl = "http://www.tumblr.com/oauth/authorize";
        public const string OAuthTokenUrl = "http://www.tumblr.com/oauth/access_token";
        public const string Scope = "write";
        public const string RedirectUri = "localhost";
        public const string ClientSecret = "***";
        public const string ApiKey = "***";
        public const int RequestTimeout = 10 * 1000;
        public const string ApplicationTitle = "PhotoQwrekster";
        public const string UploadUrl = "http://api.tumblr.com/v2/blog/pavelkaparykha.tumblr.com/post";
        public const string MyAccessToken = "***";
        public const string MyAccesTokenSecret = "***";

        public TumblrConfigHelper()
        {
            TumblrConfigHelper.InitPropertiesFromStorage();
        }

        public static void InitPropertiesFromStorage()
        {
            var settings = ((App)App.Current).SettingsData.Settings;
            _tokenExpiry = (int)(!settings.Contains("TokenExpiry" + AccountKey) ? 0 : settings["TokenExpiry" + AccountKey]);
            _tokenCode = !settings.Contains("TokenCode" + AccountKey) ? string.Empty : settings["TokenCode" + AccountKey].ToString();
            _accessToken = !settings.Contains("AccessToken" + AccountKey) ? string.Empty : settings["AccessToken" + AccountKey].ToString();
            _refreshToken = !settings.Contains("RefreshToken" + AccountKey) ? string.Empty : settings["RefreshToken" + AccountKey].ToString();
        }

        private static int _tokenExpiry;
        public static int TokenExpiry
        {
            get
            {
                return _tokenExpiry;
            }
            set
            {
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["TokenExpiry" + AccountKey] = value;
                _tokenExpiry = value;                
            }
        }

        private static string _tokenCode;
        public static string TokenCode
        {
            get
            {
                return _tokenCode;
            }
            set
            {
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["TokenCode" + AccountKey] = value;
                _tokenCode = value;
            }
        }

        private static string _accessToken;
        public static string AccessToken
        {
            get
            {
                return _accessToken;
            }
            set
            {
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["AccessToken" + AccountKey] = value;
                settings["Timestamp" + AccountKey] = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                _accessToken = value;
            }
        }

        private static string _refreshToken;
        public static string RefreshToken
        {
            get
            {
                return _refreshToken;
            }
            set
            {
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["RefreshToken" + AccountKey] = value;
                _refreshToken = value;
            }
        }

        public static string AccountKey = "Tumblr";

        public static bool IsLoggedIn
        {
            get
            {
                return  !string.IsNullOrEmpty(AccessToken);
            }
        }

        #endregion
    }
}
