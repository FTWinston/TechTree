using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels
{
    internal static class ExtensionMethods
    {
        public static int RoundNearest(this int value, int multiple)
        {
            return value / multiple * multiple;
        }
    }
}
