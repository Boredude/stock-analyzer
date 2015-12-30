using System;

namespace BigDataClient.BL.Infrastructure
{
    public static class StringExtensions
    {
        public static string ReplaceFirstOccurrence(this string source, string find, string replace)
        {
            int place = source.IndexOf(find, StringComparison.Ordinal);
            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);
            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
    }
}
