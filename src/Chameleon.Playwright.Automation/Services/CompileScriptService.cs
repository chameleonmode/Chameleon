namespace Chameleon.Playwright.Automation.Services;
public class CompileScriptService
    : ICompileScriptService
{
    public Task<IExternalScript?> CompileScript(string script)
        => Task.Run(() =>
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

            IExternalScript instance = (IExternalScript)Activator.CreateInstance(type);
            return instance;
        });

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
        var references = GetReferences();

        string assemblyName = Path.GetRandomFileName();

        return CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private HashSet<MetadataReference> GetReferences()
    {    
        var domainAssemblys = AppDomain.CurrentDomain.GetAssemblies().Union(new Assembly[]
            {
                typeof(object).Assembly,
                typeof(Console).Assembly,
                typeof(Regex).Assembly,
                typeof(Microsoft.Playwright.Playwright).Assembly,
                typeof(IExternalScript).Assembly,
                typeof(System.Linq.Expressions.Expression).Assembly,
                typeof(TaskExtensions).Assembly
            });
        var metadataReferenceList = new HashSet<MetadataReference>();

        foreach (var assembl in domainAssemblys)
        {
            unsafe
            {
                if(!assembl.TryGetRawMetadata(out byte* blob, out int length))
                    continue;

                var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                var metadataReference = assemblyMetadata.GetReference();
                metadataReferenceList.Add(metadataReference);
            }
        }


        return metadataReferenceList;
    }
}
