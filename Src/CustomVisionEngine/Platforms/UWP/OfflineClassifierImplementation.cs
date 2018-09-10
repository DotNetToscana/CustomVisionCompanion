using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.CustomVisionEngine.Models;
using Plugin.CustomVisionEngine.Platforms.UWP;
using Windows.AI.MachineLearning.Preview;
using Windows.Media;
using Windows.Storage;

namespace Plugin.CustomVisionEngine
{
    public class OfflineClassifierImplementation : IOfflineClassifier
    {
        private LearningModelPreview learningModel;
        private string[] tags;

        public async Task InitializeAsync(ModelType modelType, params string[] parameters)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(parameters[0]));
            var learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);

            tags = parameters.Skip(1).ToArray();
            this.learningModel = learningModel;
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, params string[] parameters)
        {
            using (var bitmap = await image.AsSoftwareBitmapAsync())
            {
                using (var frame = VideoFrame.CreateWithSoftwareBitmap(bitmap))
                {
                    var output = new ModelOutput(tags);
                    var binding = new LearningModelBindingPreview(learningModel);

                    binding.Bind("data", frame);
                    binding.Bind("classLabel", output.ClassLabel);
                    binding.Bind("loss", output.Loss);
                    var evalResult = await learningModel.EvaluateAsync(binding, string.Empty);

                    var result = output.Loss.OrderByDescending(l => l.Value).Select(l => new Recognition { Tag = l.Key, Probability = l.Value });
                    return result;
                }
            }
        }
    }
}
