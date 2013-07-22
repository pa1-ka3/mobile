using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

// Directives
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using PhotoQwrekster.ViewModel;
using MetroInMotionUtils;


namespace PhotoQwrekster
{
    public partial class CameraPage : PhoneApplicationPage
    {

        CameraViewModel cameraViewModel;

        // Constructor
        public CameraPage()
        {
            InitializeComponent();
            cameraViewModel = this.DataContext as CameraViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            PhotoCamera cam = cameraViewModel.Initialize();
            if (cam != null)
            {
                //Set the VideoBrush source to the camera.
                viewfinderBrush.SetSource(cam);

                // The event is fired when the viewfinder is tapped (for focus).
                viewfinderCanvas.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(focus_Tapped);

                SetViewfinderBrushOrientation(this.Orientation);
            }
            else
            {
                // Disable UI.
                ShutterButton.IsEnabled = false;
                FlashButton.IsEnabled = false;
                AFButton.IsEnabled = false;
                ResButton.IsEnabled = false;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            cameraViewModel.ReleaseResources();

            
        }

        // Ensure that the viewfinder is upright in LandscapeRight.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cameraViewModel.IsCameraPresent())
            {               
                base.OnOrientationChanged(e);
                PageOrientation orient = e.Orientation;
                SetViewfinderBrushOrientation(orient);
            }            
        }

        private void SetViewfinderBrushOrientation(PageOrientation orient)
        {
            switch (orient)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    viewfinderBrush.RelativeTransform =
                    new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                    break;
                case PageOrientation.LandscapeRight:
                    viewfinderBrush.RelativeTransform =
                    new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 180 };
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    viewfinderBrush.RelativeTransform =
                    new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };
                    break;
                case PageOrientation.PortraitDown:
                    viewfinderBrush.RelativeTransform =
                    new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 270 };
                    break;
            }
        }

        // Provide touch focus in the viewfinder.
        void focus_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Determine location of tap.
            Point tapLocation = e.GetPosition(viewfinderCanvas);
            cameraViewModel.focusTapped(tapLocation, viewfinderCanvas.Width, viewfinderCanvas.Height);       
     
             
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // this means App goes to exit
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            e.Cancel = true;

            // create the list of items to peel
            var peelList = new FrameworkElement[] { UpperGridRow, viewfinderCanvas };

            peelList.Peel(() =>
            {
                App.Quit();
            });
            base.OnBackKeyPress(e);  
        }

        private void ShutterButton_Click(object sender, EventArgs e)
        {
            cameraViewModel.ShutterButton_Click();
        }

        private void AFButton_Click(object sender, EventArgs e)
        {
            //cameraViewModel.focus_Clicked();
            //PrepareScreenShot();
            cameraViewModel.LoadImageFromLibrary(SetBackgroundFromLibrary);
        }

        void SetBackgroundFromLibrary(byte[] img)
        {
            MemoryStream capturedStream = new MemoryStream(img);
            BitmapImage bmpImg = new BitmapImage();
            bmpImg.SetSource(capturedStream);
            WriteableBitmap wbitmp = new WriteableBitmap(bmpImg);

            BackgroundImageBrush.ImageSource = wbitmp;// RotateBitmap(wbitmp, wbitmp.PixelWidth,  wbitmp.PixelHeight, 0);
            BackgroundImageBrush.Opacity = 1;
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

        private void PrepareScreenShot()
        {
            

            // Display that on the screen
            BackgroundImageBrush.ImageSource = cameraViewModel.PrepareScreenShot();
            BackgroundImageBrush.Opacity = 1;
        }


        private void SkipButton_Click(object sender, EventArgs e)
        {
            cameraViewModel.skip_clicked();
        }
    }
}
