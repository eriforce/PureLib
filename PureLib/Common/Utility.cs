using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public static class Utility {
        // Use constant size for stackalloc to get better performance. https://github.com/dotnet/docs/issues/28823
        [SkipLocalsInit]
        public static void RentByteSpace(int size, ActionForSpan<byte> action) {
            byte[] bufferToReturn = null;
            Span<byte> buffer = size <= Constants.StackAllocThresholdOfBytes
                ? stackalloc byte[Constants.StackAllocThresholdOfBytes]
                : bufferToReturn = ArrayPool<byte>.Shared.Rent(size);

            action(buffer);

            if (bufferToReturn != null)
                ArrayPool<byte>.Shared.Return(bufferToReturn);
        }

        [SkipLocalsInit]
        public static void RentCharSpace(int size, ActionForSpan<char> action) {
            char[] bufferToReturn = null;
            Span<char> buffer = size <= Constants.StackAllocThresholdOfChars
                ? stackalloc char[Constants.StackAllocThresholdOfChars]
                : bufferToReturn = ArrayPool<char>.Shared.Rent(size);

            action(buffer);

            if (bufferToReturn != null)
                ArrayPool<char>.Shared.Return(bufferToReturn);
        }

        public static int GetWeekdayCount(DateOnly start, DateOnly end) {
            int dayCount = 0;
            int daysRemain = 0;
            for (DateOnly i = start; i <= end; i = i.AddDays(1)) {
                if (i.DayOfWeek != DayOfWeek.Sunday && i.DayOfWeek != DayOfWeek.Saturday)
                    dayCount++;
                if (i.DayOfWeek == end.DayOfWeek) {
                    daysRemain = (end.DayNumber - i.DayNumber) / 7 * 5;
                    break;
                }
            }
            return dayCount + daysRemain;
        }

        public static string WildcardToRegex(this string pattern) {
            return Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".");
        }

        public static bool IsNullOrEmpty(this string str) {
            return string.IsNullOrEmpty(str);
        }

        public static string FormatWith(this string format, params object[] args) {
            return string.Format(format, args);
        }

        public static string ToIso8601(this DateTime time) {
            return time.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
        }

        public static T CreateInstance<T>(params object[] args) {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        public static bool IsDarkColor(byte[] rgb) {
            if (rgb.Length != 3)
                throw new ArgumentException("RGB array is invalid.", nameof(rgb));
            return rgb.Max() + rgb.Min() < 255; // L < 0.5 
        }

        public static T[] ToEnum<T>(this string str, string separator = ",") where T : struct, IConvertible {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type.");

            return str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => (T)Enum.Parse(typeof(T), s)).ToArray();
        }

        public static List<T[]> ChunkBy<T>(this IList<T> items, int chunkSize) {
            Span<T> span = default;
            if (items is T[] array)
                span = array.AsSpan();
            else if (items is List<T> list)
                span = CollectionsMarshal.AsSpan(list);

            if (span != default) {
                List<T[]> list = new();
                for (int i = 0; i < span.Length; i += chunkSize) {
                    list.Add(span.Slice(i, Math.Min(chunkSize, span.Length - i)).ToArray());
                }
                return list;
            }
            else {
                return Enumerable.Range(0, (items.Count - 1) / chunkSize + 1)
                    .Select(i => items.Skip(i * chunkSize).Take(Math.Min(chunkSize, items.Count - i * chunkSize)).ToArray())
                    .ToList();
            }
        }

        public static void Shuffle<T>(this IList<T> list, Random rand) {
            for (int i = 1; i < list.Count; i++) {
                int pos = rand.Next(i + 1);
                (list[pos], list[i]) = (list[i], list[pos]);
            }
        }
    }
}
