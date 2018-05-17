using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Services
{
    public interface IPermissionService
    {
        Task<PermissionStatus> CheckAsync(Permission permissions, string permissionRequestRationaleMessage = null);

        Task<Dictionary<Permission, PermissionStatus>> CheckMultipleAsync(string permissionRequestRationaleMessage, params Permission[] permissions);

        Task<Dictionary<Permission, PermissionStatus>> CheckMultipleAsync(params Permission[] permissions);
    }
}
