using Android.Graphics;
using Org.Tensorflow.Contrib.Android;
using Plugin.CustomVisionEngine.Models;
using Plugin.CustomVisionEngine.Platforms.Android;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine
{
    public class OfflineClassifierImplementation : IOfflineClassifier
    {
        private const string INPUT_NAME = "Placeholder";
        private const string OUTPUT_NAME = "loss";

        private List<string> labels;
        private TensorFlowInferenceInterface inferenceInterface;
        private int inputSize;

        public async Task InitializeAsync(ModelType modelType, params string[] parameters)
        {
            var assets = Android.App.Application.Context.Assets;
            var modelFile = $"file:///android_asset/{parameters[0]}";
            var labelFile = parameters[1];

            labels = new List<string>();
            using var sr = new StreamReader(assets.Open(labelFile));
            var content = await sr.ReadToEndAsync();
            var labelStrings = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.TrimEnd('\r'));
            labels.AddRange(labelStrings);

            inferenceInterface = new TensorFlowInferenceInterface(assets, modelFile);
            inputSize = (int)inferenceInterface.GraphOperation(INPUT_NAME).Output(0).Shape().Size(1);
        }

        private IEnumerable<Recognition> Recognize(Bitmap bitmap)
        {
            var argbPixelArray = new int[inputSize * inputSize];
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
            inferenceInterface.Feed(INPUT_NAME, normalizedPixelComponents, 1, inputSize, inputSize, 3);

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

            if (bitmap.Height != inputSize || bitmap.Width != inputSize)
            {
                using var croppedBitmap = await ImageUtilities.ResizeAndCropAsync(image, bitmap, inputSize, inputSize);
                results = await Task.Run(() => Recognize(croppedBitmap));
                croppedBitmap.Recycle();
            }
            else
            {
                results = await Task.Run(() => Recognize(bitmap));
            }

            return results;
        }
    }
}