using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    /// <summary>
    /// Specifies the date time unit.
    /// </summary>
    public enum DateTimeUnit {
        /// <summary>
        /// Indicates year.
        /// </summary>
        Year = 0,
        /// <summary>
        /// Indicates month.
        /// </summary>
        Month = 1,
        /// <summary>
        /// Indicates day.
        /// </summary>
        Day = 2,
        /// <summary>
        /// Indicates hour.
        /// </summary>
        Hour = 3,
        /// <summary>
        /// Indicates minute.
        /// </summary>
        Minute = 4,
        /// <summary>
        /// Indicates second.
        /// </summary>
        Second = 5,
        /// <summary>
        /// Indicates millisecond.
        /// </summary>
        Millisecond = 6
    }
}
