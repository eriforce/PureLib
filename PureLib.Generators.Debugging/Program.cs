using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PureLib.Generators.DicToInstGenerator;

namespace PureLib.Generators.Debugging {
    class Program {
        static void Main(string[] args) {
            var dic = new Dictionary<string, object> {
                { "Id", 123 },
                { "Name", "erich" },
                { "Value", "v" },
            };
            var payload = dic.ToPayload();

            Console.WriteLine(payload.Name);
        }
    }

    [FromDictionary]
    public class Payload {
        public int Id { get; set; }
        public string Name { get; set; }
        [Ignore]
        public string Value { get; init; }
    }
}
