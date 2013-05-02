using Microsoft.Phone.Controls;
using PhotoQwrekster.ViewModel;
using System.Windows;
using System;
using System.Windows.Media.Imaging;

namespace PhotoQwrekster.View
{
    /// <summary>
    /// Description for ImageProcessingView.
    /// </summary>
    public partial class ImageProcessingView : PhoneApplicationPage
    {

        ImageProcessingViewModel imageProcessingViewModel; 
        /// <summary>
        /// Initializes a new instance of the ImageProcessingView class.
        /// </summary>
        public ImageProcessingView()
        {
            InitializeComponent();
            imageProcessingViewModel = this.DataContext as ImageProcessingViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            imgSelectedPhoto.Source = imageProcessingViewModel.GetCurrentImage();
            string isCallback;
            if (NavigationContext.QueryString.TryGetValue("callback", out isCallback))
            {
                if (isCallback == "true")
                {
                    imageProcessingViewModel.ProcessCallback();
                }
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            imageProcessingViewModel.NavigateBack();
               
        }

        double initialScale;
        private void OnPinchStarted(object sender, Input.PinchStartedGestureEventArgs e)
        {
            //  flickData.Text = "started";

            //   initialAngle = transform.Rotation;
            initialScale = transform.ScaleX = transform.ScaleY = 1;
            GestureElipse.Height = GestureElipse.Width = 100;
            Point p1 = e.GetPosition(imgSelectedPhoto, 0);
            Point p2 = e.GetPosition(imgSelectedPhoto, 1);

            GestureElipse.Margin = GetMargin(p1, p2);
            GestureElipse.Visibility = System.Windows.Visibility.Visible;
            flickData.Text = string.Format("Pinch: Touch1 {0},{1} Touch2 {2},{3}  Margin [{4},{5},{6},{7}]",

               p1.X, p1.Y,
               p2.X, p2.Y,
               GestureElipse.Margin.Left, GestureElipse.Margin.Top, GestureElipse.Margin.Right, GestureElipse.Margin.Bottom
               );

        }

        public Point GestureCenter { get; set; }

        private Thickness GetMargin(Point point1, Point point2)
        {
            Thickness t = new Thickness();
            Point center = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
            double radius = (Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2))) / 2;
            t.Top = (int)(center.Y - radius);
            t.Left = (int)(center.X - radius);
            t.Right = imgSelectedPhoto.ActualWidth - (int)(center.X + radius);
            t.Bottom = imgSelectedPhoto.ActualHeight - (int)(center.Y + radius);

            GestureElipse.Height = GestureElipse.Width = 2 * (int)radius;
            centerData.Text = string.Format("Center: {0},{1}  radius {2}  ImgActual [w {3}, h {4}]",

               center.X, center.Y,
               (int)radius,
               imgSelectedPhoto.Width.ToString(), imgSelectedPhoto.Height.ToString()
               );

            GestureCenter = center;
            Radius = (int)radius;
            return t;
        }

        private void OnPinchDelta(object sender, Input.PinchGestureEventArgs e)
        {

            //    transform.Rotation = initialAngle + e.TotalAngleDelta;
            transform.ScaleX = transform.ScaleY = initialScale * e.DistanceRatio;

            flickData.Text = string.Format("transform.ScaleX: {0}",
              transform.ScaleX
               );


        }

        private int Radius { get; set; }

        private void OnPinchCompleted(object sender, Input.PinchGestureEventArgs e)
        {
            //     border.Fill = normalBrush;
            GestureElipse.Visibility = System.Windows.Visibility.Collapsed;

            imageProcessingViewModel.UpdateImageAfterPinchComplete((int)GestureCenter.X, (int)GestureCenter.Y, transform.ScaleX, Radius);
        }

        

    }
}