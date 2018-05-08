using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using CoreML;
using Foundation;
using Plugin.CustomVisionEngine.Exceptions;
using Plugin.CustomVisionEngine.Models;
using Plugin.CustomVisionEngine.Platforms.iOS;
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
                var modelName = parameters[0];
                var modelPath = NSBundle.MainBundle.GetUrlForResource(modelName, COMPILED_MODEL_EXT) ?? CompileModel(modelName);
                if (modelPath == null)
                {
                    throw new ClassifierException($"Model {modelName} doesn't exist.");
                }

                model = MLModel.Create(modelPath, out var err);

                if (err != null)
                {
                    throw new ClassifierException(err.ToString());
                }
            });
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream source, params string[] parameters)
        {
            var results = new List<Recognition>();
            var image = await UIImage.LoadFromData(NSData.FromStream(source)).ResizeImageAsync(INPUT_WIDTH, INPUT_HEIGHT);

            await Task.Run(() =>
            {
                var pixelBuffer = image.Scale(ImageSize).ToCVPixelBuffer();
                var imageValue = MLFeatureValue.Create(pixelBuffer);

                var inputs = new NSDictionary<NSString, NSObject>(new NSString(INPUT_NAME), imageValue);

                var inputFeatureProvider = new MLDictionaryFeatureProvider(inputs, out var error1);
                if (error1 != null)
                {
                    throw new ClassifierException(error1.ToString());
                }

                var outFeatures = model.GetPrediction(inputFeatureProvider, out var error2);
                if (error2 != null)
                {
                    throw new ClassifierException(error2.ToString());
                }

                var predictionsDictionary = outFeatures.GetFeatureValue(OUTPUT_NAME).DictionaryValue;
                foreach (var key in predictionsDictionary.Keys)
                {
                    var description = (string)(NSString)key;
                    var probability = (double)predictionsDictionary[key];
                    results.Add(new Recognition
                    {
                        Tag = description,
                        Probability = probability
                    });
                }

                // Sort descending.
                results.Sort((t1, t2) => (t1.Probability.CompareTo(t2.Probability)) * -1);
            });

            return results;
        }

        private NSUrl CompileModel(string modelName)
        {
            var uncompiled = NSBundle.MainBundle.GetUrlForResource(modelName, MODEL_EXT);
            var modelPath = MLModel.CompileModel(uncompiled, out var err);

            if (err != null)
            {
                throw new ClassifierException(err.ToString());
            }

            return modelPath;
        }
    }
}
