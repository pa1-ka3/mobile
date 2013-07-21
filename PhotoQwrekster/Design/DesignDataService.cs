using System;
using PhotoQwrekster.Model;
using System.Collections.ObjectModel;
using PhotoQwrekster.Helpers;
using System.Windows.Media.Imaging;

namespace PhotoQwrekster.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }


        public void SetCapturedImageStream(System.IO.Stream capturedStream)
        {
            throw new NotImplementedException();
        }

        public void SetCupturedImageByte(byte[] dest1)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetCapturedStream()
        {
            throw new NotImplementedException();
        }


        public byte[] GetCapturedImageByte()
        {
            throw new NotImplementedException();
        }


       System.Windows.Size IDataService.GetResolutionSetting()
        {
            return new System.Windows.Size(3640, 3480);
        }


       public void SaveResolutionSettings(System.Windows.Size res)
       {
           throw new NotImplementedException();
       }


       public void InitPropertiesFromStorage()
       {
           throw new NotImplementedException();
       }


       public void SaveFlashModeSettings(Microsoft.Devices.FlashMode flashMode)
       {
           throw new NotImplementedException();
       }


       public Microsoft.Devices.FlashMode GetFlashModeSettings()
       {
           return Microsoft.Devices.FlashMode.On;
       }


       public bool GetTransformModeSettings()
       {
           return true;
       }

       public void SaveTransformModeSettings(bool transformMode)
       {
           throw new NotImplementedException();
       }


       public System.Windows.Media.Imaging.WriteableBitmap GetBitmapImageSource()
       {
           throw new NotImplementedException();
       }

       public void SetBitmapImageSource(System.Windows.Media.Imaging.WriteableBitmap bitmap)
       {
           throw new NotImplementedException();
       }


       public void SetImageName(string imageName)
       {
           throw new NotImplementedException();
       }

       public string GetImageName()
       {
           throw new NotImplementedException();
       }


       public System.Collections.ObjectModel.ObservableCollection<ActionDescriptionModel> GetActionsList()
       {
           return new ObservableCollection<ActionDescriptionModel>()
            { 
                new ActionDescriptionModel(){ ActionId=ActionDestination.Facebook, 
                    UserName = "Facebook.UserName",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Flickr,
                    
                    UserName = "FlickrConfigHelper.UserName",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/flickr.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Picasa,
                    UserName = GoogleConfigHelper.UserName,
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/picasa.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Twitter,
                    UserName = TwitterConfigHelper.UserName,
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/twitter.png", UriKind.Relative))},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Tumblr, Description="tumbler",
                    UserName = "Tumblr.UserName",
                    IconImage = new BitmapImage(new Uri("/PhotoQwrekster;component/Resources/facebook.png", UriKind.Relative))}
            };
       }


       public void MakeScreenshot()
       {
           throw new NotImplementedException();
       }


       public void MakeScreenshot(Microsoft.Phone.Controls.PhoneApplicationPage page)
       {
           throw new NotImplementedException();
       }
    }
}