using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    /// <summary>
    /// Provides methods about the environment.
    /// </summary>
    public static class EnvironmentHelper {
        /// <summary>
        /// Gets the threads number of current machine.
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfThreads() {
            return int.Parse(Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
        }
    }
}
