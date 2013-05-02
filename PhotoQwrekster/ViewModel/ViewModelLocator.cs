/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:PhotoQwrekster.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PhotoQwrekster.Model;

namespace PhotoQwrekster.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                //SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
                App.Container.Register<IDataService>(c => new Design.DesignDataService());// (c => new StorageSelectorViewModel());
            }
            else
            {
                //SimpleIoc.Default.Register<IDataService, DataService>();
                App.Container.Register<IDataService>(c => new DataService());
            }

            //SimpleIoc.Default.Register<MainViewModel>();
            App.Container.Register<CameraViewModel>((c) => new CameraViewModel(App.Container.Resolve<IDataService>()));
            App.Container.Register<ImageProcessingViewModel>((c) => new ImageProcessingViewModel(App.Container.Resolve<IDataService>()));
            App.Container.Register<INavigationService>(c => new NavigationService());
            
        }

        public const string ImageProcessing = "/View/ImageProcessingView.xaml";
        public const string OAuthLogin = "/View/OAuthLogin.xaml";
        public const string Camera = "/View/CameraPage.xaml";

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CameraViewModel MainCamera
        {
            get
            {
                //return ServiceLocator.Current.GetInstance<MainViewModel>();
                return App.Container.Resolve<CameraViewModel>();
            }
        }

        public ImageProcessingViewModel ImageViewModel
        {
            get
            {
                //return ServiceLocator.Current.GetInstance<MainViewModel>();
                return App.Container.Resolve<ImageProcessingViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}