using System.Collections.Generic;
using System.Security.Cryptography;

namespace TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;

public interface IAssemblyRewriter {
    AssemblyRewritingContext Context { get; }

    bool ProcessAssembly();

    IEnumerable<(string name, byte[] data)> GetAuxiliaryFiles();

    void Hash(ICryptoTransform hash);
}
