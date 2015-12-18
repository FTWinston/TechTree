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

        public static void Randomize<T>(this List<T> list, Random r)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
