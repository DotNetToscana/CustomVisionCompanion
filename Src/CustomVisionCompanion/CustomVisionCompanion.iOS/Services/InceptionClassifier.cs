using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CustomVisionCompanion.Models;
using CustomVisionCompanion.Services;
using CustomVisionCompanion.iOS.Services;
using CoreGraphics;
using CoreML;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using CustomVisionCompanion.iOS.Extensions;

[assembly: Dependency(typeof(InceptionClassifier))]
namespace CustomVisionCompanion.iOS.Services
{
    public class InceptionClassifier : IClassifier
    {
        private const int INPUT_WIDTH = 227;
        private const int INPUT_HEIGHT = 227;

        private const string INPUT_NAME = "data";
        private const string OUTPUT_NAME = "loss";

        private readonly CGSize ImageSize = new CGSize(INPUT_WIDTH, INPUT_HEIGHT);
        private MLModel model;

        public InceptionClassifier()
        {
            model = LoadModel("Computer");
        }

        private IEnumerable<Recognition> Recognize(Stream source)
        {
            var byProbability = new List<Recognition>();
            var image = UIImage.LoadFromData(NSData.FromStream(source));

            if (model == null)
            {
                //ErrorOccurred(this, new EventArgsT<string>(error.ToString()));
                return byProbability;
            }

            var pixelBuffer = image.Scale(ImageSize).ToCVPixelBuffer();
            var imageValue = MLFeatureValue.Create(pixelBuffer);

            var inputs = new NSDictionary<NSString, NSObject>(new NSString(INPUT_NAME), imageValue);

            var inputFeatureProvider = new MLDictionaryFeatureProvider(inputs, out var error1);
            if (error1 != null)
            {
                //ErrorOccurred(this, new EventArgs<string>(error1.ToString()));
                return byProbability;
            }

            var outFeatures = model.GetPrediction(inputFeatureProvider, out var error2);
            if (error2 != null)
            {
                //ErrorOccurred(this, new EventArgs<string>(error2.ToString()));
                return byProbability;
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

            return byProbability;
        }

        private MLModel LoadModel(string modelName)
        {
            var bundle = NSBundle.MainBundle;
            var assetPath = bundle.GetUrlForResource(modelName, "mlmodelc");
            var mdl = MLModel.Create(assetPath, out var error);

            if (error != null)
            {
                //ErrorOccurred(this, new EventArgs<string>(err.ToString()));
            }

            return mdl;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image)
        {
            var results = await Task.Run(() => Recognize(image));
            return results;
        }
    }
}
