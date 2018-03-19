﻿using CustomVisionCompanion.Common;
using CustomVisionCompanion.Services;
using Microsoft.Cognitive.CustomVision.Prediction;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.CustomVisionEngine;
using Plugin.CustomVisionEngine.Models;

namespace CustomVisionCompanion.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IMedia mediaService;
        private readonly IPermissions permissionsService;

        private IEnumerable<string> predictions;
        public IEnumerable<string> Predictions
        {
            get => predictions;
            set => Set(ref predictions, value);
        }

        private bool isOffline;
        public bool IsOffline
        {
            get => isOffline;
            set => Set(ref isOffline, value);
        }

        private Stream image;
        public Stream Image
        {
            get => image;
            set => Set(ref image, value);
        }

        public AutoRelayCommand TakePhotoCommand { get; private set; }

        public AutoRelayCommand PickPhotoCommand { get; private set; }

        public MainViewModel(IPermissions permissionsService, IMedia mediaService)
        {
            this.mediaService = mediaService;
            this.permissionsService = permissionsService;

            CreateCommands();
        }

        private void CreateCommands()
        {
            TakePhotoCommand = new AutoRelayCommand(async () => await TakePhotoAsync(), () => !IsBusy).DependsOn(nameof(IsBusy));
            PickPhotoCommand = new AutoRelayCommand(async () => await PickPhotoAsync(), () => !IsBusy).DependsOn(nameof(IsBusy));
        }

        private async Task TakePhotoAsync()
        {
            await AnalyzeAsync(Permission.Camera, async () =>
            {
                // Take a photo using the camera.
                var file = await mediaService.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = false,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    AllowCropping = false,
                    MaxWidthHeight = 1920,
                    CompressionQuality = 85
                });

                return file;
            });
        }

        private async Task PickPhotoAsync()
        {
            await AnalyzeAsync(Permission.Photos, async () =>
            {
                // Pick a photo from the gallery.
                var file = await mediaService.PickPhotoAsync();

                return file;
            });
        }

        private async Task AnalyzeAsync(Permission permissionType, Func<Task<MediaFile>> GetImageAsync)
        {
            IsBusy = true;

            try
            {
                var status = PermissionStatus.Granted;

                // Only on iOS we need to explicitly checks for permission.
                // On Android, if users are running Marshmallow the Media Plugin will automatically prompt them for runtime permissions.
                if (Device.RuntimePlatform == Device.iOS)
                {
                    status = await permissionsService.CheckPermissionStatusAsync(permissionType);
                    if (status != PermissionStatus.Granted)
                    {
                        if (await permissionsService.ShouldShowRequestPermissionRationaleAsync(permissionType))
                        {
                            await DialogService.AlertAsync($"This app needs access to {permissionType}, please accept the request.", Constants.AppName);
                        }

                        var results = await permissionsService.RequestPermissionsAsync(permissionType);

                        //Best practice to always check that the key exists
                        if (results.ContainsKey(permissionType))
                        {
                            status = results[permissionType];
                        }
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    // Clean up previous results.
                    CleanUp();

                    await mediaService.Initialize();

                    try
                    {
                        var file = await GetImageAsync();
                        await AnalyzePhotoAsync(file);
                    }
                    catch (MediaPermissionException)
                    {
                        await DialogService.AlertAsync($"You need to allow {permissionType} access in order to use the app.", Constants.AppName);
                    }
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DialogService.AlertAsync("Cannot continue, check app settings for the right permission.", Constants.AppName);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AnalyzePhotoAsync(MediaFile file)
        {
            if (file != null)
            {
                IsBusy = true;

                try
                {
                    Image = file.GetStream();

                    // Check whether to use the online or offline version of the prediction model.
                    IEnumerable<Recognition> predictionsRecognized = null;
                    if (isOffline)
                    {
                        var classifier = CrossOfflineClassifier.Current;
                        predictionsRecognized = await classifier.RecognizeAsync(file.GetStream(), file.Path);
                    }
                    else
                    {
                        var classifier = CrossOnlineClassifier.Current;
                        predictionsRecognized = await classifier.RecognizeAsync(SettingsService.PredictionKey, SettingsService.ProjectId, file.GetStream());
                    }

                    Predictions = predictionsRecognized.Select(p => $"{p.Tag}: {p.Probability:P1}");
                    file.Dispose();
                }
                catch (Exception ex)
                {
                    await DialogService.AlertAsync(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void CleanUp()
        {
            Predictions = null;
            Image?.Dispose();
            Image = null;
        }
    }
}
