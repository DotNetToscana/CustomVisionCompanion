﻿using Newtonsoft.Json;
using System;

namespace Plugin.CustomVisionEngine.Models
{
    internal class ImageTagPrediction
    {
        public Guid TagId { get; set; }

        [JsonProperty("TagName")]
        public string Tag { get; set; }

        public double Probability { get; set; }
    }
}
