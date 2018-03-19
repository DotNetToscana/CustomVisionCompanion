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
        Task InitializeAsync(string predictionKey, Guid projectId);

        Task<IEnumerable<Recognition>> RecognizeAsync(Stream image);

        Task<IEnumerable<Recognition>> RecognizeAsync(string predictionKey, Guid projectId, Stream image);
    }
}
