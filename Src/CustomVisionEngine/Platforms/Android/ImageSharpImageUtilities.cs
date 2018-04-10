using Android.Graphics;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine.Platforms.Android
{
    public static class ImageSharpImageUtilities
    {
        public static async Task<Bitmap> ResizeAndCropAsync(System.IO.Stream imageData, Bitmap bitmap, int width, int height)
        {
            // Read image from stream
            using (var output = new MemoryStream())
            {
                await Task.Run(() =>
                {
                    imageData.Position = 0;
                    var image = Image.Load(imageData)
                                    .Resize(new ResizeOptions
                                    {
                                        Size = new SixLabors.Primitives.Size(width, height),
                                        Mode = ResizeMode.Crop
                                    });

                    image.Save(output, ImageFormats.Jpeg);
                    image.Dispose();
                });

                output.Position = 0;
                return await BitmapFactory.DecodeStreamAsync(output);
            }
        }
    }
}
