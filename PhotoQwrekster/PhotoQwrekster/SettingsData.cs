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
using Microsoft.Phone.Shell;
using System.Linq;
using System.IO.IsolatedStorage;
namespace PhotoQwrekster
{
    /// <summary>
    /// Class for saving data that comes with first URI(That appears when clicking on tile).
    /// </summary>
    public class SettingsData
    {
        
        
        public string Uri
        {
            get;
            set;
        }
        object locker = new object();
        IsolatedStorageSettings settings;
        public IsolatedStorageSettings Settings
        {
            get
            {
                lock(locker)
                {
                    return settings;
                }
            }
        }
        
        public SettingsData()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

    }
}
