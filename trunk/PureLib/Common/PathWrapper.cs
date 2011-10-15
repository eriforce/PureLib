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
        private const string invalidPathPattern = "[\":/<>\\?\\*\\|]";

        /// <summary>
        /// Filters invalid characters in the path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string FilterInvalidChar(this string path, string replacement = "_") {
            return Regex.Replace(path, invalidPathPattern, replacement);
        }

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
