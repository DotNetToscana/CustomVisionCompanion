using CustomVisionCompanion.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Services
{
    public interface IClassifier
    {
        Task<IEnumerable<Recognition>> RecognizeAsync(Stream image);
    }
}
