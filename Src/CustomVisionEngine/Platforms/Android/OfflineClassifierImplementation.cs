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
        private const string DATA_NORM_LAYER_PREFIX = "data_bn";

        private bool hasNormalizationLayer = false;

        private List<String> labels;
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

            var opIter = inferenceInterface.Graph().Operations();
            while (opIter.HasNext)
            {
                var op = (Org.Tensorflow.Operation)opIter.Next();
                System.Diagnostics.Debug.WriteLine(op.Name());

                if (op.Name().Contains(DATA_NORM_LAYER_PREFIX))
                {
                    hasNormalizationLayer = true;
                    break;
                }
            }

            hasNormalizationLayer = true;
        }

        private IEnumerable<Recognition> Recognize(Bitmap bitmap)
        {
            var outputNames = new string[] { OUTPUT_NAME };
            var intValues = new int[INPUT_WIDTH * INPUT_HEIGHT];
            var floatValues = new float[INPUT_WIDTH * INPUT_HEIGHT * 3];

            bitmap.GetPixels(intValues, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

            var IMAGE_MEAN_R = 0.0f; ;
            var IMAGE_MEAN_G = 0.0f; ;
            var IMAGE_MEAN_B = 0.0f; ;

            if (!hasNormalizationLayer)
            {
                // This is an older model without mean normalization layer and needs to do mean subtraction.
                IMAGE_MEAN_R = 124.0f;
                IMAGE_MEAN_G = 117.0f;
                IMAGE_MEAN_B = 105.0f;
            }

            for (var i = 0; i < intValues.Length; ++i)
            {
                var val = intValues[i];
                floatValues[i * 3 + 0] = (float)(val & 0xFF) - IMAGE_MEAN_B;
                floatValues[i * 3 + 1] = (float)((val >> 8) & 0xFF) - IMAGE_MEAN_G;
                floatValues[i * 3 + 2] = (float)((val >> 16) & 0xFF) - IMAGE_MEAN_R;
            }

            inferenceInterface.Feed(INPUT_NAME, floatValues, 1, INPUT_WIDTH, INPUT_HEIGHT, 3);
            inferenceInterface.Run(outputNames);

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
                using (var croppedBitmap = await NativeImageUtilities.ResizeAndCropAsync(image, bitmap, INPUT_WIDTH, INPUT_HEIGHT))
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
