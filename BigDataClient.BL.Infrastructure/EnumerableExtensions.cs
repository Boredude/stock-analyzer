using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigData.UI.Client.Infrastructure
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

        public static IEnumerable<double> Normalize(this IEnumerable<double> enumerable)
        {
            if (enumerable == null || !enumerable.Any()) return enumerable;
            var doubles = enumerable as IList<double> ?? enumerable.ToList();
            var max = doubles.Max();
            return doubles.Select(i => i/ max);
        } 
    }
}
