using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionCompanion.Common
{
    public class TaskEx
    {
        public static Task CompletedTask => Task.FromResult<object>(null);
    }
}
