using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public static class PathWrapper {
        private const string invalidPathPattern = "[\":/<>\\?\\*\\|]";
        private static readonly string escapedDirectorySeparator = "{0}{0}".FormatWith(Path.DirectorySeparatorChar);

        public static string FilterInvalidChar(this string path, string replacement = "_") {
            return Regex.Replace(path, invalidPathPattern, replacement);
        }

        public static string WrapPath(this string path) {
            if (Regex.IsMatch(path, @"^([a-zA-Z]:|\\)?\\"))
                return path;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public static string CombineWithAppName(this string ext) {
            return Path.ChangeExtension(Process.GetCurrentProcess().MainModule.FileName, ext).WrapPath();
        }

        public static string EscapePathForCmd(this string path) {
            return path.Replace(Path.DirectorySeparatorChar.ToString(), escapedDirectorySeparator);
        }
    }
}
