using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using PhotoQwrekster.Model;
using System.Windows.Media.Imaging;
using System;

namespace PhotoQwrekster.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ShareInNetViewModel : ViewModelBase
    {
        

        /// <summary>
        /// Initializes a new instance of the ShareInNetViewModel class.
        /// </summary>
        public ShareInNetViewModel()
        {
            ActionsList = new ObservableCollection<ActionDescriptionModel>()
            { 
                new ActionDescriptionModel(){ ActionId=ActionDestination.Facebook, 
                    Description="facebuchek",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Flikr,
                    Description="flickr",
                    UserName="aaaa",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Picasa,
                    Description="google+",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/picasa.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Twitter,
                    Description="twpid", 
                    UserName="bhh",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/twitter.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Tumblr, Description="tumbler",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))}
            };

            //"/PhotoQwrekster;component/Resources/Photo.png"

        }

        private ObservableCollection<ActionDescriptionModel> _listWithActions = default(ObservableCollection<ActionDescriptionModel>);

        public ObservableCollection<ActionDescriptionModel> ActionsList
        {
            get
            {
                return _listWithActions;
            }
            set
            {
                if (_listWithActions == value)
                {
                    return;
                }

                var oldValue = _listWithActions;
                _listWithActions = value;

                RaisePropertyChanged("ActionsList");
            }
        }
    }
}