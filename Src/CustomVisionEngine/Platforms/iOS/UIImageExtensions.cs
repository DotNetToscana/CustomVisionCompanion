using System;
using CoreGraphics;
using CoreImage;
using CoreVideo;
using UIKit;

namespace Plugin.CustomVisionEngine.Platforms.iOS
{
    public static class UIImageExtensions
    {
        public static CVPixelBuffer ToCVPixelBuffer(this UIImage self)
        {
            var attrs = new CVPixelBufferAttributes
            {
                CGImageCompatibility = true,
                CGBitmapContextCompatibility = true
            };

            var cgImg = self.CGImage;

            var pb = new CVPixelBuffer(cgImg.Width, cgImg.Height, CVPixelFormatType.CV32ARGB, attrs);
            pb.Lock(CVPixelBufferLock.None);

            var pData = pb.BaseAddress;
            var colorSpace = CGColorSpace.CreateDeviceRGB();

            var ctxt = new CGBitmapContext(pData, cgImg.Width, cgImg.Height, 8, pb.BytesPerRow, colorSpace, CGImageAlphaInfo.NoneSkipFirst);
            ctxt.TranslateCTM(0, cgImg.Height);
            ctxt.ScaleCTM(1.0f, -1.0f);

            UIGraphics.PushContext(ctxt);
            self.Draw(new CGRect(0, 0, cgImg.Width, cgImg.Height));
            UIGraphics.PopContext();

            pb.Unlock(CVPixelBufferLock.None);

            return pb;
        }

        public static CIImageOrientation ToCIImageOrientation(this UIImageOrientation self)
        {
            // Take action based on value
            switch (self)
            {
                case UIImageOrientation.Up:
                    return CIImageOrientation.TopLeft;

                case UIImageOrientation.UpMirrored:
                    return CIImageOrientation.TopRight;

                case UIImageOrientation.Down:
                    return CIImageOrientation.BottomLeft;

                case UIImageOrientation.DownMirrored:
                    return CIImageOrientation.BottomRight;

                case UIImageOrientation.Left:
                    return CIImageOrientation.LeftTop;

                case UIImageOrientation.LeftMirrored:
                    return CIImageOrientation.LeftBottom;

                case UIImageOrientation.Right:
                    return CIImageOrientation.RightTop;

                case UIImageOrientation.RightMirrored:
                    return CIImageOrientation.RightBottom;
            }

            // Default to up
            return CIImageOrientation.TopLeft;
        }
    }
}