using CustomVisionCompanion.Common;
using Xamarin.Forms;

namespace CustomVisionCompanion.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public string Region
        {
            get => SettingsService.Region;
            set => SettingsService.Region = value;
        }

        public string ProjectName
        {
            get => SettingsService.ProjectName;
            set => SettingsService.ProjectName = value;
        }

        public string PredictionKey
        {
            get => SettingsService.PredictionKey;
            set => SettingsService.PredictionKey = value;
        }

        public string IterationId
        {
            get => SettingsService.IterationId;
            set => SettingsService.IterationId = value;
        }

        public AutoRelayCommand OpenCustomVisionWebSiteCommand { get; private set; }

        public SettingsViewModel()
        {
            CreateCommands();
        }

        private void CreateCommands()
        {
            OpenCustomVisionWebSiteCommand = new AutoRelayCommand(() => Device.OpenUri(Constants.CustomVisionPortal));
        }
    }
}
