using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PureLib.Common {
    public static class PathWrapper {
        private static readonly SortedSet<char> _invalidFileNameChars = new(Path.GetInvalidFileNameChars());

        public static string EscapeFileName(this string fileName, char replacement = '_') {
            string result = null;
            Utility.RentCharSpace(fileName.Length, buffer => {
                for (int i = 0; i < fileName.Length; i++) {
                    char c = fileName[i];
                    buffer[i] = !_invalidFileNameChars.Contains(c) ? c : replacement;
                }
                result = new string(buffer[..fileName.Length]);
            });
            return result;
        }

        public static string EscapePath(this string path, char replacement = '_') {
            string result = null;
            Utility.RentCharSpace(path.Length, buffer => {
                for (int i = 0; i < path.Length; i++) {
                    char c = path[i];
                    buffer[i] =
                        c == Path.DirectorySeparatorChar ||
                        c == Path.VolumeSeparatorChar ||
                        !_invalidFileNameChars.Contains(c) ? c : replacement;
                }
                result = new string(buffer[..path.Length]);
            });
            return result;
        }

        public static string MakeFullPath(this string path) {
            return Path.IsPathRooted(path) ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public static string GetAppPath(string ext) {
            return Path.ChangeExtension(Environment.ProcessPath, ext);
        }

        public static string EscapePathForCmd(this string path) {
            return path.Replace(Path.DirectorySeparatorChar.ToString(), new string(Path.DirectorySeparatorChar, 2));
        }
    }
}
