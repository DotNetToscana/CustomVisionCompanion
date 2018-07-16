using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine.Platforms.UWP
{
    internal class ModelOutput
    {
        public IList<string> ClassLabel { get; set; }

        public IDictionary<string, float> Loss { get; set; }

        public ModelOutput(params string[] tags)
        {
            ClassLabel = new List<string>();
            Loss = tags.ToDictionary(k => k, v => float.NaN);
        }
    }
}
