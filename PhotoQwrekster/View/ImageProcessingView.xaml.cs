using Microsoft.Phone.Controls;
using PhotoQwrekster.ViewModel;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Media;
using MetroInMotionUtils;
using Phone.Controls;

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
            imageRecalculator = new System.ComponentModel.BackgroundWorker();
            imageRecalculator.WorkerReportsProgress = true;
            imageRecalculator.DoWork += new System.ComponentModel.DoWorkEventHandler(imageRecalculate_DoWork);
            imageRecalculator.ProgressChanged += new ProgressChangedEventHandler(imageRecalculator_ProgressChanged);
            imageRecalculator.RunWorkerCompleted += new RunWorkerCompletedEventHandler(imageRecalculator_RunWorkerCompleted);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetSelectedPhotoSource();
            string isCallback;
            if (NavigationContext.QueryString.TryGetValue("callback", out isCallback))
            {
                if (isCallback == "true")
                {
                    imageProcessingViewModel.ProcessCallback();
                }
            }
            TransformMode.IsChecked = imageProcessingViewModel.TransformMode;
            Radius = 120;
            SetLinesBoundaries(new Point(220, 220), new Point(30, 120));
            SetGesturesLinesVisibility(System.Windows.Visibility.Collapsed);
            //performanceProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            GestureElipse.Visibility = System.Windows.Visibility.Collapsed;
            TransformElipse.Visibility = System.Windows.Visibility.Collapsed;
            if (NavigationContext.QueryString.ContainsKey("loadimage"))
            {
                NavigationContext.QueryString.Clear();
                imageProcessingViewModel.LoadImageFromLibrary(SetSelectedPhotoSource);
            }
        }

        private void SetSelectedPhotoSource()
        {
            ((Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false; //Undo
            
         
            System.Windows.Media.ImageSource img = imageProcessingViewModel.GetCurrentImage();
            if (img != null)
            {
                imgSelectedPhoto.Source = img;
                ((Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;  //"Save" 
                ((Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;  //"Share" 
                flickData.Visibility = System.Windows.Visibility.Visible;
                flickData.Text = "try to transform image with gesture";
                Size s = imageProcessingViewModel.GetImageRecalculatedSize();
                
                imgSelectedPhoto.Height = s.Height;
                imgSelectedPhoto.Width = s.Width;
            }
            else
            {
                imgSelectedPhoto.Source = null;
                flickData.Visibility = System.Windows.Visibility.Visible;
                flickData.Text = "No image selected";
                ((Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;  //"Save" 
                ((Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = false;  //"Share"       
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            e.Cancel = true;
            // create the list of items to peel
            var peelList = new FrameworkElement[] { TitlePanel, imgSelectedPhoto };

            peelList.Peel(() =>
            {
                imageProcessingViewModel.NavigateBack();
            });
               
        }

        double initialScale;
        private void OnPinchStarted(object sender, Input.PinchStartedGestureEventArgs e)
        {
            if (imageProcessingViewModel.isCalculating) return;
            initialScale = transform.ScaleX = transform.ScaleY = 1;

            Point p1 = e.GetPosition(ContentPanel, 0);
            Point p2 = e.GetPosition(ContentPanel, 1);

            GestureElipse.Height = GestureElipse.Width = TransformElipse.Height = TransformElipse.Width = 100;
            Radius = GetRadius(p1, p2); 
        //    GestureElipse.Margin = TransformElipse.Margin = GetEllipseMargin(p1, p2);
            SetGesturesLinesVisibility(System.Windows.Visibility.Visible);
            GestureElipse.Visibility = TransformElipse.Visibility = System.Windows.Visibility.Visible;

            SetLinesBoundaries(p1, p2);
            flickData.Visibility = System.Windows.Visibility.Collapsed;
       /*     flickData.Text = string.Format("Pinch: Touch1 {0},{1} Touch2 {2},{3}  Margin [{4},{5},{6},{7}]",

               p1.X, p1.Y,
               p2.X, p2.Y,
               GestureElipse.Margin.Left, GestureElipse.Margin.Top, GestureElipse.Margin.Right, GestureElipse.Margin.Bottom
               );*/
        }

        private void SetGesturesLinesVisibility(System.Windows.Visibility visibility)
        {
            Line1.Visibility = visibility;
            Line2.Visibility = visibility;
        }

        private void SetLinesBoundaries(Point p1, Point p2)
        {
            if (imageProcessingViewModel.TransformMode) // transform in two directions
            {
                Line1.Visibility = Visibility.Visible;
                Line2.Visibility = Visibility.Visible;

                double centrX = (p2.X + p1.X) / 2;
                double centrY = (p2.Y + p1.Y) / 2;

                Line1.X1 = Line2.X1 = centrX - 70;
                Line1.X2 = Line2.X2 = centrX + 70;

                Line1.Y1 = Line2.Y2 = centrY - 70;
                Line1.Y2 = Line2.Y1 = centrY + 70;

                double ellipseRadius = GetRadius(p1, p2);
                

                TransformElipse.Margin = GestureElipse.Margin = new Thickness(centrX - ellipseRadius,
                    centrY - ellipseRadius,
                    -ellipseRadius,
                    -ellipseRadius);

                TransformElipse.Height = GestureElipse.Height = 2 * ellipseRadius;
                TransformElipse.Width = GestureElipse.Width = 2 * ellipseRadius;

            }
            else
            {
                Line1.Visibility = Visibility.Visible;
                Line2.Visibility = Visibility.Visible;

                Line1.X1 = p1.X;
                Line1.Y1 = p1.Y;
                Line1.X2 = p2.X;
                Line1.Y2 = p2.Y;

                double angle = Math.Atan((p2.Y - p1.Y)/(p2.X - p1.X));
      
                double centrX = (p2.X + p1.X) / 2;
                double centrY = (p2.Y + p1.Y) / 2;

                double dxL2 = 30 * Math.Sin(angle);
                double dyL2 = 30 * Math.Cos(angle);
 
                Line2.X1 = centrX - dxL2;
                Line2.Y1 = centrY + dyL2;

                Line2.X2 = centrX + dxL2;
                Line2.Y2 = centrY - dyL2;


                nontransform.Rotation = transform.Rotation = (angle * 180) / Math.PI + 90;

                double ellipseRadius = 210;
                double ellipseSemiHeight = 50;

                TransformElipse.Margin = GestureElipse.Margin = new Thickness(centrX - ellipseRadius,
                    centrY - ellipseSemiHeight,
                    -ellipseRadius,
                    -ellipseRadius);

                TransformElipse.Height = GestureElipse.Height = 2 * ellipseSemiHeight;
                TransformElipse.Width = GestureElipse.Width = 2 * ellipseRadius;

                /*centerData.Text = string.Format("Center: {0},{1}  Line2 {2},{3} - {4},{5}",
                    centrX, centrY,
                    Line2.X1, Line2.Y1, Line2.X2, Line2.Y2);
                */
            }
        }

        public Point GestureCenter { get; set; }

        private Thickness GetEllipseMargin(Point point1, Point point2)
        {
            Thickness t = new Thickness();
            Point center = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
            double radius = GetRadius(point1, point2);
            t.Top = (int)(center.Y - radius);
            t.Left = (int)(center.X - radius);
            t.Right = 480 - (int)(center.X + radius);
            t.Bottom = 640 - (int)(center.Y + radius);
                        
            return t;
        }

        private static double GetRadius(Point point1, Point point2)
        {
            double radius = (Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2))) / 2;

            return radius;
        }

        private void OnPinchDelta(object sender, Input.PinchGestureEventArgs e)
        {
            
            if (imageProcessingViewModel.isCalculating) return;
            transform.ScaleX = transform.ScaleY = initialScale * Math.Sqrt(e.DistanceRatio);

            Point p1 = e.GetPosition(ContentPanel, 0);
            Point p2 = e.GetPosition(ContentPanel, 1);

            SetLinesBoundaries(p1, p2);

        }

        private double Radius { get; set; }
        private BackgroundWorker imageRecalculator;
        private double ScaleX { get; set; }

        private void OnPinchCompleted(object sender, Input.PinchGestureEventArgs e)
        {
            if (imageProcessingViewModel.isCalculating) return;
            GestureElipse.Visibility = TransformElipse.Visibility = System.Windows.Visibility.Collapsed;

            Point p1 = e.GetPosition(imgSelectedPhoto, 0);
            Point p2 = e.GetPosition(imgSelectedPhoto, 1);
            lock (this)
            {
                angle = - Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));

                centrX = (int)(p2.X + p1.X) / 2;
                centrY = (int)(p2.Y + p1.Y) / 2;
                ScaleX = transform.ScaleX;
            }
            //performanceProgressBar.Visibility = System.Windows.Visibility.Visible;
            if (imageProcessingViewModel.IsImageLarge())
            {
                flickData.Visibility = System.Windows.Visibility.Visible;
                flickData.Text = "image is very large. there would be performance difficulties";
            }
            imageRecalculator.RunWorkerAsync();
        }

        private int centrX;
        private int centrY;
        private double angle;

        void imageRecalculate_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
#if DEBUG
       //     Console.WriteLine("backgroundSynchronizer_DoWork");
#endif

            DoCalculation(new object());
        }

        private void DoCalculation(object o)
        {
            if (this.progress == null)
            {
                this.progress = new ProgressIndicator();
            }
            
           // progress.ProgressBar.Foreground = new SolidColorBrush(Colors.Green);
            // Change some opacity
            //progress.Opacity = 0.9;
            
            progressValue = 0;
            ShowIndicator(true);
                imageProcessingViewModel.UpdateImageAfterPinchComplete((int)centrX, (int)centrY, ScaleX, (int)Radius, angle,
                        OnUpdateFinished, imageRecalculator);
            

        }
        private int progressValue;

        void imageRecalculator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                progressValue += 5;
                progress.ProgressBar.Value = progressValue;
            }
        );

        }

        void imageRecalculator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("completed");
        }

        private void Load_Click(object sender, EventArgs e)
        {
            imageProcessingViewModel.LoadImageFromLibrary(SetSelectedPhotoSource);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            save_panel.IsOpen = true;
            ImageNameEditBox.Text = imageProcessingViewModel.GetImageName();
        }


        private void btn_save_continue_Click(object sender, RoutedEventArgs e)
        {
            save_panel.IsOpen = false;
            if (!imageProcessingViewModel.TrySaveImage(ImageNameEditBox.Text))
            {
                OverwritePromt.Text = string.Format("{0} already exist. overwrite?", ImageNameEditBox.Text);
                save_overwrite_prompt.IsOpen = true;
            }
            
        }

        private void btn_save_cancel_Click(object sender, RoutedEventArgs e)
        {
            save_panel.IsOpen = false;
        }

        private void btn_save_overwrite_Click(object sender, RoutedEventArgs e)
        {
            save_overwrite_prompt.IsOpen = false;
            imageProcessingViewModel.DoSaveImage(ImageNameEditBox.Text);
        }

        private void btn_overwrite_cancel_Click(object sender, RoutedEventArgs e)
        {
            save_overwrite_prompt.IsOpen = false;
            save_panel.IsOpen = true;
        }

        
        private void Share_Click(object sender, EventArgs e)
        {
            if(imageProcessingViewModel.CanShare)
                NavigationService.Navigate(new Uri("/View/ShareInNetView.xaml", UriKind.Relative));
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            imageProcessingViewModel.UndoClicked();
            ((Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = imageProcessingViewModel.CanUndoClick; //Undo
        }

        private void TransformMode_Checked(object sender, RoutedEventArgs e)
        {
            if (TransformMode.IsChecked.HasValue)
                imageProcessingViewModel.TransformMode = TransformMode.IsChecked.Value;
            else
                imageProcessingViewModel.TransformMode = false;

        }

        private void TransformMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (TransformMode.IsChecked.HasValue)
                imageProcessingViewModel.TransformMode = TransformMode.IsChecked.Value;
            else
                imageProcessingViewModel.TransformMode = false;


        }

        private void imgSelectedPhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           // Point p = e.GetPosition(imgSelectedPhoto);
            //centerData.Text = string.Format("Tap: {0},{1}", p.X, p.Y);
        }

        private void imgSelectedPhoto_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lock (this)
            {
                angle = 30;
                Point p = e.GetPosition(imgSelectedPhoto);
                centrX = (int)p.X;
                centrY = (int)p.Y;
                transform.ScaleX = 0.2;
                Radius = 50;
            }
            //performanceProgressBar.Visibility = System.Windows.Visibility.Visible;
        //    CalculatingPanel.Visibility = System.Windows.Visibility.Visible;
            flickData.Visibility = System.Windows.Visibility.Visible;
            if (imageProcessingViewModel.IsImageLarge())
            {   
                flickData.Text = "image is very large. there would be performance difficulties";
            }
            if(!imageRecalculator.IsBusy)
                //imageRecalculator.RunWorkerAsync(); 
                DoCalculation(new object());
        }

        private void OnUpdateFinished()
        {
            Runner.Invoke(() =>
            {
                imageProcessingViewModel.GetBitmapAfterCalculation();
                ((Microsoft.Phone.Shell.ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = imageProcessingViewModel.CanUndoClick; //Undo
                //performanceProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                flickData.Visibility = System.Windows.Visibility.Collapsed;
         //       CalculatingPanel.Visibility = System.Windows.Visibility.Collapsed;
                SetGesturesLinesVisibility(System.Windows.Visibility.Collapsed);
                ShowIndicator(false);
            });
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            progress.ProgressType = ProgressTypes.CustomMiddle;
            progress.Text = "Calculating...";
            ShowIndicator(false);
        }

        private void ShowIndicator(bool isShow)
        {
            Runner.Invoke(() =>
           {
               progress.ProgressBar.Value = 0;
               indicator_panel.IsOpen = isShow;
               progress.Visibility = isShow ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
           });
        }







        
    }
}