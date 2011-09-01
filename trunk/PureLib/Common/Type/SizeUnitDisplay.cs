using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    /// <summary>
    /// Provides SizeUnit display methods.
    /// </summary>
    internal static class SizeUnitDisplay {
        private static readonly Dictionary<SizeUnit, string> defaultUnitNames = new Dictionary<SizeUnit, string>() {
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

        /// <summary>
        /// Gets display name of size unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="unitNames"></param>
        /// <returns></returns>
        public static string ToDisplayName(this SizeUnit unit, Dictionary<SizeUnit, string> unitNames) {
            string displayName;
            if (unitNames == null || !unitNames.TryGetValue(unit, out displayName))
                displayName = defaultUnitNames[unit];
            return displayName;
        }
    }
}
