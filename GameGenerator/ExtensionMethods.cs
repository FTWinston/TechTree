using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameGenerator
{
    internal static class ExtensionMethods
    {
        public static double NextDouble(this Random random, double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

        public static int Next(this Random random, double min, double max)
        {
            return (random.NextDouble() * (max - min) + min).Round();
        }

        public static int Round(this double value)
        {
            return (int)Math.Round(value);
        }

        public static int RoundNearest(this int value, int multiple)
        {
            return value / multiple * multiple;
        }

        public static int RoundNearest(this double value, int multiple)
        {
            return (int)Math.Round(value / multiple) * multiple;
        }

        public static int RoundNearestDown(this double value, int multiple)
        {
            return (int)(value / multiple) * multiple;
        }

        public static int Scale(this int value, double factor)
        {
            return (int)Math.Round(value * factor);
        }

        public static void Randomize<T>(this IList<T> list, Random r)
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

        public static Queue<T> ToRandomQueue<T>(this IList<T> list, Random r)
        {
            var queue = new Queue<T>();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                queue.Enqueue(value);
            }

            queue.Enqueue(list[0]);
            return queue;
        }

        private static int RandomIndex<T>(this IList<T> list, Random r, int excludeIndex = -1)
        {
            if (excludeIndex < 0)
            {
                return r.Next(list.Count);
            }

            int index = r.Next(list.Count - 1);
            if (index >= excludeIndex)
                index++;

            return index;
        }


        public static T PickRandom<T>(this IList<T> list, Random r)
        {
            return list[RandomIndex(list, r)];
        }

        public static void PickRandom2<T>(this IList<T> list, Random r, out T val1, out T val2)
        {
            var index1 = RandomIndex(list, r);
            val1 = list[index1];

            var index2 = RandomIndex(list, r, index1);
            val2 = list[index2];
        }
    }
}
