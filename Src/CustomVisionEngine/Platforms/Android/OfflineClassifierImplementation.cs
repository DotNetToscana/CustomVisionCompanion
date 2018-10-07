using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Graphics;
using Org.Tensorflow.Contrib.Android;
using Plugin.CustomVisionEngine.Models;
using Plugin.CustomVisionEngine.Platforms.Android;

namespace Plugin.CustomVisionEngine
{
    public class OfflineClassifierImplementation : IOfflineClassifier
    {
        private const int INPUT_WIDTH = 227;
        private const int INPUT_HEIGHT = 227;
        private const string INPUT_NAME = "Placeholder";
        private const string OUTPUT_NAME = "loss";

        private List<string> labels;
        private TensorFlowInferenceInterface inferenceInterface;

        public async Task InitializeAsync(ModelType modelType, params string[] parameters)
        {
            var assets = Android.App.Application.Context.Assets;
            var modelFile = $"file:///android_asset/{parameters[0]}";
            var labelFile = parameters[1];

            labels = new List<string>();
            using (var sr = new StreamReader(assets.Open(labelFile)))
            {
                var content = await sr.ReadToEndAsync();
                var labelStrings = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.TrimEnd('\r'));
                labels.AddRange(labelStrings);
            }

            inferenceInterface = new TensorFlowInferenceInterface(assets, modelFile);
        }

        private IEnumerable<Recognition> Recognize(Bitmap bitmap)
        {
            var argbPixelArray = new int[INPUT_WIDTH * INPUT_HEIGHT];
            var normalizedPixelComponents = new float[argbPixelArray.Length * 3];

            bitmap.GetPixels(argbPixelArray, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

            for (var i = 0; i < argbPixelArray.Length; ++i)
            {
                var val = argbPixelArray[i];
                normalizedPixelComponents[i * 3 + 0] = val & 0xFF;
                normalizedPixelComponents[i * 3 + 1] = (val >> 8) & 0xFF;
                normalizedPixelComponents[i * 3 + 2] = (val >> 16) & 0xFF;
            }

            // Copy the input data into TF
            inferenceInterface.Feed(INPUT_NAME, normalizedPixelComponents, 1, INPUT_WIDTH, INPUT_HEIGHT, 3);

            // Run the inference
            inferenceInterface.Run(new[] { OUTPUT_NAME });

            // Grab the output data
            var outputs = new float[labels.Count];
            inferenceInterface.Fetch(OUTPUT_NAME, outputs);

            var results = new Recognition[labels.Count];
            for (var i = 0; i < labels.Count; i++)
            {
                results[i] = new Recognition
                {
                    Probability = outputs[i],
                    Tag = labels[i]
                };
            }

            // Sort high-to-low via confidence
            Array.Sort(results, (x, y) => y.Probability.CompareTo(x.Probability));
            return results;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, params string[] parameters)
        {
            IEnumerable<Recognition> results = null;
            var bitmap = await BitmapFactory.DecodeStreamAsync(image);

            if (bitmap.Height != INPUT_HEIGHT || bitmap.Width != INPUT_WIDTH)
            {
                using (var croppedBitmap = await ImageUtilities.ResizeAndCropAsync(image, bitmap, INPUT_WIDTH, INPUT_HEIGHT))
                {
                    results = await Task.Run(() => Recognize(croppedBitmap));
                    croppedBitmap.Recycle();
                }
            }
            else
            {
                results = await Task.Run(() => Recognize(bitmap));
            }

            return results;
        }
    }
}
