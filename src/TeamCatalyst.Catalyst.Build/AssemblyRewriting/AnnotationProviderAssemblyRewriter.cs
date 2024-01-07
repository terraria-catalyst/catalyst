using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;
using TeamCatalyst.Catalyst.Abstractions.Hashing;
using TeamCatalyst.Catalyst.Annotations;

namespace TeamCatalyst.Catalyst.Build.AssemblyRewriting;

public sealed class AnnotationProviderAssemblyRewriter : IAssemblyRewriter {
    public AssemblyRewritingContext Context { get; }

    private readonly string? attributes;

    public AnnotationProviderAssemblyRewriter(AssemblyRewritingContext context) {
        Context = context;
        AnnotationsProvider.TryGetAnnotationsForAssembly(context.AssemblyName, out attributes);
    }

    bool IAssemblyRewriter.ProcessAssembly() {
        return attributes is not null;
    }

    IEnumerable<(string name, byte[] data)> IAssemblyRewriter.GetAuxiliaryFiles() {
        yield return ($"{Context.AssemblyName}.ExternalAnnotations.xml", Encoding.UTF8.GetBytes(attributes!));
    }

    void IAssemblyRewriter.Hash(ICryptoTransform hash) {
        hash.HashString(attributes ?? "");
    }
}
