using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSharing.Benchmark
{
    public static class EnumerableExtension
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            Random rnd = new Random();
            var a = new List<T>();
            for (int i = 0; i < count; i++)
            { 
                int r = rnd.Next(source.Count());
                a.Add(source.ElementAt(r));
            }
            return a;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}
