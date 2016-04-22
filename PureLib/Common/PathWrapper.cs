using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public static class PathWrapper {
        public static string FilterFileName(this string fileName, string replacement = "_") {
            string pattern = "[{0}]".FormatWith(string.Join(string.Empty,
                Path.GetInvalidFileNameChars().Select(c => Regex.Escape(c.ToString()))));
            return Regex.Replace(fileName, pattern, replacement);
        }

        public static string FilterPath(this string path, string replacement = "_", string additionalInvalid = "*?") {
            string pattern = "[{0}]".FormatWith(string.Join(string.Empty,
                Path.GetInvalidPathChars().Concat(additionalInvalid).Distinct().Select(c => Regex.Escape(c.ToString()))));
            return Regex.Replace(path, pattern, replacement);
        }

        public static string MakeFullPath(this string path) {
            return Path.IsPathRooted(path) ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public static string GetAppPath(string ext) {
            return Path.ChangeExtension(Process.GetCurrentProcess().MainModule.FileName, ext);
        }

        public static string EscapePathForCmd(this string path) {
            return path.Replace(Path.DirectorySeparatorChar.ToString(), new string(Path.DirectorySeparatorChar, 2));
        }
    }
}
