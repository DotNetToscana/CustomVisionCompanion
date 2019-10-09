using Plugin.CustomVisionEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine
{
    public static class CustomVisionConstants
    {
        public const string DefaultEndpoint = "https://{0}.api.cognitive.microsoft.com/customvision/v3.0/";
    }

    public interface IOnlineClassifier
    {
        Task InitializeAsync(string region, string predictionKey, string customVisionEndpoint = CustomVisionConstants.DefaultEndpoint);

        Task<IEnumerable<Recognition>> RecognizeAsync(string projectName, Guid iterationId, Stream image);

        Task<IEnumerable<Recognition>> RecognizeAsync(string region, string predictionKey, string projectName, Guid iterationId, Stream image, string customVisionEndpoint = CustomVisionConstants.DefaultEndpoint);
    }
}
