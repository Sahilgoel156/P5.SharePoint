using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Data.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Split<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (chunkSize <= 0)
            {
                throw new ArgumentException("Invalid chunk size.", nameof(chunkSize));
            }

            return SplitIterator(source, chunkSize);
        }

        private static IEnumerable<T[]> SplitIterator<T>(IEnumerable<T> source, int chunkSize)
        {
            var buffer = new List<T>(chunkSize);
            foreach (var item in source)
            {
                if (buffer.Count == chunkSize)
                {
                    var chunk = buffer.ToArray();
                    buffer.Clear();
                    yield return chunk;
                }
                buffer.Add(item);
            }

            if (buffer.Count > 0)
            {
                var chunk = buffer.ToArray();
                buffer.Clear();
                yield return chunk;
            }
        }
    }
}
