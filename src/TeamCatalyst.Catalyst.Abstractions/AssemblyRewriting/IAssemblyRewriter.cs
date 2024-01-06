using AsmResolver.DotNet;

namespace TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;

public interface IAssemblyRewriter {
    bool ProcessAssembly(AssemblyDefinition asm);
}
