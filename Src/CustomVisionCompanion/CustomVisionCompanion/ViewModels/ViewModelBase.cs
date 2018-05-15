using System;
using GalaSoft.MvvmLight.Views;
using Acr.UserDialogs;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using CustomVisionCompanion.Common;
using GalaSoft.MvvmLight.Ioc;

namespace CustomVisionCompanion.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected IUserDialogs DialogService { get; }

        public ViewModelBase()
        {
            DialogService = SimpleIoc.Default.GetInstance<IUserDialogs>();
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (SetBusy(value) && !isBusy)
                {
                    BusyMessage = null;
                }
            }
        }

        private string busyMessage;
        public string BusyMessage
        {
            get => busyMessage;
            set => Set(ref busyMessage, value, broadcast: true);
        }

        public bool SetBusy(bool value, string message = null)
        {
            BusyMessage = message;

            var isSet = Set(() => IsBusy, ref isBusy, value, broadcast: true);
            if (isSet)
            {
                OnIsBusyChanged();
            }

            return isSet;
        }

        protected virtual void OnIsBusyChanged()
        {
        }

        public virtual void Activate(object parameter)
        {
        }

        public virtual void Deactivate()
        {
        }

        protected async Task ShowErrorAsync(string message, Exception ex = null)
        {
            DialogService.HideLoading();
            await DialogService.AlertAsync(message);
        }
    }
}