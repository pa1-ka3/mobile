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
using System.ComponentModel;

namespace PhotoQwrekster.Model
{
    public enum ActionDestination
    {
        Facebook,
        Flikr,
        Picassa,
        Tumblr,
        Twitter
    }

    public class ActionDescriptionModel : INotifyPropertyChanged
    {

        private ActionDestination _ActionId;
        public ActionDestination ActionId
        {
            get { return _ActionId; }
            set
            {
                _ActionId = value;
                RaisePropertyChanged("ActionId");
            }
        }

        private string _Description;
        public string Description
        {
            set
            {
                _Description = value;
                RaisePropertyChanged("Description");
            }
            get { return _Description; }
        }

        
        private string _userName = String.Empty;

        /// <summary>
        /// Sets and gets the UserName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName == value)   return;

                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
