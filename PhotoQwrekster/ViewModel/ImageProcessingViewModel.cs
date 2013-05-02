using GalaSoft.MvvmLight;
using PhotoQwrekster.Model;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media.Imaging;
using System.IO;
using ExifLib;
using System.Windows.Media;
using System;
using Microsoft.Phone;
using PhotoQwrekster.Helpers;
using PhotoQwrekster.Input;

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
    public class ImageProcessingViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the ImageProcessingViewModel class.
        /// </summary>
        public ImageProcessingViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }


                });
        }

        #region Commands

        private RelayCommand _openImage;

        /// <summary>
        /// Gets the OpenImage.
        /// </summary>
        public RelayCommand OpenImage
        {
            get
            {
                return _openImage
                    ?? (_openImage = new RelayCommand(
                                          () =>
                                          {
                                              OnLoadImageFromStorage();
                                          }));
            }
        }

        private void OnLoadImageFromStorage()
        {
            throw new System.NotImplementedException();
        }


        private RelayCommand _applyTransformationFilter;

        /// <summary>
        /// Gets the ApplyTransformationFilter.
        /// </summary>
        public RelayCommand ApplyTransformationFilter
        {
            get
            {
                return _applyTransformationFilter
                    ?? (_applyTransformationFilter = new RelayCommand(
                                          () =>
                                          {
                                              OnApplyTransformationFilter();
                                          }));
            }
        }

        private void OnApplyTransformationFilter()
        {
            throw new System.NotImplementedException();
        }

        private RelayCommand _shareInNet;

        /// <summary>
        /// Gets the ShareInNet.
        /// </summary>
        public RelayCommand ShareInNet
        {
            get
            {
                return _shareInNet
                    ?? (_shareInNet = new RelayCommand(
                                          () =>
                                          {
                                              OnShareInNet();
                                          }));
            }
        }

        private void OnShareInNet()
        {
            App.Container.Register<IOAuthHelper>(c => new FacebookOAuthHelper());

            if (!FacebookConfigHelper.IsLoggedIn)
            {
                callbackAction = UploadToFacebook;
                INavigationService navigation = App.Container.Resolve<INavigationService>();
               
                    navigation.NavigateTo(new Uri(ViewModelLocator.OAuthLogin, UriKind.RelativeOrAbsolute));
                
            }
            else
                UploadToFacebook();
        }

        private void UploadToFacebook()
        {
            using (MemoryStream ms = new MemoryStream())
            {

                // write an image into the stream
                Extensions.SaveJpeg(BitmapImageSource, ms,
                    this.BitmapImageSource.PixelWidth, this.BitmapImageSource.PixelHeight, 0, 100);


                byte[] buffer = ms.ToArray();

                FacebookHelper.UploadMedia(buffer, "suprematizm11");

            }
        }

        private RelayCommand _configure;

        /// <summary>
        /// Gets the Configure.
        /// </summary>
        public RelayCommand Configure
        {
            get
            {
                return _configure
                    ?? (_configure = new RelayCommand(
                                          () =>
                                          {
                                              OnConfigure();
                                          }));
            }
        }

        private void OnConfigure()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public WriteableBitmap BitmapImageSource { get; set; }

        internal System.Windows.Media.ImageSource GetCurrentImage()
        {
            BitmapImage bmpImg = new BitmapImage();
            
            //bmpImg.SetSource(picCurrentImage.GetImage());
            // bmpImg.SetSource(library.Pictures[library.Pictures.Count - 1].GetImage());
            byte[] imageData = _dataService.GetCapturedImageByte();            
            MemoryStream capturedStream = new MemoryStream(imageData); 
           
            //RotateTransform transform = new RotateTransform(){ CenterX = 0.5, CenterY = 0.5 };
            int angle = GetAngleFromExif(capturedStream);
            bmpImg.SetSource(capturedStream);
            WriteableBitmap wbitmp = new WriteableBitmap(bmpImg);
            
            switch (angle)
            {
                case 90:
                case 270:
                    {
                        BitmapImageSource = RotateBitmap(wbitmp, wbitmp.PixelHeight,
                            wbitmp.PixelWidth, angle);
                        return BitmapImageSource;
                    }
                case 180:
                    {
                        BitmapImageSource = RotateBitmap(wbitmp, wbitmp.PixelWidth,
                            wbitmp.PixelHeight, angle);
                        return BitmapImageSource;
                    }
                default:
                    {
                        BitmapImageSource = wbitmp;
                        return BitmapImageSource;
                    }
            }

            //return RotateBitmap(wbitmp, bmpImg.PixelWidth, bmpImg.PixelHeight, angle);
        }

        int GetAngleFromExif(Stream imageStream)
        {
            ExifReader exf = new ExifReader(imageStream);
            ExifOrientation orientation = exf.info.Orientation;

            switch (orientation)
            {
                case ExifOrientation.TopRight:
                    return 90;
                case ExifOrientation.BottomRight:
                    return 180;
                case ExifOrientation.BottomLeft:
                    return 270;
                case ExifOrientation.Undefined:
                case ExifOrientation.TopLeft:
                default:
                    return 0;
            }
        }

        private WriteableBitmap RotateBitmap(WriteableBitmap source,
            int width, int height, int angle)
        {
            var target = new WriteableBitmap(width, height);
            
            int sourceIndex = 0;
            int targetIndex = 0;
            for (int x = 0; x < source.PixelWidth; x++)
            {
                for (int y = 0; y < source.PixelHeight; y++)
                {
                    sourceIndex = x + y * source.PixelWidth;
                    switch (angle)
                    {
                        case 90:
                            targetIndex = (source.PixelHeight - y - 1)
                                + x * target.PixelWidth;
                            break;
                        case 180:
                            targetIndex = (source.PixelWidth - x - 1)
                                + (source.PixelHeight - y - 1) * source.PixelWidth;
                            break;
                        case 270:
                            targetIndex = y + (source.PixelWidth - x - 1)
                                * target.PixelWidth;
                            break;
                    }
                    target.Pixels[targetIndex] = source.Pixels[sourceIndex];
                }
            }
            return target;
        }

        Action callbackAction;
        internal void ProcessCallback()
        {
            if (callbackAction != null)
                callbackAction.Invoke();
            callbackAction = null;
        }

        internal void NavigateBack()
        {
            INavigationService navigation = App.Container.Resolve<INavigationService>();

            navigation.NavigateTo(new Uri(ViewModelLocator.Camera, UriKind.RelativeOrAbsolute));
            
        }


        


        internal void UpdateImageAfterPinchComplete(int x, int y)
        {
          //  PhotoQwrekster.Service.Filters.StretchAroundPoint(BitmapImageSource, x, y);
            BitmapImageSource.Invalidate();

        }

        

        internal void UpdateImageAfterPinchComplete(int x, int y, double transformScale, int Radius)
        {
            if(transformScale > 1)
                PhotoQwrekster.Service.Filters.StretchAroundPoint(BitmapImageSource, x, y, transformScale, Radius);
            else
                PhotoQwrekster.Service.Filters.SqueezeAroundPoint(BitmapImageSource, x, y, transformScale, Radius);

            BitmapImageSource.Invalidate();
        }
    }
}