using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PureLib.Properties;

namespace PureLib.Common {
    public static class SizeUnitDisplay {
        private const string REGEX_NAME_NUMBER = "number";
        private const string REGEX_NAME_UNIT = "unit";

        private static readonly Dictionary<SizeUnit, string> _defaultSizeUnitNames = new Dictionary<SizeUnit, string>() {
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
        private static readonly Dictionary<string, double> _unitKeyValueMaps;
        private static readonly Regex _sizeRegex;

        public static string UnitFormat { get; set; } = "{0} {1}";

        static SizeUnitDisplay() {
            SizeUnit[] units = (SizeUnit[])Enum.GetValues(typeof(SizeUnit));
            _unitKeyValueMaps = units.ToDictionary(u => u.ToString().First().ToString(),
                u => Math.Pow(2, (int)u * 10), StringComparer.OrdinalIgnoreCase);

            string sizeStringPattern = @"^(?<{0}>\d+) *(?<{1}>[{2}]?)b?$".FormatWith(
                REGEX_NAME_NUMBER, REGEX_NAME_UNIT, string.Join(string.Empty, _unitKeyValueMaps.Keys));
            _sizeRegex = new Regex(sizeStringPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static long ParseSize(this string sizeString) {
            long size;
            if (long.TryParse(sizeString, out size))
                return size;

            Match m = _sizeRegex.Match(sizeString);
            if (!m.Success)
                throw new ArgumentException("Size string cannot be parsed.");

            double result = double.Parse(m.Groups[REGEX_NAME_NUMBER].Value);
            string unit = m.Groups[REGEX_NAME_UNIT].Value.ToLower();
            if (!unit.IsNullOrEmpty())
                result *= _unitKeyValueMaps[unit];
            return (long)result;
        }

        public static string ToFriendlyString(this long size, int digits = 2, Dictionary<SizeUnit, string> unitNames = null) {
            decimal displaySize = size;
            int maxIndex = Enum.GetValues(typeof(SizeUnit)).Length - 1;
            int unitIndex = 0;
            while ((displaySize >= 1000) && (unitIndex < maxIndex)) {
                displaySize /= 1024;
                unitIndex++;
            }
            SizeUnit unit = (SizeUnit)unitIndex;
            return UnitFormat.FormatWith(Math.Round(displaySize, digits), unit.ToDisplayName(unitNames));
        }

        private static string ToDisplayName(this SizeUnit unit, Dictionary<SizeUnit, string> unitNames = null) {
            string displayName;
            if (unitNames == null || !unitNames.TryGetValue(unit, out displayName))
                displayName = _defaultSizeUnitNames[unit];
            return displayName;
        }
    }
}
