using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common {
    public class Arguments : Dictionary<string, List<string>> {
        private const string ARGUMENT_NAME = "name";
        private static readonly string _argumentPattern = @"^(/|\-{{1,2}})(?<{0}>\w+)$".FormatWith(ARGUMENT_NAME);
        private static readonly Regex _argumentRegex = new Regex(_argumentPattern, RegexOptions.Compiled);

        public Arguments(string[] args) : base(StringComparer.OrdinalIgnoreCase) {
            if ((args == null) || (args.Length == 0))
                return;

            string currentName = null;
            foreach (string arg in args) {
                Match m = _argumentRegex.Match(arg);
                decimal argValue;
                if (m.Success && !decimal.TryParse(arg, out argValue)) {
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
