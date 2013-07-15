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
            cameraViewModel.focus_Clicked();
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            cameraViewModel.skip_clicked();
        }

        private void Load_Click(object sender, EventArgs e)
        {
            cameraViewModel.LoadImageFromLibrary(NavigateToProcessingPage);
        }

        private void NavigateToProcessingPage()
        {   
            cameraViewModel.skip_clicked();
        }
    }
}
