using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    /// <summary>
    /// Provides common methods.
    /// </summary>
    public static class Utility {
        /// <summary>
        /// Parses the data size.
        /// </summary>
        /// <param name="sizeString"></param>
        /// <returns></returns>
        public static long ParseDataSize(string sizeString) {
            Dictionary<string, long> units = new Dictionary<string, long> { 
                { "k", 1 << 10 },
                { "m", 1 << 20 },
                { "g", 1 << 30 },
                { "t", 1 << 40 },
                { "p", 1 << 50 } 
            };
            const string numberName = "number";
            const string unitName = "unit";
            string sizeStringPattern = @"^(?<{0}>\d+)(?<{1}>[{2}]?)b?$".FormatWith(numberName, unitName, string.Join(string.Empty, units.Keys));
            Match m = Regex.Match(sizeString, sizeStringPattern, RegexOptions.IgnoreCase);
            if (!m.Success)
                throw new ApplicationException("Size string cannot be parsed.");

            long result = long.Parse(m.Groups[numberName].Value);
            string unit = m.Groups[unitName].Value.ToLower();
            if (!unit.IsNullOrEmpty())
                result *= units[unit];
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
        /// Replaces the format item in a specified System.String with the text equivalent of the value of a corresponding System.Object instance in a specified array.
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
        /// Converts the specified System.String, which encodes binary data as base 64 digits, to an equivalent 8-bit unsigned integer array.
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
    }
}
