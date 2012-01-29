using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common.Entities {
    [Serializable]
    public class Arguments : Dictionary<string, List<string>> {
        public Arguments(string[] args) : base(StringComparer.OrdinalIgnoreCase) {
            if ((args == null) || (args.Length == 0))
                return;

            const string argumentName = "name";
            string argumentNamePattern = @"^(/|\-{{1,2}})(?<{0}>\w+)$".FormatWith(argumentName);
            string currentName = null;
            foreach (string arg in args) {
                Match m = Regex.Match(arg, argumentNamePattern);
                if (m.Success) {
                    currentName = m.Groups[argumentName].Value;
                    Add(currentName, new List<string>());
                }
                else if (!currentName.IsNullOrEmpty())
                    this[currentName].Add(arg);
            }
        }

        protected Arguments(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public string GetValue(string key) {
            if (!ContainsKey(key))
                throw new KeyNotFoundException("The key cannot be found in arguments.");
            return this[key].FirstOrDefault();
        }
    }
}
