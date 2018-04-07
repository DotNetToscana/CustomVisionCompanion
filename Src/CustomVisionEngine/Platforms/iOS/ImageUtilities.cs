using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Plugin.CustomVisionEngine.Platforms.iOS
{
    public static class ImageUtilities
    {
        public static async Task<UIImage> ResizeImageAsync(this UIImage image, int width, int height)
        {
            UIImage croppedImage = null;

            await Task.Run(() =>
            {
                using (var resizedImage = MaxResizeImage(image, width, height))
                {
                    croppedImage = CropImage(resizedImage, 0, 0, width, height);
                }
            });

            return croppedImage;
        }

        // resize the image to be contained within a maximum width and height, keeping aspect ratio
        private static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
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
        private static UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
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
