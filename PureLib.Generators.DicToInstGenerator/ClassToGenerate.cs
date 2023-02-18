using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace PureLib.Generators.DicToInstGenerator {
    internal readonly struct ClassToGenerate {
        public readonly string ClassName;
        public readonly Accessibility AccessLevel;
        public readonly string Namespace;
        public readonly IPropertySymbol[] Properties;

        public ClassToGenerate(
            string className,
            Accessibility accessLevel,
            string @namespace,
            IPropertySymbol[] properties
            ) {
            ClassName = className;
            AccessLevel = accessLevel;
            Namespace = @namespace;
            Properties = properties;
        }
    }
}
