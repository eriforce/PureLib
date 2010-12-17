using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    /// <summary>
    /// Provides methods to wrap path.
    /// </summary>
    public static class PathWrapper {
        /// <summary>
        /// Wraps a local path to be absolute.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string WrapPath(this string path) {
            if (Regex.IsMatch(path, @"^([a-zA-Z]:|\\)?\\"))
                return path;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
