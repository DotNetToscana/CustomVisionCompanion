using Newtonsoft.Json;
using Plugin.CustomVisionEngine.Exceptions;
using Plugin.CustomVisionEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine
{
    public class OnlineClassifierImplementation : IOnlineClassifier
    {
        private string predictionKey;
        private HttpClient client;

        public Task InitializeAsync(string region, string predictionKey, string customVisionEndpoint)
        {
            this.predictionKey = predictionKey;
            var uri = string.Format(customVisionEndpoint, region);

            client = new HttpClient
            {
                BaseAddress = new Uri(uri.EndsWith("/") ? uri : uri += "/")
            };

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(string projectName, Guid iterationId, Stream image)
        {
            var request = CreatePredictRequest(projectName, iterationId);

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

        public async Task<IEnumerable<Recognition>> RecognizeAsync(string region, string predictionKey, string projectName, Guid iterationId, Stream image, string customVisionEndpoint)
        {
            await InitializeAsync(region, predictionKey, customVisionEndpoint);
            var results = await RecognizeAsync(projectName, iterationId, image);

            return results;
        }

        private HttpRequestMessage CreatePredictRequest(string projectName, Guid iterationId)
        {
            var endpoint = $"Prediction/{iterationId}/classify/iterations/{projectName}/image";
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
