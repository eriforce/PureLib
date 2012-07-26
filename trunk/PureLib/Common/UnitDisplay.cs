using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PureLib.Properties;

namespace PureLib.Common {
    public static class UnitDisplay {
        private const string pairFormat = "{0} {1}";

        private static readonly Dictionary<SizeUnit, string> _defaultUnitNames = new Dictionary<SizeUnit, string>() {
            { SizeUnit.Byte,      "B"  },
            { SizeUnit.Kilobyte,  "KB" },
            { SizeUnit.Megabyte,  "MB" },
            { SizeUnit.Gigabyte,  "GB" },
            { SizeUnit.Terabyte,  "TB" },
            { SizeUnit.Petabyte,  "PB" },
            { SizeUnit.Exabyte,   "EB" },
            { SizeUnit.Zettabyte, "ZB" },
            { SizeUnit.Yottabyte, "YB" },
        };

        public static long ParseSize(this string sizeString) {
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

        public static string ToFriendlyString(this long size, int digits = 2, Dictionary<SizeUnit, string> unitNames = null) {
            int maxIndex = Enum.GetValues(typeof(SizeUnit)).Length - 1;
            int unitIndex = 0;
            while ((size >= 1000) && (unitIndex < maxIndex)) {
                size /= 1024;
                unitIndex++;
            }
            SizeUnit unit = (SizeUnit)unitIndex;
            return pairFormat.FormatWith(Math.Round((decimal)size, digits), unit.ToDisplayName(unitNames));
        }

        public static string ToFriendlyString(this TimeSpan ts, DateTimeUnit? truncateUnit = null,
                bool truncateToInteger = false, int truncateDigits = 3, string separator = null) {
            List<string> parts = new List<string>();
            ParseTimeSpan(ts, parts, truncateUnit, truncateToInteger, truncateDigits);
            return string.Join(separator.IsNullOrEmpty() ? Resources.Common_Comma : separator, parts);
        }

        private static string ToDisplayName(this SizeUnit unit, Dictionary<SizeUnit, string> unitNames = null) {
            string displayName;
            if (unitNames == null || !unitNames.TryGetValue(unit, out displayName))
                displayName = _defaultUnitNames[unit];
            return displayName;
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
