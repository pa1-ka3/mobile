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
using System.Globalization;
using PhotoQwrekster.Model;
using PhotoQwrekster.Helpers;

namespace PhotoQwrekster.Converters
{
    public class LoginLogoutConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ActionDestination destination = (ActionDestination)value;
            bool isLoggedIn = false;
            switch (destination)
            {
                case ActionDestination.Facebook:
                    {
                        if (!string.IsNullOrEmpty(FacebookConfigHelper.AccessToken))
                            isLoggedIn = true;
                        break;
                        
                    }
                case ActionDestination.Flickr:
                    {
                        if (!string.IsNullOrEmpty(FlickrConfigHelper.AccessToken))
                            isLoggedIn = true;
                        break;
                    }
                case ActionDestination.Picasa:
                    {
                        if (!string.IsNullOrEmpty(GoogleConfigHelper.AccessToken))
                            isLoggedIn = true;
                        break;
                    }
                case ActionDestination.Tumblr:
                    {
                        if (!string.IsNullOrEmpty(TumblrConfigHelper.AccessToken))
                            isLoggedIn = true;
                        break;
                    }
                case ActionDestination.Twitter:
                    {
                        if (!string.IsNullOrEmpty(TwitterConfigHelper.AccessToken))
                            isLoggedIn = true;
                        break;
                    }
                
            }
            if (isLoggedIn) return "LogOut";
            else return "LogIn";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
