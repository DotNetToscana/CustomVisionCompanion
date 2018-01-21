using CustomVisionCompanion.iOS.Services;
using CustomVisionCompanion.Services;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using UIKit;
using System.Drawing;
using Foundation;
using System.Runtime.InteropServices;

[assembly: Dependency(typeof(ImageService))]
namespace CustomVisionCompanion.iOS.Services
{
    public class ImageService : IImageService
    {
        public async Task<byte[]> ResizeImageAsync(MediaFile file, int width, int height)
        {
            byte[] imageByteArray = null;

            await Task.Run(() =>
            {
                using (var image = UIImage.LoadFromData(NSData.FromStream(file.GetStream())))
                {
                    using (var resizedImage = MaxResizeImage(image, width, height))
                    {
                        using (var croppedImage = CropImage(resizedImage, 0, 0, width, height))
                        {
                            using (var imageData = croppedImage.AsJPEG())
                            {
                                imageByteArray = new Byte[imageData.Length];
                                Marshal.Copy(imageData.Bytes, imageByteArray, 0, Convert.ToInt32(imageData.Length));
                            }
                        }
                    }
                }
            });

            return imageByteArray;
        }

        // resize the image to be contained within a maximum width and height, keeping aspect ratio
        public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1)
            {
                return sourceImage;
            }

            var width = (float)(maxResizeFactor * sourceSize.Width);
            var height = (float)(maxResizeFactor * sourceSize.Height);

            UIGraphics.BeginImageContextWithOptions(new SizeF(width, height), false, 2.0f);
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return resultImage;
        }

        // crop the image, without resizing
        private UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
        {
            var imgSize = sourceImage.Size;
            UIGraphics.BeginImageContextWithOptions(new SizeF(width, height), false, 2.0f);
            var context = UIGraphics.GetCurrentContext();

            var clippedRect = new RectangleF(0, 0, width, height);
            context.ClipToRect(clippedRect);

            var drawRect = new RectangleF(-crop_x, -crop_y, (float)imgSize.Width, (float)imgSize.Height);
            sourceImage.Draw(drawRect);
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return modifiedImage;
        }
    }
}
