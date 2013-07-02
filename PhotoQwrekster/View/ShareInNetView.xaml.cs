using Microsoft.Phone.Controls;
using System.Windows.Controls;
using PhotoQwrekster.Model;
using PhotoQwrekster.ViewModel;
using MetroInMotionUtils;
using System.Windows;
using System.Linq;

namespace PhotoQwrekster.View
{
    /// <summary>
    /// Description for ShareInNetView.
    /// </summary>
    public partial class ShareInNetView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the ShareInNetView class.
        /// </summary>
        public ShareInNetView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("result"))
            {
                string result = NavigationContext.QueryString["result"];
              //  viewModel = this.DataContext as ShareInNetViewModel;
                //viewModel.ShowPopupNotification = true;
               // viewModel.NotificationMessage = result;
                upload_result_notification.IsOpen = true;
                Notification.Text = result;
                performanceProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        ShareInNetViewModel viewModel; 

        private void ShareImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            performanceProgressBar.Visibility = System.Windows.Visibility.Visible;
            ActionDescriptionModel data = (sender as TextBlock).DataContext as ActionDescriptionModel;
            viewModel = this.DataContext as ShareInNetViewModel;
            viewModel.ShareImage(data);

        }

        private void btn_ok_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            upload_result_notification.IsOpen = false;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var listItems = ActionsListBox.GetItemsInView().ToList();
            // create the list of items to peel
            var peelList = new FrameworkElement[] { TitlePanel }.Union(listItems);

            peelList.Peel(() =>
            {
                NavigationService.Navigate(new System.Uri("/View/ImageProcessingView.xaml", System.UriKind.RelativeOrAbsolute));
            });
        }

        private void LoginLogout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ActionDescriptionModel data = (sender as TextBlock).DataContext as ActionDescriptionModel;
            viewModel = this.DataContext as ShareInNetViewModel;
            viewModel.LoginLogout(data);

        }
    }
}