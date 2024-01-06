using TeamCatalyst.Catalyst.Abstractions.ReferenceModification;

namespace TeamCatalyst.Catalyst.Abstractions.Engines;

public interface IEngine {
    PublicReferenceManifest ProcessAssemblyPublicizer(string filePath);
}
