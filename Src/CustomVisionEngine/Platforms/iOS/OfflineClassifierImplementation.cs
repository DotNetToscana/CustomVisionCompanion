using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Plugin.CustomVisionEngine.Models;

namespace Plugin.CustomVisionEngine
{
    public class OfflineClassifierImplementation : IOfflineClassifier
    {
        public Task InitializeAsync(ModelType modelType, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
