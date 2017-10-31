using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PureLib.Properties;

namespace PureLib.Common {
    public static class TimeUnitDisplay {
        public static string UnitFormat { get; set; } = "{0} {1}";

        public static string ToFriendlyString(this TimeSpan ts, DateTimeUnit? truncateUnit = null,
                bool truncateToInteger = false, int truncateDigits = 3, string separator = null) {
            List<string> parts = new List<string>();
            ParseTimeSpan(ts, parts, truncateUnit, truncateToInteger, truncateDigits);
            return string.Join(separator.IsNullOrEmpty() ? Resources.Common_Comma : separator, parts);
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
                    parts.Add(UnitFormat.FormatWith(truncateToInteger ?
                        timePartValue : Math.Round(timePartFloat, truncateDigits), timePartName));
                    toContinue = false;
                }
                else
                    parts.Add(UnitFormat.FormatWith(timePartValue, timePartName));
            }
            return toContinue;
        }
    }
}
