using CustomVisionCompanion.iOS.Services;
using CustomVisionCompanion.Services;
using ImageSharp;
using ImageSharp.Processing;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageService))]
namespace CustomVisionCompanion.iOS.Services
{
    public class ImageService : IImageService
    {
        public async Task<byte[]> ResizeImageAsync(MediaFile file, int width, int height)
        {
            // Read image from stream
            using (var output = new MemoryStream())
            {
                await Task.Run(() =>
                {
                    var image = ImageSharp.Image.Load(file.GetStream())
                                    .Resize(new ResizeOptions
                                    {
                                        Size = new SixLabors.Primitives.Size(width, height),
                                        Mode = ResizeMode.Crop
                                    });

                    image.Save(output, ImageFormats.Jpeg);
                });

                return output.ToArray();
            }
        }
    }
}
