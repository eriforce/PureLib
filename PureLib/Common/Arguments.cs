using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public sealed class Arguments : Dictionary<string, List<string>> {
        private const string ARGUMENT_NAME = "name";
        private const string _argumentPattern = $$"""^(/|\-{1,2})(?<{{ARGUMENT_NAME}}>\w+)$""";
        private static readonly Regex _argumentRegex = new(_argumentPattern, RegexOptions.Compiled | RegexOptions.NonBacktracking);

        public Arguments(string[] args) : base(StringComparer.OrdinalIgnoreCase) {
            if ((args == null) || (args.Length == 0))
                return;

            string currentName = null;
            foreach (string arg in args) {
                Match m = _argumentRegex.Match(arg);
                if (m.Success && !decimal.TryParse(arg, out decimal argValue)) {
                    currentName = m.Groups[ARGUMENT_NAME].Value;
                    Add(currentName, new List<string>());
                }
                else if (!currentName.IsNullOrEmpty())
                    this[currentName].Add(arg);
            }
        }

        public string GetValue(string key) {
            if (!ContainsKey(key))
                throw new KeyNotFoundException("The key cannot be found in arguments.");
            return this[key].FirstOrDefault();
        }
    }
}
