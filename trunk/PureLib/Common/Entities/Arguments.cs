using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PureLib.Common.Entities {
    /// <summary>
    /// A structure of the startup arguments from command-line.
    /// </summary>
    public class Arguments : Dictionary<string, List<string>> {
        /// <summary>
        /// Initializes a new instance of Arguments.
        /// </summary>
        /// <param name="args"></param>
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

        /// <summary>
        /// Gets the value by provided key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key) {
            if (!ContainsKey(key))
                throw new ApplicationException("The key cannot be found in arguments.");
            return this[key].FirstOrDefault();
        }
    }
}
