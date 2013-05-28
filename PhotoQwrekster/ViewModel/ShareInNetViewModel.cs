using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using PhotoQwrekster.Model;

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
    public class ShareInNetViewModel : ViewModelBase
    {
        

        /// <summary>
        /// Initializes a new instance of the ShareInNetViewModel class.
        /// </summary>
        public ShareInNetViewModel()
        {
            ActionsList = new ObservableCollection<ActionDescriptionModel>()
            { 
                new ActionDescriptionModel(){ ActionId=ActionDestination.Facebook, Description="facebuchek"},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Flikr, Description="flikr", UserName="aaaa"},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Picassa, Description="google+"},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Twitter, Description="twpid", UserName="bhh"},
                new ActionDescriptionModel(){ ActionId=ActionDestination.Tumblr, Description="tumbler"}
            };

        }

        private ObservableCollection<ActionDescriptionModel> _listWithActions = default(ObservableCollection<ActionDescriptionModel>);

        public ObservableCollection<ActionDescriptionModel> ActionsList
        {
            get
            {
                return _listWithActions;
            }
            set
            {
                if (_listWithActions == value)
                {
                    return;
                }

                var oldValue = _listWithActions;
                _listWithActions = value;

                RaisePropertyChanged("ActionsList");
            }
        }
    }
}