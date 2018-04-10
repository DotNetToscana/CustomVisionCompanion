using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Cognitive.CustomVision.Prediction;
using Plugin.CustomVisionEngine.Models;

namespace Plugin.CustomVisionEngine
{
    public class OnlineClassifierImplementation : IOnlineClassifier
    {
        private string predictionKey;
        private Guid projectId;

        public Task InitializeAsync(string predictionKey, Guid projectId)
        {
            this.predictionKey = predictionKey;
            this.projectId = projectId;

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image)
        {
            // Use the online model.
            var predictionEndpoint = new PredictionEndpoint { ApiKey = predictionKey };
            var predictions = await predictionEndpoint.PredictImageAsync(projectId, image);

            var results = predictions.Predictions.Select(p => new Recognition
            {
                Tag = p.Tag,
                Probability = p.Probability
            }).ToList();

            return results;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(string predictionKey, Guid projectId, Stream image)
        {
            await InitializeAsync(predictionKey, projectId);
            var results = await RecognizeAsync(image);

            return results;
        }
    }
}
