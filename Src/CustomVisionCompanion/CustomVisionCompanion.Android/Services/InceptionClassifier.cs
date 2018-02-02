using Android.App;
using Android.Content.Res;
using Android.Graphics;
using CustomVisionCompanion.Droid.Services;
using CustomVisionCompanion.Models;
using CustomVisionCompanion.Services;
using Org.Tensorflow.Contrib.Android;
using Plugin.CurrentActivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(InceptionClassifier))]
namespace CustomVisionCompanion.Droid.Services
{
    public class InceptionClassifier : Activity, IClassifier
    {
        private const int INPUT_WIDTH = 227;
        private const int INPUT_HEIGHT = 227;
        private const string INPUT_NAME = "Placeholder";
        private const string OUTPUT_NAME = "loss";

        private const string MODEL_FILE = "file:///android_asset/model.pb";
        private const string LABEL_FILE = "labels.txt";

        private const float IMAGE_MEAN_R = 124f;
        private const float IMAGE_MEAN_G = 117f;
        private const float IMAGE_MEAN_B = 105f;
        private const float IMAGE_STD = 1f;

        private List<String> labels;
        private TensorFlowInferenceInterface inferenceInterface;

        private void Initialize()
        {
            var assetManager = CrossCurrentActivity.Current.Activity.Assets;

            labels = new List<string>();
            using (var sr = new StreamReader(assetManager.Open(LABEL_FILE)))
            {
                var content = sr.ReadToEnd();
                var labelStrings = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.TrimEnd('\r'));
                labels.AddRange(labelStrings);
            }

            inferenceInterface = new TensorFlowInferenceInterface(assetManager, MODEL_FILE);
        }

        private IEnumerable<Recognition> Recognize(Bitmap bitmap)
        {
            var argbPixelArray = new int[INPUT_WIDTH * INPUT_HEIGHT];
            bitmap.GetPixels(argbPixelArray, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

            var normalizedPixelComponents = new float[argbPixelArray.Length * 3];
            for (var i = 0; i < argbPixelArray.Length; ++i)
            {
                var val = argbPixelArray[i];

                normalizedPixelComponents[i * 3 + 0] = ((val & 0xFF) - IMAGE_MEAN_B) / IMAGE_STD;
                normalizedPixelComponents[i * 3 + 1] = (((val >> 8) & 0xFF) - IMAGE_MEAN_G) / IMAGE_STD;
                normalizedPixelComponents[i * 3 + 2] = (((val >> 16) & 0xFF) - IMAGE_MEAN_R) / IMAGE_STD;
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

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image)
        {
            if (inferenceInterface == null)
            {
                await Task.Run(() => Initialize());
            }

            var bitmap = await BitmapFactory.DecodeStreamAsync(image);
            var results = await Task.Run(() => Recognize(bitmap));

            return results;
        }
    }
}
