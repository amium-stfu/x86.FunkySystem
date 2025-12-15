using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FunkySystem.Core
{

    public class FunkySequence
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool Visible { get; set; }
        public string Uri { get; set; }

        [JsonIgnore]
        public FunkyPlcBase? PlcInstance { get; private set; }
        [JsonIgnore]
        internal PluginLoadContext? LoadContext { get; private set; }
        [JsonIgnore]
        public IReadOnlyList<Diagnostic>? LastDiagnostics { get; private set; }

        public FunkySequence(string name, string uri)
        {
            Name = name;
            Uri = uri;
            IsActive = false;
            Visible = true;
        }

        public async Task CreatePlcInstanceAsync(
            IEnumerable<string>? additionalReferencePaths = null,
            CancellationToken cancellationToken = default)
        {
            UnloadPlcInstance();

            var csPath = FunkyPlcRuntimeCompiler.ResolveToFilePath(Uri);

            var result = await FunkyPlcRuntimeCompiler.CompileAndLoadAsync(
                csFilePaths: new[] { csPath },
                additionalReferencePaths: additionalReferencePaths,
                assemblyName: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            LastDiagnostics = result.Diagnostics;

            if (result.Assembly == null)
                throw new InvalidOperationException("Compile failed. See LastDiagnostics.");

            LoadContext = result.LoadContext;
            PlcInstance = FunkyPlcRuntimeCompiler.CreatePlcInstance(result.Assembly);
        }

        public void UnloadPlcInstance()
        {
            PlcInstance = null;

            if (LoadContext != null)
            {
                var ctx = LoadContext;
                FunkyPlcRuntimeCompiler.Unload(ref ctx);
                LoadContext = null;
            }
        }
    }


    internal class SequenceManager
    {
        public readonly Dictionary<string,Dictionary<string, FunkySequence>> Sequences 
            = new Dictionary<string,Dictionary<string, FunkySequence>>(StringComparer.OrdinalIgnoreCase);


    }

    internal sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver? _resolver;

        // isCollectible=true ist Voraussetzung für Unload()
        public PluginLoadContext(string? pluginMainPath = null, bool isCollectible = true)
            : base(isCollectible)
        {
            if (!string.IsNullOrWhiteSpace(pluginMainPath))
                _resolver = new AssemblyDependencyResolver(pluginMainPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // Optional: DependencyResolver für zusätzliche Abhängigkeiten (z.B. wenn du später DLLs daneben legst)
            if (_resolver is null) return null;

            var path = _resolver.ResolveAssemblyToPath(assemblyName);
            if (path is null) return null;

            return LoadFromAssemblyPath(path);
        }
    }



    internal sealed record PluginCompileResult(
        Assembly Assembly,
        PluginLoadContext LoadContext,
        IReadOnlyList<Diagnostic> Diagnostics);

    internal static class FunkyPlcRuntimeCompiler
    {
        internal static async Task<PluginCompileResult> CompileAndLoadAsync(
            IEnumerable<string> csFilePaths,
            IEnumerable<string>? additionalReferencePaths = null,
            string? assemblyName = null,
            CancellationToken cancellationToken = default)
        {
            var files = csFilePaths?.ToArray() ?? Array.Empty<string>();
            if (files.Length == 0) throw new ArgumentException("No .cs files provided", nameof(csFilePaths));

            foreach (var f in files)
                if (!File.Exists(f)) throw new FileNotFoundException("Source file not found", f);

            // SyntaxTrees
            var parseOptions = CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.Preview); // für .NET 10 / C# preview falls nötig

            var trees = new List<SyntaxTree>(files.Length);
            foreach (var path in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var code = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
                trees.Add(CSharpSyntaxTree.ParseText(code, parseOptions, path));
            }

            // References:
            // - TrustedPlatformAssemblies (TPA) => alle Standard-Framework-Assemblies, die die App geladen hat
            // - plus: Host-Assembly (enthält FunkyPlcBase) und ggf. FunkySystem.*
            var refs = new List<MetadataReference>();

            var tpa = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string) ?? "";
            foreach (var p in tpa.Split(Path.PathSeparator))
            {
                if (!string.IsNullOrWhiteSpace(p) && File.Exists(p))
                    refs.Add(MetadataReference.CreateFromFile(p));
            }

            // Host-Assembly explizit (wichtig, falls nicht im TPA enthalten)
            refs.Add(MetadataReference.CreateFromFile(typeof(FunkyPlcBase).Assembly.Location));

            if (additionalReferencePaths != null)
            {
                foreach (var r in additionalReferencePaths)
                {
                    if (string.IsNullOrWhiteSpace(r)) continue;
                    if (!File.Exists(r)) throw new FileNotFoundException("Reference not found", r);
                    refs.Add(MetadataReference.CreateFromFile(r));
                }
            }

            assemblyName ??= "FunkyPlcUser_" + Guid.NewGuid().ToString("N");

            var compilationOptions = new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: trees,
                references: refs,
                options: compilationOptions);

            await using var peStream = new MemoryStream();
            await using var pdbStream = new MemoryStream();

            var emitOptions = new Microsoft.CodeAnalysis.Emit.EmitOptions(
                debugInformationFormat: Microsoft.CodeAnalysis.Emit.DebugInformationFormat.PortablePdb);

            var emit = compilation.Emit(
                peStream,
                pdbStream: pdbStream,
                options: emitOptions,
                cancellationToken: cancellationToken);

            var diagnostics = emit.Diagnostics.ToArray();
            if (!emit.Success)
            {
                return new PluginCompileResult(
                    Assembly: null!,
                    LoadContext: null!,
                    Diagnostics: diagnostics);
            }

            peStream.Position = 0;
            pdbStream.Position = 0;

            // Collectible LoadContext, damit Unload möglich ist
            var alc = new PluginLoadContext(pluginMainPath: files[0], isCollectible: true);
            var asm = alc.LoadFromStream(peStream, pdbStream);

            return new PluginCompileResult(asm, alc, diagnostics);
        }

        internal static FunkyPlcBase CreatePlcInstance(Assembly asm)
        {
            var baseType = typeof(FunkyPlcBase);

            var plcType = asm
                .GetTypes()
                .FirstOrDefault(t => !t.IsAbstract && baseType.IsAssignableFrom(t));

            if (plcType == null)
                throw new InvalidOperationException("No non-abstract type deriving from FunkyPlcBase found.");

            return (FunkyPlcBase)Activator.CreateInstance(plcType)!;
        }

        internal static string ResolveToFilePath(string uriOrPath)
        {
            if (string.IsNullOrWhiteSpace(uriOrPath))
                throw new ArgumentException("Uri/path is empty", nameof(uriOrPath));

            if (System.Uri.TryCreate(uriOrPath, System.UriKind.Absolute, out var u))
            {
                if (u.IsFile) return u.LocalPath;
                throw new NotSupportedException($"Only file:// URIs are supported. Got: {u.Scheme}");
            }

            return Path.GetFullPath(uriOrPath);
        }


        internal static void Unload(ref PluginLoadContext? alc)
        {
            if (alc == null) return;
            alc.Unload();
            alc = null;

            // Hinweis: tatsächliches Unload passiert asynchron durch den GC.
            // Für Tests kann man mehrfach GC.Collect/WaitForPendingFinalizers aufrufen.
        }
    }



}
