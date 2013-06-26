using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using PhotoQwrekster.Model;
using System.Windows.Media.Imaging;
using System;
using PhotoQwrekster.Helpers;
using System.Collections.Generic;
using System.IO;

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
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the ShareInNetViewModel class.
        /// </summary>
        public ShareInNetViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ActionsList = new ObservableCollection<ActionDescriptionModel>()
            { 
                new ActionDescriptionModel(){ ActionId=ActionDestination.Facebook, 
                    Description="facebuchek",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Flickr,
                    Description="flickr",
                    UserName="aaaa",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/flickr.png", UriKind.Relative))},
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

        internal void ShareImage(ActionDescriptionModel data)
        {
            switch (data.ActionId)
            {
                case ActionDestination.Facebook:
                    {
                        ShareInFacebook();
                        return;
                    }
                case ActionDestination.Flickr:
                    {
                        ShareInFlickr();
                        return;
                    }
                case ActionDestination.Picasa:
                    {
                        ShareInPicasa();
                        return;
                    }
                case ActionDestination.Tumblr:
                    {
                        ShareInTumblr();
                        return;
                    }
                case ActionDestination.Twitter:
                    {
                        ShareInTwitter();
                        return;
                    }
                default:
                    return;
            }
        }


        /// <summary>
        /// The <see cref="ShowPopupNotification" /> property's name.
        /// </summary>
        public const string ShowPopupNotificationPropertyName = "ShowPopupNotification";

        private bool _showPopupNotification = false;

        /// <summary>
        /// Sets and gets the ShowPopupNotification property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public bool ShowPopupNotification
        {
            get
            {
                return _showPopupNotification;
            }

            set
            {
                if (_showPopupNotification == value)
                {
                    return;
                }

                var oldValue = _showPopupNotification;
                _showPopupNotification = value;
                RaisePropertyChanged(ShowPopupNotificationPropertyName, oldValue, value, true);
            }
        }


        /// <summary>
        /// The <see cref="NotificationMessage" /> property's name.
        /// </summary>
        public const string NotificationMessagePropertyName = "NotificationMessage";

        private string _notificationMessage = string.Empty;

        /// <summary>
        /// Sets and gets the NotificationMessage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public string NotificationMessage
        {
            get
            {
                return _notificationMessage;
            }

            set
            {
                if (_notificationMessage == value)
                {
                    return;
                }

                var oldValue = _notificationMessage;
                _notificationMessage = value;
                RaisePropertyChanged(NotificationMessagePropertyName, oldValue, value, true);
            }
        }


        private void ShareInFacebook()
        {
            App.Container.ReplaceRegistration<IOAuthHelper>(c => new FacebookOAuthHelper());
            FacebookConfigHelper.InitPropertiesFromStorage();

            if (!FacebookConfigHelper.IsLoggedIn)
            {
                // callbackAction = UploadToFacebook;
                PhotoQwrekster.View.OAuthLogin.InspectTokenCallback = UploadToFacebook;
                INavigationService navigation = App.Container.Resolve<INavigationService>();

                navigation.NavigateTo(new Uri(ViewModelLocator.OAuthLogin, UriKind.RelativeOrAbsolute));

            }
            else
                UploadToFacebook(null);
        }

        private void UploadToFacebook(List<object> obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                byte[] buffer = GetImageBuffer(ms);

                FacebookHelper.UploadMedia(buffer, _dataService.GetImageName());

            }
        }

        private byte[] GetImageBuffer(MemoryStream ms)
        {
            // write an image into the stream
            Extensions.SaveJpeg(_dataService.GetBitmapImageSource(), ms,
                _dataService.GetBitmapImageSource().PixelWidth, _dataService.GetBitmapImageSource().PixelHeight, 0, 100);


            byte[] buffer = ms.ToArray();
            return buffer;
        }


        private void ShareInFlickr()
        {
            App.Container.ReplaceRegistration<IOAuthV1Helper>(c => new FlickrOAuthHelper());
            FlickrConfigHelper.InitPropertiesFromStorage();
            
            if (!FlickrConfigHelper.IsLoggedIn)
            {
                PhotoQwrekster.View.OAuthFlickrLogin.InspectTokenCallback = UploadToFlickr;
                INavigationService navigation = App.Container.Resolve<INavigationService>();

                navigation.NavigateTo(new Uri(ViewModelLocator.OAuthFlickrLogin, UriKind.RelativeOrAbsolute));

            }
            else
                UploadToFlickr(null);
        }

        private void UploadToFlickr(List<object> obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                byte[] buffer = GetImageBuffer(ms);

                FlickrHelper.UploadMedia(buffer, _dataService.GetImageName());

            }
        }


        private void ShareInPicasa()
        {
            App.Container.ReplaceRegistration<IOAuthHelper>(c => new GoogleOAuthHelper());
            GoogleConfigHelper.InitPropertiesFromStorage();

            if (!GoogleConfigHelper.IsLoggedIn)
            {
                PhotoQwrekster.View.OAuthLogin.InspectTokenCallback = UploadToPicasa;
                INavigationService navigation = App.Container.Resolve<INavigationService>();

                navigation.NavigateTo(new Uri(ViewModelLocator.OAuthLogin, UriKind.RelativeOrAbsolute));

            }
            else
                UploadToPicasa(null);

        }

        public void UploadToPicasa (List<object> obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                byte[] buffer = GetImageBuffer(ms);

                GoogleHelper.UploadMedia(buffer, _dataService.GetImageName(), ShowUploadResult);

            }
        }

        private void ShowUploadResult(string uploadResult)
        {
            Runner.Invoke(() =>
            {
                ShowPopupNotification = true;
                NotificationMessage = uploadResult;
            });
        }

        private void ShareInTumblr()
        {
            App.Container.ReplaceRegistration<IOAuthV1Helper>(c => new TumblrOAuthHelper());
            TumblrConfigHelper.InitPropertiesFromStorage();

            if (!TumblrConfigHelper.IsLoggedIn)
            {
                PhotoQwrekster.View.OAuthFlickrLogin.InspectTokenCallback = UploadToTumblr;
                INavigationService navigation = App.Container.Resolve<INavigationService>();

                navigation.NavigateTo(new Uri(ViewModelLocator.OAuthFlickrLogin, UriKind.RelativeOrAbsolute));

            }
            else
                UploadToTumblr(null);
        }

        public void UploadToTumblr(List<object> obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                byte[] buffer = GetImageBuffer(ms);

                TumblrHelper.UploadMedia(buffer, _dataService.GetImageName());

            }
        }


        private void ShareInTwitter()
        {
            App.Container.ReplaceRegistration<IOAuthV1Helper>(c => new TwitterOAuthHelper());
            TwitterConfigHelper.InitPropertiesFromStorage();

            if (!TwitterConfigHelper.IsLoggedIn)
            {
                PhotoQwrekster.View.OAuthFlickrLogin.InspectTokenCallback = UploadToTwitter;
                INavigationService navigation = App.Container.Resolve<INavigationService>();

                navigation.NavigateTo(new Uri(ViewModelLocator.OAuthFlickrLogin, UriKind.RelativeOrAbsolute));

            }
            else
                UploadToTwitter(null);
        }

        public void UploadToTwitter(List<object> obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                byte[] buffer = GetImageBuffer(ms);

                TwitterHelper.UploadMedia(buffer, _dataService.GetImageName());

            }
        }

    }
    
}