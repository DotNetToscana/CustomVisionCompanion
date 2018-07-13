using CustomVisionCompanion.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace CustomVisionCompanion.Common
{
    public abstract class ContentPageBase : ContentPage
    {
        public ContentPageBase()
        {
            On<iOS>().SetUseSafeArea(true);
            //On<iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Never);
            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, String.Empty);
        }

        protected override void OnAppearing()
        {
            if (BindingContext is ViewModelBase viewModel && !viewModel.IsActive)
            {
                viewModel.Activate(this.GetNavigationArgs());
                viewModel.IsActive = true;
            }
            else if (App.IsPausing)
            {
                App.IsPausing = false;
            }

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (BindingContext is ViewModelBase viewModel && !App.IsPausing)
            {
                viewModel.Deactivate();
                viewModel.IsActive = false;
            }

            Messenger.Default.Unregister(this);
            base.OnDisappearing();
        }
    }
}
