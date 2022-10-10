using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalizer
{
    internal class DeltaTime
    {
        DateTime FirstTime;
        public static DeltaTime CreatePoint()
        {
            return new DeltaTime() { FirstTime = DateTime.Now };
        }
        public TimeSpan GetDeltaTime()
        {
            return DateTime.Now - FirstTime;
        }
    }
}
