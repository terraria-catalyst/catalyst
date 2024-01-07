using AsmResolver.DotNet;

namespace TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;

public readonly record struct AssemblyRewritingContext {
    public string AssemblyName { get; }

    public string AssemblyPath { get; }

    public AssemblyDefinition Assembly { get; }

    public ModuleDefinition Module { get; }

    public AssemblyRewritingContext(string assemblyName, string assemblyPath, AssemblyDefinition assembly, ModuleDefinition module) {
        AssemblyName = assemblyName;
        AssemblyPath = assemblyPath;
        Assembly = assembly;
        Module = module;
    }
}
