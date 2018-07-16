using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.CustomVisionEngine.Exceptions;
using Plugin.CustomVisionEngine.Models;

namespace Plugin.CustomVisionEngine
{
    public class OnlineClassifierImplementation : IOnlineClassifier
    {
        private string predictionKey;
        private Guid projectId;

        private HttpClient client;

        public Task InitializeAsync(string predictionKey, Guid projectId, string customVisionEndpoint)
        {
            this.predictionKey = predictionKey;
            this.projectId = projectId;

            client = new HttpClient
            {
                BaseAddress = new Uri(customVisionEndpoint.EndsWith("/") ? customVisionEndpoint : customVisionEndpoint += "/")
            };

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, Guid? iterationId = null)
        {
            var request = CreatePredictRequest(projectId, iterationId);

            request.Content = new StreamContent(image);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var predictions = await SendRequestAsync<ImagePredictionResult>(request);
            var results = predictions.Predictions.Select(p => new Recognition
            {
                Tag = p.Tag,
                Probability = p.Probability
            }).ToList();

            return results;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(string predictionKey, Guid projectId, Stream image, Guid? iterationId, string customVisionEndpoint)
        {
            await InitializeAsync(predictionKey, projectId, customVisionEndpoint);
            var results = await RecognizeAsync(image, iterationId);

            return results;
        }

        private HttpRequestMessage CreatePredictRequest(Guid projectId, Guid? iterationId)
        {
            var endpoint = $"Prediction/{projectId}/image?iterationId={iterationId}";
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Prediction-Key", predictionKey);

            return request;
        }

        private async Task<T> SendRequestAsync<T>(HttpRequestMessage request)
        {
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContentString = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<T>(responseContentString);

                return content;
            }
            else
            {
                var exception = new ClassifierException(response.ReasonPhrase);
                throw exception;
            }
        }
    }
}
