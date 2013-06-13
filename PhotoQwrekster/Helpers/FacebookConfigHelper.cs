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
    public class FacebookConfigHelper
    {
        #region Public Properties

        public const string OAuthUrl = "https://www.facebook.com/dialog/oauth/";
        public const string OAuthTokenUrl = "https://graph.facebook.com/oauth/access_token";

        public const string RedirectUri = "http://localhost.com/";
        public const string ClientId = "***";
        public const string ClientSecret = "***";
        public const string ApiKey = "***";
        //    public const string ApiKey = "***";
        //  public const string ClientId = "***";
        // public const string ClientSecret = "***";
        public const int RequestTimeout = 10 * 1000;
        public const string ApplicationTitle = "PhotoStitchRasp";

        /// <summary>
        /// The list of scopes.
        /// </summary>
        public static string[] scopes = new string[] { "user_photos,publish_stream,publish_actions" };

        public FacebookConfigHelper()
        {
            FacebookConfigHelper.InitPropertiesFromStorage();
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

        public const string AccountKey = "Facebook"; 
        
        #endregion

        public static bool IsLoggedIn { 
            get
            {
                return !string.IsNullOrEmpty(RefreshToken) &&                    
                    !string.IsNullOrEmpty(AccessToken);
            }  
        }
    }
}
