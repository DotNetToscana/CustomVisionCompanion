using Plugin.CustomVisionEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine
{
    public interface IOfflineClassifier
    {
        Task InitializeAsync(ModelType modelType, params string[] parameters);

        Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, params string[] parameters);
    }
}
