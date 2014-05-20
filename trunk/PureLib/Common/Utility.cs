using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public static class Utility {
        public static TimeSpan GetExecutingDuration(Action action) {
            DateTime start = DateTime.UtcNow;
            action();
            return DateTime.UtcNow - start;
        }

        public static int GetWeekdayCount(DateTime start, DateTime end) {
            int dayCount = 0;
            int daysRemain = 0;
            for (DateTime i = start; i <= end; i = i.AddDays(1)) {
                if (i.DayOfWeek != DayOfWeek.Sunday && i.DayOfWeek != DayOfWeek.Saturday)
                    dayCount++;
                if (i.DayOfWeek == end.DayOfWeek) {
                    daysRemain = (end - i).Days / 7 * 5;
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

        public static string ToBase64String(this byte[] bytes) {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] FromBase64String(this string str) {
            return Convert.FromBase64String(str);
        }

        public static string ToHexString(this byte[] bytes) {
            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes) {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static byte[] FromHexString(this string hex) {
            if (hex.IsNullOrEmpty() || (hex.Length % 2 != 0))
                return null;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++) {
                //bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        public static T GetInstance<T>() {
            return Activator.CreateInstance<T>();
        }

        public static T GetInstance<T>(params object[] args) {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        public static bool IsDarkColor(byte[] rgb) {
            if (rgb.Length != 3)
                throw new ArgumentException("RGB array is invalid.");
            return rgb.Max() + rgb.Min() < 255; // L < 0.5 
        }

        public static void Shuffle<T>(this IList<T> list) {
            Shuffle<T>(list, list.Count);
        }

        public static void Shuffle<T>(this T[] array) {
            Shuffle<T>(array, array.Length);
        }

        private static void Shuffle<T>(dynamic list, int count) {
            Random r = new Random();
            for (int i = 1; i < count; i++) {
                int pos = r.Next(i + 1);
                T value = list[i];
                list[i] = list[pos];
                list[pos] = value;
            }
        }
    }
}
