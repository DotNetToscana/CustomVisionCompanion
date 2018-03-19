using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Common
{
    public static class Constants
    {
        public const string PredictionKey = "740625682c404203a490c8e49a69b094";

        // Computer
        //public static Guid ProjectId = new Guid("93deb65e-dfa1-49a3-8a7c-b802a3a909e1");

        // Taggia (compact)
        public static Guid ProjectId = new Guid("fdf677f4-fbb8-4958-860b-273c08aff30b");

        public const int Width = 227;
        public const int Height = 227;

        public const string MainPage = nameof(MainPage);

        public const string AppName = "Custom Vision Companion";
    }
}
