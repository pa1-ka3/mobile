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
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.ComponentModel;
using PhotoQwrekster.Service;

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
            
            _transformMode = _dataService.GetTransformModeSettings();
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

        public WriteableBitmap BitmapImageSource
        {
            get 
            {
                return _dataService.GetBitmapImageSource();
            }
            set
            {
                _dataService.SetBitmapImageSource(value);
            }
        }
        private WriteableBitmap BitmapBackup { get; set; }
        private WriteableBitmap BitmapCalculated { get; set; }
        private WriteableBitmap BitmapProcessed { get; set; }

        internal System.Windows.Media.ImageSource GetCurrentImage()
        {
            BitmapImage bmpImg = new BitmapImage();
            
            byte[] imageData = _dataService.GetCapturedImageByte();
            if (imageData == null) return null;
            MemoryStream capturedStream = new MemoryStream(imageData); 
           
            int angle = GetAngleFromExif(capturedStream);
            bmpImg.SetSource(capturedStream);
            if (angle == 0 && bmpImg.PixelWidth > bmpImg.PixelHeight) angle = 90;

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


        internal void GetBitmapCopyForCalculation()
        {
            BitmapBackup = new WriteableBitmap(BitmapImageSource.PixelWidth, BitmapImageSource.PixelHeight);
            BitmapImageSource.Pixels.CopyTo(BitmapBackup.Pixels, 0);

            BitmapCalculated = new WriteableBitmap(BitmapImageSource.PixelWidth, BitmapImageSource.PixelHeight);
            BitmapImageSource.Pixels.CopyTo(BitmapCalculated.Pixels, 0);

        }

        internal void GetBitmapAfterCalculation()
        {
            BitmapCalculated.Pixels.CopyTo(BitmapImageSource.Pixels, 0);

            BitmapImageSource.Invalidate();
        }

        public bool isCalculating = false;

        internal void UpdateImageAfterPinchComplete(int x, int y, double transformScale, int radius, double deg,
            Action callback, 
            BackgroundWorker backgroundWorker)
        {
            radius = radius * 5;
            isCalculating = true;
            FloatPoint[,] fp;
            /*
            if (TransformMode) // transform in two directions
            {
                if (transformScale > 1)
                    PhotoQwrekster.Service.Filters.StretchAroundPoint(BitmapCalculated, BitmapBackup,
                        (int)(Mashtab * x), (int)Mashtab * y, transformScale, (int)Mashtab * radius, backgroundWorker);
                else
                    PhotoQwrekster.Service.Filters.SqueezeAroundPoint(BitmapCalculated, BitmapBackup,
                        (int)(Mashtab * x), (int)Mashtab * y, transformScale, (int)Mashtab * radius, backgroundWorker);
            }
            else // transform in one direction
            {*/
                if (transformScale > 1)
                    fp = PhotoQwrekster.Service.Filters.StretchInOneDimension(Width, Height,
                        (int)(Mashtab * x), (int)Mashtab * y, transformScale, (int)Mashtab * radius, deg, backgroundWorker);
                else
                {
                    fp = PhotoQwrekster.Service.Filters.SqueezeInOneDimension(Width, Height,
                         (int)(Mashtab * x), (int)Mashtab * y, transformScale, (int)Mashtab * radius, deg, backgroundWorker);
                    
                }
            //}

                Runner.Invoke(() =>
                {
                    GetBitmapCopyForCalculation();
                    Filters.OffsetFilterAntiAlias(BitmapCalculated, BitmapBackup, fp);

                    isCalculating = false;

                    if (callback != null)
                        callback.Invoke();
                });
        }

        public bool CanShare { get { return (BitmapImageSource != null); } }

        public double Mashtab { get; set; }

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
            Runner.Invoke(() =>
            {
                Height = (int)BitmapImageSource.PixelHeight;
                Width = (int)BitmapImageSource.PixelWidth;
            });
            return s;
        }

        public int Height { get; set; }
        public int Width { get; set; }


        private bool _transformMode;
        public bool TransformMode
        {
            get
            {
                return _transformMode;                
            }
            set {
                if (value != _transformMode)
                {
                    _transformMode = value;
                    _dataService.SaveTransformModeSettings(value);
                }
            }
        }


        public bool CanUndoClick
        {
            get
            {
                return (BitmapBackup!= null);                
            }
        }




        internal void UndoClicked()
        {
            BitmapBackup.Pixels.CopyTo(BitmapImageSource.Pixels, 0);
            BitmapBackup = null;
            BitmapImageSource.Invalidate();
        }

        internal string GetImageName()
        {
            return _dataService.GetImageName();
        }

        internal bool TrySaveImage(string fileName)
        {
            // Save picture to the library camera roll.
            MediaLibrary library = new MediaLibrary();
            using (var pics = library.Pictures)
            {
                using (var img = pics.LastOrDefault(pic => pic.Name == fileName))
                {
                    if (img == null)
                    {
                        // file doesn't exist
                        DoSaveImage(fileName);
                        return true;
                    }
                    else
                    {
                        // file does exist
                        return false;
                    }
                }
            }
        }

        internal void DoSaveImage(string fileName)
        {
             using (MemoryStream ms = new MemoryStream())
            {
                Extensions.SaveJpeg(BitmapImageSource, ms,
                    this.BitmapImageSource.PixelWidth, this.BitmapImageSource.PixelHeight, 0, 100);

                byte[] buffer = ms.ToArray();

                MediaLibrary library = new MediaLibrary();
                library.SavePictureToCameraRoll(fileName, buffer);
                _dataService.SetImageName(fileName);
            }
        }

        internal bool IsImageLarge()
        {
            if (BitmapImageSource.PixelHeight > 1000 || BitmapImageSource.PixelWidth > 1000)
                return true;
            else return false;
        }

        

        internal void RunMillion(BackgroundWorker worker)
        {
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(200);
                worker.ReportProgress(i);
            }
        }
    }
}