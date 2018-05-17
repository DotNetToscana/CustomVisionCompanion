using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMedia media;
        private readonly IPermissionService permissionService;

        public MediaService(IPermissionService permissionService)
        {
            media = CrossMedia.Current;
            this.permissionService = permissionService;
        }

        public async Task<MediaFile> TakePhotoAsync()
        {
            var permissions = await permissionService.CheckMultipleAsync(Permission.Camera, Permission.Storage);
            if (permissions.TryGetValue(Permission.Camera, out var cameraPermissionStatus) && cameraPermissionStatus == PermissionStatus.Granted
                && permissions.TryGetValue(Permission.Storage, out var storagePermissionStattus) && storagePermissionStattus == PermissionStatus.Granted)
            {
                // Take a photo using the camera.
                var file = await media.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = false,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    AllowCropping = false,
                    MaxWidthHeight = 1920,
                    CompressionQuality = 85
                });

                return file;
            }

            return null;
        }

        public async Task<MediaFile> PickPhotoAsync()
        {
            var permissions = await permissionService.CheckMultipleAsync(Permission.Photos, Permission.Storage);
            if (permissions.TryGetValue(Permission.Photos, out var photosPermissionStatus) && photosPermissionStatus == PermissionStatus.Granted
                && permissions.TryGetValue(Permission.Storage, out var storagePermissionStattus) && storagePermissionStattus == PermissionStatus.Granted)
            {
                // Pick a photo from the gallery.
                var file = await media.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 1920,
                    CompressionQuality = 85
                });

                return file;
            }

            return null;
        }
    }
}
