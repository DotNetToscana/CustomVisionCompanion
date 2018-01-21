using Android.Graphics;
using Android.Media;
using Android.Runtime;
using CustomVisionCompanion.Droid.Services;
using CustomVisionCompanion.Services;
using ImageSharp;
using ImageSharp.Processing;
using Java.Nio;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageService))]
namespace CustomVisionCompanion.Droid.Services
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
                    using (var image = ImageSharp.Image.Load(file.GetStream()))
                    {
                        using (var resizedImage = image.Resize(new ResizeOptions
                        {
                            Size = new SixLabors.Primitives.Size(width, height),
                            Mode = ResizeMode.Crop
                        }))
                        {
                            resizedImage.Save(output, ImageFormats.Jpeg);
                        }
                    }
                });

                return output.ToArray();
            }
        }
    }
}
