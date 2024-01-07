using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;
using TeamCatalyst.Catalyst.Abstractions.Hashing;

namespace TeamCatalyst.Catalyst.Build.AssemblyRewriting;

public sealed class SummaryProviderAssemblyRewriter : IAssemblyRewriter {
    public AssemblyRewritingContext Context { get; }

    private readonly string? summaryPath;

    public SummaryProviderAssemblyRewriter(AssemblyRewritingContext context) {
        Context = context;

        var documentationPath = Path.Combine(Path.GetDirectoryName(Context.AssemblyPath)!, Context.AssemblyName + ".xml");
        if (File.Exists(documentationPath))
            summaryPath = documentationPath;
    }

    bool IAssemblyRewriter.ProcessAssembly() {
        return false;
    }

    IEnumerable<(string name, byte[] data)> IAssemblyRewriter.GetAuxiliaryFiles() {
        if (summaryPath is null)
            yield break;

        yield return (Path.GetFileName(summaryPath), File.ReadAllBytes(summaryPath));
    }

    void IAssemblyRewriter.Hash(ICryptoTransform hash) {
        hash.HashBoolean(summaryPath is not null);
    }
}
