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
using System.Windows.Data;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace PhotoQwrekster.Converters
{
    public class FlashImageConverter : IValueConverter
    {
        private static Dictionary<String, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();
        private static string assetPath = "../Resources/";


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage result = null;
            // Path to the icon image
            string path = assetPath + (string)value;
            // Check if we already cached the image
            if (!imageCache.TryGetValue(path, out result))
            {
                Uri source = new Uri(path, UriKind.Relative);
                result = new BitmapImage(source);
                imageCache.Add(path, result);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

       
    }

}

