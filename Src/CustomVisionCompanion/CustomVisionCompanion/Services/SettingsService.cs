using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Services
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private const string PREDICTION_KEY = nameof(PREDICTION_KEY);
        private const string PROJECT_ID = nameof(PROJECT_ID);

        private readonly ISettings settings;

        public SettingsService()
        {
            settings = CrossSettings.Current;
        }

        public string PredictionKey
        {
            get => settings.GetValueOrDefault(PREDICTION_KEY, null);
            set => settings.AddOrUpdateValue(PREDICTION_KEY, value);
        }

        public string ProjectId
        {
            get => settings.GetValueOrDefault(PROJECT_ID, Guid.Empty.ToString("D"));
            set => settings.AddOrUpdateValue(PROJECT_ID, value);
        }
    }
}