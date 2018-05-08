using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CustomVisionEngine.Exceptions
{
    public class ClassifierException : Exception
    {
        public ClassifierException(string message) : base(message)
        {
        }

        public ClassifierException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
