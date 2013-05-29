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
using Microsoft.Phone.Tasks;
using System.Windows;
using System.Collections.Generic;

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

        PhotoChooserTask photoChooser = null; 

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

            photoChooser = new PhotoChooserTask();
            photoChooser.Completed += new EventHandler<PhotoResult>(photoChooser_Completed);
        }

        void photoChooser_Completed(object sender, PhotoResult e)
        {            
            App app = Application.Current as App;

            if (e.TaskResult == TaskResult.OK)
            {
                byte[] data;
                using (var br =  new BinaryReader(e.ChosenPhoto)) 
                    data = br.ReadBytes((int)e.ChosenPhoto.Length);

                _dataService.SetCupturedImageByte(data);

                if (imageChooseComplete != null)
                    imageChooseComplete.Invoke();
                imageChooseComplete = null;             
            }
        }

        private Action imageChooseComplete = null;

        internal void LoadImageFromLibrary(Action callback)
        {
            imageChooseComplete = callback;
            photoChooser.ShowCamera = false;
            photoChooser.Show();
        }

        #region Commands


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

                // write an image into the stream
                Extensions.SaveJpeg(BitmapImageSource, ms,
                    this.BitmapImageSource.PixelWidth, this.BitmapImageSource.PixelHeight, 0, 100);


                byte[] buffer = ms.ToArray();

                FacebookHelper.UploadMedia(buffer, "suprematizm11");

            }
        }

        #endregion

        public WriteableBitmap BitmapImageSource { get; set; }

        internal System.Windows.Media.ImageSource GetCurrentImage()
        {
            BitmapImage bmpImg = new BitmapImage();
            
            //bmpImg.SetSource(picCurrentImage.GetImage());
            // bmpImg.SetSource(library.Pictures[library.Pictures.Count - 1].GetImage());
            byte[] imageData = _dataService.GetCapturedImageByte();
            if (imageData == null) return null;
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


        

        internal void UpdateImageAfterPinchComplete(int x, int y, double transformScale, int Radius)
        {
            if (TransformMode) // transform in two directions
            {
                if (transformScale > 1)
                    PhotoQwrekster.Service.Filters.StretchAroundPoint(BitmapImageSource,
                        (int)(Mashtab * x), (int)Mashtab * y, transformScale, (int)Mashtab * Radius);
                else
                    PhotoQwrekster.Service.Filters.SqueezeAroundPoint(BitmapImageSource,
                        (int)(Mashtab * x), (int)Mashtab * y, transformScale, (int)Mashtab * Radius);
            }
            else // transform in one direction
            {
            }

            BitmapImageSource.Invalidate();
        }

        public bool CanShare { get { return (BitmapImageSource != null); } }

        private double Mashtab { get; set; }

        public Size GetImageRecalculatedSize()
        {
            if (BitmapImageSource == null) return new Size(0,0);

            Size s = new Size(BitmapImageSource.PixelWidth, BitmapImageSource.PixelHeight);
            if (s.Height / 640 > s.Width / 480)
            {
                double mashtb = s.Height / 640;
                s = new Size(s.Width / mashtb, s.Height / mashtb);
                Mashtab = mashtb;
            }
            else
            {
                double mashtb = s.Width / 480;
                s = new Size(s.Width / mashtb, s.Height / mashtb);
                Mashtab = mashtb;
            }
            return s;
        }

        public bool TransformMode { get; set; }
    }
}