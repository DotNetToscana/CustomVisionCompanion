using Acr.UserDialogs;
using CustomVisionCompanion.Common;
using CustomVisionCompanion.Services;
using CustomVisionCompanion.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Globalization;
using Xamarin.Forms;

namespace CustomVisionCompanion.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {

            SimpleIoc.Default.Register<IUserDialogs>(() => UserDialogs.Instance);
            SimpleIoc.Default.Register<IPermissionService, PermissionService>();
            SimpleIoc.Default.Register<IMediaService, MediaService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}
