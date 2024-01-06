using System.Collections.Generic;

namespace TeamCatalyst.Catalyst.Abstractions.ReferenceModification;

public sealed class PublicReferenceManifest {
    public string Name { get; }

    public Dictionary<string, PublicReferenceRecord> Assemblies { get; } = new();

    public PublicReferenceManifest(string name) {
        Name = name;
    }
}
