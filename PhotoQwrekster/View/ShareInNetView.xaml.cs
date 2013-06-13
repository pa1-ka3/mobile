using Microsoft.Phone.Controls;
using System.Windows.Controls;
using PhotoQwrekster.Model;
using PhotoQwrekster.ViewModel;

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

        ShareInNetViewModel viewModel; 

        private void ShareImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ActionDescriptionModel data = (sender as TextBlock).DataContext as ActionDescriptionModel;
            viewModel = this.DataContext as ShareInNetViewModel;
            viewModel.ShareImage(data);

        }
    }
}