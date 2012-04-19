using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    internal static class SizeUnitDisplay {
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

        public static string ToDisplayName(this SizeUnit unit, Dictionary<SizeUnit, string> unitNames) {
            string displayName;
            if (unitNames == null || !unitNames.TryGetValue(unit, out displayName))
                displayName = _defaultUnitNames[unit];
            return displayName;
        }
    }
}
