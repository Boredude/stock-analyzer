using System.Collections.Generic;
using System.Linq;

namespace BigDataClient.BL.Infrastructure
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static IEnumerable<double> Normalize(this IEnumerable<double> enumerable, int newMin = 0, int newMax = 100)
        {
            if (enumerable == null || !enumerable.Any()) return enumerable;
            var doubles = enumerable as IList<double> ?? enumerable.ToList();

            // get max and min for given range
            var max = doubles.Max();
            var min = doubles.Min();

            return doubles.Select(i => (i - min) / (max - min) * (newMax - newMin) + newMin);
        } 
    }
}
