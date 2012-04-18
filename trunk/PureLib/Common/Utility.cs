using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PureLib.Properties;

namespace PureLib.Common {
    public static class Utility {
        private const string pairFormat = "{0} {1}";

        public static string ToFriendlyString(decimal size, int digits = 2) {
            return ToFriendlyString(size, null, digits);
        }

        public static string ToFriendlyString(decimal size, Dictionary<SizeUnit, string> unitNames, int digits = 2) {
            int maxIndex = Enum.GetValues(typeof(SizeUnit)).Length - 1;
            int unitIndex = 0;
            while ((size >= 1000) && (unitIndex < maxIndex)) {
                size /= 1024;
                unitIndex++;
            }
            SizeUnit unit = (SizeUnit)unitIndex;
            return pairFormat.FormatWith(Math.Round(size, digits), unit.ToDisplayName(unitNames));
        }

        public static string ToFriendlyString(this TimeSpan ts, DateTimeUnit? truncateUnit = null, 
            bool truncateToInteger = false, int truncateDigits = 3, string separator = null) {
                List<string> parts = new List<string>();
                ParseTimeSpan(ts, parts, truncateUnit, truncateToInteger, truncateDigits);
                return string.Join(separator.IsNullOrEmpty() ? Resources.Common_Comma : separator, parts);
        }

        public static long ParseDataSize(string sizeString) {
            const string numberName = "number";
            const string unitName = "unit";
            SizeUnit[] units = (SizeUnit[])Enum.GetValues(typeof(SizeUnit));
            Dictionary<string, double> unitKeyValueMaps = units.ToDictionary(u => u.ToString().First().ToString(),
                u => Math.Pow(2, (int)u * 10), StringComparer.OrdinalIgnoreCase);
            string sizeStringPattern = @"^(?<{0}>\d+) *(?<{1}>[{2}]?)b?$".FormatWith(
                numberName, unitName, string.Join(string.Empty, unitKeyValueMaps.Keys));
            Match m = Regex.Match(sizeString, sizeStringPattern, RegexOptions.IgnoreCase);
            if (!m.Success)
                throw new ArgumentException("Size string cannot be parsed.");

            double result = double.Parse(m.Groups[numberName].Value);
            string unit = m.Groups[unitName].Value.ToLower();
            if (!unit.IsNullOrEmpty())
                result *= unitKeyValueMaps[unit];
            return (long)result;
        }

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

        private static void ParseTimeSpan(TimeSpan ts, List<string> parts,
            DateTimeUnit? truncateUnit, bool truncateToInteger, int truncateDigits) {

            if (!ParseTimeSpanPart(DateTimeUnit.Day, ts.Days, ts.TotalDays,
                Resources.Common_Days, parts, truncateUnit, truncateToInteger, truncateDigits))
                return;
            if (!ParseTimeSpanPart(DateTimeUnit.Hour, ts.Hours, ts.TotalHours % 24,
                Resources.Common_Hours, parts, truncateUnit, truncateToInteger, truncateDigits))
                return;
            if (!ParseTimeSpanPart(DateTimeUnit.Minute, ts.Minutes, ts.TotalMinutes % 60,
                Resources.Common_Minutes, parts, truncateUnit, truncateToInteger, truncateDigits))
                return;
            if (!ParseTimeSpanPart(DateTimeUnit.Second, ts.Seconds, ts.TotalSeconds % 60,
                Resources.Common_Seconds, parts, truncateUnit, truncateToInteger, truncateDigits))
                return;
            if (!ParseTimeSpanPart(DateTimeUnit.Millisecond, ts.Milliseconds, ts.TotalMilliseconds % 1000,
                Resources.Common_Milliseconds, parts, truncateUnit, truncateToInteger, truncateDigits))
                return;
        }

        private static bool ParseTimeSpanPart(DateTimeUnit timePart, int timePartValue, double timePartFloat,
            string timePartName, List<string> parts, DateTimeUnit? truncateUnit, bool truncateToInteger, int truncateDigits) {

            bool toContinue = true;
            if (timePartValue != 0) {
                if (truncateUnit == timePart) {
                    parts.Add(pairFormat.FormatWith(truncateToInteger ?
                        timePartValue : Math.Round(timePartFloat, truncateDigits), timePartName));
                    toContinue = false;
                }
                else
                    parts.Add(pairFormat.FormatWith(timePartValue, timePartName));
            }
            return toContinue;
        }
    }
}
