using Plugin.CustomVisionEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine.Platforms.Android
{
    public static class ImageExtensions
    {
        public static float ImageMeanR(this ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.Retail:
                    return 0.0f;

                case ModelType.General:
                case ModelType.Landmarks:
                default:
                    return 123.0f;
            }
        }

        public static float ImageMeanG(this ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.Retail:
                    return 0.0f;

                case ModelType.General:
                case ModelType.Landmarks:
                default:
                    return 117.0f;
            }
        }

        public static float ImageMeanB(this ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.Retail:
                    return 0.0f;

                case ModelType.General:
                case ModelType.Landmarks:
                default:
                    return 104.0f;
            }
        }
    }
}
