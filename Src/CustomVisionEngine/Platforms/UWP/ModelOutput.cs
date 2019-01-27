using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;

namespace Plugin.CustomVisionEngine.Platforms.UWP
{
    internal class ModelOutput
    {
        public IList<string> ClassLabel { get; set; }

        public IDictionary<string, float> Loss { get; set; }
    }
}
