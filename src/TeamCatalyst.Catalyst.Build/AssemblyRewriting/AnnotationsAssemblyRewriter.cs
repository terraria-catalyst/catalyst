using AsmResolver.DotNet;
using TeamCatalyst.Catalyst.Abstractions.Annotations;
using TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;

namespace TeamCatalyst.Catalyst.Build.AssemblyRewriting;

internal sealed class AnnotationsAssemblyRewriter : IAssemblyRewriter {
    private readonly IAnnotationsProvider[] annotationsProviders;

    public AnnotationsAssemblyRewriter(params IAnnotationsProvider[] annotationsProviders) {
        this.annotationsProviders = annotationsProviders;
    }

    bool IAssemblyRewriter.ProcessAssembly(AssemblyDefinition asm) {
        var modified = false;

        foreach (var provider in annotationsProviders) {
            var annotations = provider.GetAnnotationsForAssembly(asm.Name!);
            if (annotations is null)
                continue;

            modified = true;
        }

        return modified;
    }
}
