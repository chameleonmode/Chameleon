using Chameleon.Interfaces.App.Automation.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Chameleon.Avalonia.Playwright.Automation.Services
{
    public class CompileScriptService
    : ICompileScriptService
    {
        public const string ScriptMainMethodName = "Run";

        public MethodInfo? CompileScript(string script)
        {
            Assembly assembly = CompileCode(script);

            if (assembly == null)
            {
                return null;
            }

            var type = assembly.GetTypes().FirstOrDefault();
            MethodInfo method = type.GetMethod(ScriptMainMethodName);

            return method;
        }

        private Assembly CompileCode(string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            string assemblyName = Path.GetRandomFileName();

            var references = new HashSet<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.Playwright.Playwright).Assembly.Location)
            };

            typeof(Microsoft.Playwright.Playwright).Assembly.GetReferencedAssemblies()
                .Union(Assembly.GetEntryAssembly()?.GetReferencedAssemblies() ?? Enumerable.Empty<AssemblyName>()).ToList()
                .ForEach(a =>
                {
                    references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location));
                });

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.WriteLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                    return null;
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }
            }
        }
    }
}