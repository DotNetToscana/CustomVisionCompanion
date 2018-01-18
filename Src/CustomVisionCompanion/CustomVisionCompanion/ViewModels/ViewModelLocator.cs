using Acr.UserDialogs;
using CustomVisionCompanion.Common;
using CustomVisionCompanion.Services;
using CustomVisionCompanion.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using PriamoAppPsc.ViewModels;
using System.Globalization;
using Xamarin.Forms;

namespace CustomVisionCompanion.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            var navigationService = new NavigationService();
            navigationService.Configure(Constants.MainPage, typeof(MainPage));

            SimpleIoc.Default.Register<NavigationService>(() => navigationService);

            SimpleIoc.Default.Register<IUserDialogs>(() => UserDialogs.Instance);
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();            

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}
