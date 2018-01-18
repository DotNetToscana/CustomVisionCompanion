using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Services
{
    public interface IImageService
    {
        Task<byte[]> ResizeImageAsync(MediaFile file, int width, int height);
    }
}
