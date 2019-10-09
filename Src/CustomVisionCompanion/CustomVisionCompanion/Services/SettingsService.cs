using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace CustomVisionCompanion.Services
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly ISettings settings;

        public SettingsService()
        {
            settings = CrossSettings.Current;
        }

        public string Region
        {
            get => settings.GetValueOrDefault(nameof(Region), null);
            set => settings.AddOrUpdateValue(nameof(Region), value);
        }

        public string ProjectName
        {
            get => settings.GetValueOrDefault(nameof(ProjectName), null);
            set => settings.AddOrUpdateValue(nameof(ProjectName), value);
        }

        public string PredictionKey
        {
            get => settings.GetValueOrDefault(nameof(PredictionKey), null);
            set => settings.AddOrUpdateValue(nameof(PredictionKey), value);
        }

        public string IterationId
        {
            get => settings.GetValueOrDefault(nameof(IterationId), Guid.Empty.ToString("D"));
            set => settings.AddOrUpdateValue(nameof(IterationId), value);
        }
    }
}