using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace PureLib.Generators.DicToInstGenerator {
    [Generator]
    public class DictionaryToInstanceGenerator : IIncrementalGenerator {
        private const string FromDictionaryGeneratorNamespace = "PureLib.Generators.DicToInstGenerator";
        private const string FromDictionaryAttributeName = "FromDictionaryAttribute";
        private const string FromDictionaryAttributeFullQualifiedName =
            $"{FromDictionaryGeneratorNamespace}.{FromDictionaryAttributeName}";
        private const string IgnoreAttributeName = "IgnoreAttribute";

        private const string FromDictionaryGeneratorAttributeSource = $$"""
using System;

namespace {{FromDictionaryGeneratorNamespace}} {
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class {{FromDictionaryAttributeName}} : Attribute {
    }
}
""";

        private const string IgnorePropertyAttributeSource = $$"""
using System;

namespace {{FromDictionaryGeneratorNamespace}} {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class {{IgnoreAttributeName}} : Attribute {
    }
}
""";

        public void Initialize(IncrementalGeneratorInitializationContext context) {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                $"{FromDictionaryAttributeName}.g.cs", SourceText.From(FromDictionaryGeneratorAttributeSource, Encoding.UTF8)));
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                $"{IgnoreAttributeName}.g.cs", SourceText.From(IgnorePropertyAttributeSource, Encoding.UTF8)));

            IncrementalValuesProvider<ClassToGenerate> classesToGenerate = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    FromDictionaryAttributeFullQualifiedName,
                    predicate: (node, _) => node is ClassDeclarationSyntax,
                    transform: GetClassToGenerate);

            context.RegisterSourceOutput(classesToGenerate,
                static (spc, classToGenerate) => Execute(in classToGenerate, spc));
        }

        static void Execute(in ClassToGenerate classToGenerate, SourceProductionContext context) {
            var source = GenerateExtensionSource(classToGenerate);
            context.AddSource($"DictionaryTo{classToGenerate.ClassName}Extensions.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        static ClassToGenerate GetClassToGenerate(GeneratorAttributeSyntaxContext context, CancellationToken ct) {
            ct.ThrowIfCancellationRequested();

            var classSymbol = context.TargetSymbol as INamedTypeSymbol;

            return new ClassToGenerate(
                 classSymbol.Name,
                 classSymbol.DeclaredAccessibility,
                 classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToString(),
                 classSymbol.GetMembers().OfType<IPropertySymbol>().Where(p => {
                     if (p.SetMethod is null || p.SetMethod.DeclaredAccessibility <= Accessibility.Internal)
                         return false;

                     var attrs = p.GetAttributes();
                     return !attrs.Any(a => a.AttributeClass?.Name == IgnoreAttributeName &&
                         a.AttributeClass.ContainingNamespace.ToString() == FromDictionaryGeneratorNamespace);
                 }).ToArray()
            );
        }

        static string GenerateExtensionSource(ClassToGenerate @class) {
            return $$"""
                using System.Collections.Generic;
                using System.Runtime.CompilerServices;
                using System.Runtime.InteropServices;

                namespace {{@class.Namespace}} {
                    {{(@class.AccessLevel == Accessibility.Public ? "public" : "internal")}} static class DictionaryTo{{@class.ClassName}}Extensions {
                        public static {{@class.ClassName}} To{{@class.ClassName}}(this Dictionary<string, object> dic) {
                            {{string.Join(@"
            ", @class.Properties.Select(p => $"""ref var refOf{p.Name} = ref CollectionsMarshal.GetValueRefOrNullRef(dic, "{p.Name}");"""))}}

                            return new {{@class.ClassName}} {
                                {{string.Join(@"
                ", @class.Properties.Select(p => $"""{p.Name} = Unsafe.IsNullRef(ref refOf{p.Name}) ? default : ({p.Type.Name})refOf{p.Name},"""))}}
                            };
                        }
                    }
                }
                """;
        }
    }
}
