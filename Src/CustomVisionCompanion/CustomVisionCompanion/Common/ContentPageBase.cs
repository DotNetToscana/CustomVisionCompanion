using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Messaging;
using CustomVisionCompanion.ViewModels;

namespace CustomVisionCompanion.Common
{
    public abstract class ContentPageBase : ContentPage
    {
        public ContentPageBase()
        {
            NavigationPage.SetHasNavigationBar(this, false);
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
