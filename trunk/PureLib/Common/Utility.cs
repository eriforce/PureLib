using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PureLib.Properties;

namespace PureLib.Common {
    /// <summary>
    /// Provides common methods.
    /// </summary>
    public static class Utility {
        private const string pairFormat = "{0} {1}";

        /// <summary>
        /// Gets friendly string of size.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static string ToFriendlyString(decimal size, int digits = 2) {
            return ToFriendlyString(size, null, digits);
        }

        /// <summary>
        /// Gets friendly string of size.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="unitNames"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets friendly string of timespan.
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="truncateUnit"></param>
        /// <param name="truncateToInteger"></param>
        /// <param name="truncateDigits"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToFriendlyString(this TimeSpan ts, DateTimeUnit? truncateUnit = null, 
            bool truncateToInteger = false, int truncateDigits = 3, string separator = null) {
                List<string> parts = new List<string>();
                ParseTimeSpan(ts, parts, truncateUnit, truncateToInteger, truncateDigits);
                return string.Join(separator.IsNullOrEmpty() ? Resources.Comma : separator, parts);
        }

        /// <summary>
        /// Parses the data size.
        /// </summary>
        /// <param name="sizeString"></param>
        /// <returns></returns>
        public static double ParseDataSize(string sizeString) {
            const string numberName = "number";
            const string unitName = "unit";
            SizeUnit[] units = (SizeUnit[])Enum.GetValues(typeof(SizeUnit));
            Dictionary<string, double> unitKeyValueMaps = units.ToDictionary(u => u.ToString().First().ToString(),
                u => Math.Pow(2, (int)u * 10), StringComparer.OrdinalIgnoreCase);
            string sizeStringPattern = @"^(?<{0}>\d+)(?<{1}>[{2}]?)b?$".FormatWith(
                numberName, unitName, string.Join(string.Empty, unitKeyValueMaps.Keys));
            Match m = Regex.Match(sizeString, sizeStringPattern, RegexOptions.IgnoreCase);
            if (!m.Success)
                throw new ArgumentException("Size string cannot be parsed.");

            double result = double.Parse(m.Groups[numberName].Value);
            string unit = m.Groups[unitName].Value.ToLower();
            if (!unit.IsNullOrEmpty())
                result *= unitKeyValueMaps[unit];
            return result;
        }

        /// <summary>
        /// Gets the duration of the code execution.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan GetExecutingDuration(Action action) {
            DateTime start = DateTime.UtcNow;
            action();
            return DateTime.UtcNow - start;
        }

        /// <summary>
        /// Gets the number of weekdays in a period.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Indicates whether the specified System.String object is null or an System.String.Empty string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str) {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Replaces the format item in a specified System.String with the text equivalent of 
        /// the value of a corresponding System.Object instance in a specified array.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, params object[] args) {
            return string.Format(format, args);
        }

        /// <summary>
        /// Returns the time string in ISO8601 format.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToIso8601(this DateTime time) {
            return time.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
        }

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent System.String representation encoded with base 64 digits.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] bytes) {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts the specified System.String, which encodes binary data as base 64 digits,
        /// to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(this string str) {
            return Convert.FromBase64String(str);
        }

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent hexadecimal System.String representation.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bytes) {
            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes) {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts the specified hexadecimal System.String to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the instance of a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Gets the instance of a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T GetInstance<T>(params object[] args) {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// Indicates whether the RGB represents a dark color.
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static bool IsDarkColor(this byte[] rgb) {
            return rgb.Max() + rgb.Min() < 255; // L < 0.5 
        }

        private static void ParseTimeSpan(TimeSpan ts, List<string> parts, 
            DateTimeUnit? truncateUnit, bool truncateToInteger, int truncateDigits) {
                if (ts.Days != 0) {
                    if (truncateUnit == DateTimeUnit.Day) {
                        parts.Add(pairFormat.FormatWith(truncateToInteger ? 
                            ts.Days : Math.Round(ts.TotalDays, truncateDigits), Resources.Days));
                        return;
                    }
                    else
                        parts.Add(pairFormat.FormatWith(ts.Days, Resources.Days));
                }

                if (ts.Hours != 0) {
                    if (truncateUnit == DateTimeUnit.Hour) {
                        parts.Add(pairFormat.FormatWith(truncateToInteger ? 
                            ts.Hours : Math.Round(ts.TotalHours % 24, truncateDigits), Resources.Hours));
                        return;
                    }
                    else
                        parts.Add(pairFormat.FormatWith(ts.Hours, Resources.Hours));
                }

                if (ts.Minutes != 0) {
                    if (truncateUnit == DateTimeUnit.Minute) {
                        parts.Add(pairFormat.FormatWith(truncateToInteger ? 
                            ts.Minutes : Math.Round(ts.TotalMinutes % 60, truncateDigits), Resources.Minutes));
                        return;
                    }
                    else
                        parts.Add(pairFormat.FormatWith(ts.Minutes, Resources.Minutes));
                }

                if (ts.Seconds != 0) {
                    if (truncateUnit == DateTimeUnit.Second) {
                        parts.Add(pairFormat.FormatWith(truncateToInteger ? 
                            ts.Seconds : Math.Round(ts.TotalSeconds % 60, truncateDigits), Resources.Seconds));
                        return;
                    }
                    else
                        parts.Add(pairFormat.FormatWith(ts.Seconds, Resources.Seconds));
                }

                if (ts.Milliseconds != 0) {
                    if (truncateUnit == DateTimeUnit.Millisecond) {
                        parts.Add(pairFormat.FormatWith(truncateToInteger ? 
                            ts.Milliseconds : Math.Round(ts.TotalMilliseconds % 1000, truncateDigits), Resources.Milliseconds));
                        return;
                    }
                    else
                        parts.Add(pairFormat.FormatWith(ts.Milliseconds, Resources.Milliseconds));
                }
        }
    }
}
