using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using CoreML;
using Foundation;
using Plugin.CustomVisionEngine.Models;
using Plugin.CustomVisionEngine.Platforms.iOS;
using Plugin.CustomVisionEngine.Shared.Models;
using UIKit;
using Vision;

namespace Plugin.CustomVisionEngine
{
    public class OfflineClassifierImplementation : IOfflineClassifier
    {
        private const int INPUT_WIDTH = 227;
        private const int INPUT_HEIGHT = 227;

        private const string INPUT_NAME = "data";
        private const string OUTPUT_NAME = "loss";

        private const string COMPILED_MODEL_EXT = "mlmodelc";
        private const string MODEL_EXT = "mlmodel";

        private readonly CGSize ImageSize = new CGSize(INPUT_WIDTH, INPUT_HEIGHT);
        private MLModel model;

        public async Task InitializeAsync(ModelType modelType, params string[] parameters)
        {
            await Task.Run(() =>
            {
                var modelPath = NSBundle.MainBundle.GetUrlForResource(parameters[0], COMPILED_MODEL_EXT) ?? CompileModel(parameters[0]);

                if (modelPath == null)
                    throw new ClassifierException($"Model {parameters[0]} does not exist");

                model = MLModel.Create(modelPath, out NSError err);

                if (err != null)
                    throw new ClassifierException($"Generic error: {err.ToString()}");
            });
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream source, params string[] parameters)
        {
            var byProbability = new List<Recognition>();

            var image = await UIImage.LoadFromData(NSData.FromStream(source)).ResizeImageAsync(INPUT_WIDTH, INPUT_HEIGHT);

            await Task.Run(() =>
            {
                if (model == null)
                {
                    throw new ArgumentNullException();
                }

                var pixelBuffer = image.Scale(ImageSize).ToCVPixelBuffer();
                var imageValue = MLFeatureValue.Create(pixelBuffer);

                var inputs = new NSDictionary<NSString, NSObject>(new NSString(INPUT_NAME), imageValue);

                var inputFeatureProvider = new MLDictionaryFeatureProvider(inputs, out var error1);
                if (error1 != null)
                {
                    throw new ClassifierException($"Recognize Error: {error1}");
                }

                var outFeatures = model.GetPrediction(inputFeatureProvider, out var error2);
                if (error2 != null)
                {
                    throw new ClassifierException($"Recognize Error: {error2}");
                }

                var predictionsDictionary = outFeatures.GetFeatureValue(OUTPUT_NAME).DictionaryValue;
                foreach (var key in predictionsDictionary.Keys)
                {
                    var description = (string)(NSString)key;
                    var prob = (double)predictionsDictionary[key];
                    byProbability.Add(new Recognition
                    {
                        Tag = description,
                        Probability = prob
                    });
                }

                //Sort descending
                byProbability.Sort((t1, t2) => (t1.Probability.CompareTo(t2.Probability)) * -1);
            });

            return byProbability;
        }

        private NSUrl CompileModel(string modelName)
        {
            var uncompiled = NSBundle.MainBundle.GetUrlForResource(modelName, MODEL_EXT);
            var modelPath = MLModel.CompileModel(uncompiled, out NSError err);

            if (err != null)
            {
                throw new ClassifierException($"Generic error: {err.ToString()}");
            }

            return modelPath;
        }
    }
}
