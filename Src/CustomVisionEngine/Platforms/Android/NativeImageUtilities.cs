using Android.Graphics;
using Android.Media;
using Plugin.CustomVisionEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine.Platforms.Android
{
    public static class ImageUtilities
    {
        public static async Task<Bitmap> ResizeAndCropAsync(System.IO.Stream image, Bitmap bitmap, int width, int height)
        {
            Bitmap croppedBitmap = null;

            await Task.Run(() =>
            {
                using (var resizedBitmap = ResizeImage(image, bitmap, width, height))
                {
                    croppedBitmap = Crop(resizedBitmap, width, height);
                    resizedBitmap.Recycle();
                }
            });

            return croppedBitmap;
        }

        private static Bitmap ResizeImage(System.IO.Stream image, Bitmap bitmap, int width, int height)
        {
            Bitmap scaledBitmap = null;

            var originalPixelWidth = bitmap.Width;
            var originalPixelHeight = bitmap.Height;

            var widthRatio = (double)height / originalPixelWidth;
            var heightRatio = (double)width / originalPixelHeight;
            var aspectWidth = width;
            var aspectHeight = height;

            if (originalPixelWidth > originalPixelHeight)
            {
                aspectWidth = (int)(heightRatio * originalPixelWidth);
            }
            else
            {
                aspectHeight = (int)(widthRatio * originalPixelHeight);
            }

            scaledBitmap = Bitmap.CreateBitmap(aspectWidth, aspectHeight, Bitmap.Config.Argb8888);

            var ratioX = aspectWidth / (float)bitmap.Width;
            var ratioY = aspectHeight / (float)bitmap.Height;
            var middleX = aspectWidth / 2.0f;
            var middleY = aspectHeight / 2.0f;

            var scaleMatrix = new Matrix();
            scaleMatrix.SetScale(ratioX, ratioY, middleX, middleY);

            var canvas = new Canvas(scaledBitmap)
            {
                Matrix = scaleMatrix
            };
            canvas.DrawBitmap(bitmap, middleX - bitmap.Width / 2, middleY - bitmap.Height / 2, new Paint(PaintFlags.FilterBitmap));

            // check the rotation of the image and display it properly
            var exif = new ExifInterface(image);
            var orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, 0);
            var matrix = new Matrix();

            if (orientation == 6)
            {
                matrix.PostRotate(90);
            }
            else if (orientation == 3)
            {
                matrix.PostRotate(180);
            }
            else if (orientation == 8)
            {
                matrix.PostRotate(270);
            }

            scaledBitmap = Bitmap.CreateBitmap(scaledBitmap, 0, 0,
                    scaledBitmap.Width, scaledBitmap.Height, matrix,
                    true);

            return scaledBitmap;
        }

        private static Bitmap Crop(Bitmap srcBmp, int width, int height)
        {
            Bitmap dstBmp = null;

            if (srcBmp.Width >= srcBmp.Height)
            {
                dstBmp = Bitmap.CreateBitmap(
                   srcBmp,
                   srcBmp.Width / 2 - srcBmp.Height / 2,
                   0,
                   srcBmp.Height,
                   srcBmp.Height
                   );
            }
            else
            {
                dstBmp = Bitmap.CreateBitmap(
                   srcBmp,
                   0,
                   srcBmp.Height / 2 - srcBmp.Width / 2,
                   srcBmp.Width,
                   srcBmp.Width
                   );
            }

            return dstBmp;
        }
    }
}
