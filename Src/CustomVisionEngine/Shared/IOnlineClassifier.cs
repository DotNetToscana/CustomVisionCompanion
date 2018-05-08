using Plugin.CustomVisionEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine
{
    public interface IOnlineClassifier
    {
        Task InitializeAsync(string predictionKey, Guid projectId, string customVisionEndpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.1/");

        Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, Guid? iterationId = null);

        Task<IEnumerable<Recognition>> RecognizeAsync(string predictionKey, Guid projectId, Stream image, Guid? iterationId = null, string customVisionEndpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.1/");
    }
}
