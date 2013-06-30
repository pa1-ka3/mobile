using System;
using System.IO;
using System.Windows;
using Microsoft.Devices;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using PhotoQwrekster.Helpers;

namespace PhotoQwrekster.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }
        
        private Stream capturedStream;

        public Stream CapturedStream
        {
            get { return capturedStream; }
            set { capturedStream = value; }
        }

        public void SetCapturedImageStream(System.IO.Stream capturedStream)
        {
            CapturedStream = capturedStream;
        }


        private byte[] capturedImageByte;

        public byte[] CapturedImageByte
        {
            get { return capturedImageByte; }
            set { capturedImageByte = value; }
        }



        public void SetCupturedImageByte(byte[] dest1)
        {
            CapturedImageByte = dest1;
        }


        public Stream GetCapturedStream()
        {
            return CapturedStream;
        }


        public byte[] GetCapturedImageByte()
        {
            return CapturedImageByte;
        }


        private Size _cameraResolution;

        public Size CameraResolution
        {
            get { return _cameraResolution; }
            set
            {
                _cameraResolution = value;
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["CameraResolution"] = value;
            }
        }

        public Size GetResolutionSetting()
        {
            return CameraResolution;
        }

        public void SaveResolutionSettings(System.Windows.Size res)
        {
            CameraResolution = res;
        }

        public void InitPropertiesFromStorage()
        {            
            var settings = ((App)App.Current).SettingsData.Settings;
            _cameraResolution = settings.Contains("CameraResolution") ? (Size)(settings["CameraResolution"]) : new Size(0, 0);
            _cameraFlashMode = settings.Contains("CameraFlashMode") ? (FlashMode)(settings["CameraFlashMode"]) : FlashMode.Auto;
            _transformationMode = settings.Contains("TransformationMode") ? (bool)(settings["TransformationMode"]) : false; 
        }


        private FlashMode _cameraFlashMode;

        public FlashMode CameraFlashMode
        {
            get { return _cameraFlashMode; }
            set
            {
                _cameraFlashMode = value;
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["CameraFlashMode"] = value;
            }
        }


        public void SaveFlashModeSettings(FlashMode flashMode)
        {
            CameraFlashMode = flashMode;
        }


        public FlashMode GetFlashModeSettings()
        {
            return CameraFlashMode;
        }


        private bool _transformationMode;

        public bool TransformationMode
        {
            get { return _transformationMode; }
            set
            {
                _transformationMode = value;
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["TransformationMode"] = value;
            }
        }
        


        public bool GetTransformModeSettings()
        {
            return TransformationMode;
        }

        public void SaveTransformModeSettings(bool transformMode)
        {
            TransformationMode = transformMode;
        }

        public WriteableBitmap BitmapImageSource { get; set; }


        public WriteableBitmap GetBitmapImageSource()
        {
            return BitmapImageSource;
        }

        public void SetBitmapImageSource(WriteableBitmap bitmap)
        {
            BitmapImageSource = bitmap;
        }

        private string _imageName { get; set; }

        public void SetImageName(string imageName)
        {
            _imageName = imageName;
        }

        public string GetImageName()
        {
            if(string.IsNullOrEmpty(_imageName))
                _imageName = "PQ-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".jpg";
            return _imageName;
        }


        public System.Collections.ObjectModel.ObservableCollection<ActionDescriptionModel> GetActionsList()
        {
            return new ObservableCollection<ActionDescriptionModel>()
            { 
                new ActionDescriptionModel(){ ActionId=ActionDestination.Facebook, 
                    UserName = FacebookConfigHelper.UserName,
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Flickr,
                    Description="flickr",
                    UserName = FlickrConfigHelper.UserName,
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/flickr.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Picasa,
                    UserName = GoogleConfigHelper.UserName,
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/picasa.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Twitter,
                    UserName = TwitterConfigHelper.UserName,
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/twitter.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Tumblr, Description="tumbler",
                    UserName = TumblrConfigHelper.UserName,
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))}
            };
        }
    }
}