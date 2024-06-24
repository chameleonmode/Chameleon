using Chameleon.Interfaces.App.Automation.ExternalScript;
using Chameleon.Interfaces.App.Automation.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Chameleon.Playwright.Automation.Services
{
    public class CompileScriptService
    : ICompileScriptService
    {
        public IExternalScript CompileScript(string script)
        {
            Assembly assembly = CompileCode(script);

            if (assembly == null)
            {
                return null;
            }

            var type = assembly.GetTypes().FirstOrDefault();
            if (!typeof(IExternalScript).IsAssignableFrom(type))
            {
                throw new Exception("The script does not meet the requirements to run. Please implement the IExternalScript interface.");
            }

            IExternalScript instance = (IExternalScript) Activator.CreateInstance(type);

            return instance;
        }

        private Assembly CompileCode(string code)
        {
           CSharpCompilation compilation = CompileTree(code);

            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }

                string message = GenerateExceptionMessage(result);

                throw new Exception(message);
            }
        }

        private string GenerateExceptionMessage(EmitResult result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("It was a error when compiling the script:");

            IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (Diagnostic diagnostic in failures)
            {
                sb.AppendLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
            }

            return sb.ToString();
        }

        private CSharpCompilation CompileTree(string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            HashSet<MetadataReference> references = GetReferences();

            string assemblyName = Path.GetRandomFileName();

            return CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        private HashSet<MetadataReference> GetReferences()
        {
            var references = new HashSet<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.Playwright.Playwright).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IExternalScript).Assembly.Location)
            };

            typeof(Microsoft.Playwright.Playwright).Assembly.GetReferencedAssemblies()
                .Union(Assembly.GetEntryAssembly()?.GetReferencedAssemblies() ?? Enumerable.Empty<AssemblyName>()).ToList()
                .ForEach(a =>
                {
                    references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location));
                });

            return references;
        }
    }
}