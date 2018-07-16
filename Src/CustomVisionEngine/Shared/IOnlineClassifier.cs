using Plugin.CustomVisionEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine
{
    public static class CustomVisionConstants
    {
        public const string DefaultEndpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/";
    }

    public interface IOnlineClassifier
    {
        Task InitializeAsync(string predictionKey, Guid projectId, string customVisionEndpoint = CustomVisionConstants.DefaultEndpoint);

        Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, Guid? iterationId = null);

        Task<IEnumerable<Recognition>> RecognizeAsync(string predictionKey, Guid projectId, Stream image, Guid? iterationId, string customVisionEndpoint = CustomVisionConstants.DefaultEndpoint);
    }
}
