using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public sealed class Arguments {
        private const string ARGUMENT_NAME = "name";
        private const string _argumentPattern = $$"""^(/|\-{1,2})(?<{{ARGUMENT_NAME}}>\w+)$""";
        private static readonly Regex _argumentRegex = new(_argumentPattern, RegexOptions.Compiled | RegexOptions.NonBacktracking);

        private readonly Dictionary<string, ReadOnlyCollection<string>> _internal;

        public Arguments(string[] args, StringComparer stringComparer = null) {
            _internal = new Dictionary<string, ReadOnlyCollection<string>>(stringComparer);

            List<string> currentArgument = null;
            foreach (string arg in args) {
                Match m = _argumentRegex.Match(arg);
                if (m.Success && !decimal.TryParse(arg, out decimal argValue)) {
                    string currentName = m.Groups[ARGUMENT_NAME].Value;
                    currentArgument = new List<string>();
                    _internal.Add(currentName, currentArgument.AsReadOnly());
                }
                else if (currentArgument != null)
                    currentArgument.Add(arg);
            }
        }

        public bool Contains(string key) {
            return _internal.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value) {
            bool exists = TryGetValues(key, out ReadOnlyCollection<string> values);
            value = values?.FirstOrDefault();
            return exists;
        }

        public string GetValueOrDefault(string key, out bool exists, string defaultValue = default) {
            exists = TryGetValues(key, out ReadOnlyCollection<string> values);
            return exists ? values.FirstOrDefault() : defaultValue;
        }

        public T GetValueOrDefault<T>(string key, out bool exists, Func<string, T> parseFunc, T defaultValue = default) {
            exists = TryGetValues(key, out ReadOnlyCollection<string> values);
            return exists && values.Any() ? parseFunc(values.First()) : defaultValue;
        }

        public bool TryGetValues(string key, out ReadOnlyCollection<string> values) {
            ref ReadOnlyCollection<string> list = ref CollectionsMarshal.GetValueRefOrNullRef(_internal, key);
            bool exists = !Unsafe.IsNullRef(ref list);
            values = exists ? list : null;
            return exists;
        }
    }
}
