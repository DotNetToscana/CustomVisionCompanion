using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissions permission;
        private readonly IUserDialogs dialogService;

        public PermissionService(IUserDialogs dialogService)
        {
            permission = CrossPermissions.Current;
            this.dialogService = dialogService;
        }

        public async Task<PermissionStatus> CheckAsync(Permission permissionType, string permissionRequestRationaleMessage = null)
        {
            var status = PermissionStatus.Denied;
            var results = await CheckMultipleAsync(permissionRequestRationaleMessage, permissionType);

            //Best practice to always check that the key exists
            if (results.ContainsKey(permissionType))
            {
                status = results[permissionType];
            }

            return status;
        }

        public Task<Dictionary<Permission, PermissionStatus>> CheckMultipleAsync(params Permission[] permissions)
            => CheckMultipleAsync(null, permissions);

        public async Task<Dictionary<Permission, PermissionStatus>> CheckMultipleAsync(string permissionRequestRationaleMessage, params Permission[] permissions)
        {
            var requestPermissions = false;
            var shouldShowRequestPermission = false;
            var results = new Dictionary<Permission, PermissionStatus>();

            // Checks the permission status for every permission.
            for (var i = 0; i < permissions.Length; i++)
            {
                var permissionType = permissions[i];
                var status = await permission.CheckPermissionStatusAsync(permissionType);
                results[permissionType] = status;

                if (status != PermissionStatus.Granted)
                {
                    requestPermissions = true;
                    shouldShowRequestPermission |= await permission.ShouldShowRequestPermissionRationaleAsync(permissionType);
                }
            }

            if (shouldShowRequestPermission)
            {
                await dialogService.AlertAsync(permissionRequestRationaleMessage ?? "Authorize app permission to continue.");
            }

            if (requestPermissions)
            {
                results = await permission.RequestPermissionsAsync(permissions);
            }

            return results;
        }
    }
}
