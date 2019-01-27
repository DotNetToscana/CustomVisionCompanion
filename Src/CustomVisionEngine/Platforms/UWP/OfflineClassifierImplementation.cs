using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.CustomVisionEngine.Models;
using Plugin.CustomVisionEngine.Platforms.UWP;
using Windows.AI.MachineLearning;
using Windows.Media;
using Windows.Storage;

namespace Plugin.CustomVisionEngine
{
    public class OfflineClassifierImplementation : IOfflineClassifier
    {
        private LearningModel model;
        private LearningModelSession session;
        private LearningModelBinding binding;

        public async Task InitializeAsync(ModelType modelType, params string[] parameters)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(parameters[0]));

            model = await LearningModel.LoadFromStreamAsync(file);
            session = new LearningModelSession(model);
            binding = new LearningModelBinding(session);
        }

        public async Task<IEnumerable<Recognition>> RecognizeAsync(Stream image, params string[] parameters)
        {
            using (var bitmap = await image.AsSoftwareBitmapAsync())
            {
                using (var frame = VideoFrame.CreateWithSoftwareBitmap(bitmap))
                {
                    var imageFeature = ImageFeatureValue.CreateFromVideoFrame(frame);
                    binding.Bind("data", imageFeature);

                    var evalResult = await session.EvaluateAsync(binding, "0");
                    var output = new ModelOutput()
                    {
                        ClassLabel = (evalResult.Outputs["classLabel"] as TensorString).GetAsVectorView().ToList(),
                        Loss = (evalResult.Outputs["loss"] as IList<IDictionary<string, float>>)[0].ToDictionary(k => k.Key, v => v.Value)
                    };

                    var result = output.Loss.OrderByDescending(l => l.Value).Select(l => new Recognition { Tag = l.Key, Probability = l.Value });
                    return result;
                }
            }
        }
    }
}
