using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomVisionCompanion.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Cognitive.CustomVision.Prediction;

namespace CustomVisionCompanion.Services
{
    public class CustomVisionClassifier : IClassifier
    {
        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image)
        {
            var settingsService = SimpleIoc.Default.GetInstance<ISettingsService>();

            // Use the online model.
            var predictionEndpoint = new PredictionEndpoint { ApiKey = settingsService.PredictionKey };
            var predictions = await predictionEndpoint.PredictImageAsync(settingsService.ProjectId, image);

            var results = predictions.Predictions.Select(p => new Recognition
            {
                Tag = p.Tag,
                Probability = p.Probability
            }).ToList();

            return results;
        }
    }
}
