using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine.Models
{
    public class Recognition
    {
        public string Tag { get; set; }

        public double Probability { get; set; }

        public override string ToString()
        {
            return string.Format($"[Tag: {Tag}, Probability: {Probability}]");
        }
    }
}
