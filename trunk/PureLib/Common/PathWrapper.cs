using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public static class PathWrapper {
        private const string invalidPathPattern = "[\":/<>\\?\\*\\|]";

        public static string FilterInvalidChar(this string path, string replacement = "_") {
            return Regex.Replace(path, invalidPathPattern, replacement);
        }

        public static string WrapPath(this string path) {
            if (Regex.IsMatch(path, @"^([a-zA-Z]:|\\)?\\"))
                return path;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
