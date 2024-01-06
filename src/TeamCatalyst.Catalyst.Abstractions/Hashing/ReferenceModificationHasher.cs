using System.Security.Cryptography;
using TeamCatalyst.Catalyst.Abstractions.ReferenceModification;

namespace TeamCatalyst.Catalyst.Abstractions.Hashing;

public sealed class ReferenceModificationHasher : IHasher {
    public PublicReferenceManifest Manifest { get; }

    public ReferenceModificationHasher(PublicReferenceManifest manifest) {
        Manifest = manifest;
    }

    void IHasher.Hash(ICryptoTransform hash) {
        Hasher.HashString(hash, Manifest.Name);

        foreach (var asm in Manifest.Assemblies) {
            Hasher.HashString(hash, asm.Key);
            Hasher.HashString(hash, asm.Value.AssemblyName);
            Hasher.HashBoolean(hash, asm.Value.PublicizeAllMembersAndTypes);
            Hasher.HashBoolean(hash, asm.Value.AllowVirtualMembers);

            foreach (var type in asm.Value.Types) {
                Hasher.HashString(hash, type.TypeName);
                Hasher.HashBoolean(hash, type.PublicizeSelf);
                Hasher.HashBoolean(hash, type.PublicizeAllMembers);

                foreach (var field in type.Fields)
                    Hasher.HashString(hash, field);

                foreach (var property in type.Properties)
                    Hasher.HashString(hash, property);

                foreach (var @event in type.Events)
                    Hasher.HashString(hash, @event);

                foreach (var method in type.Methods)
                    Hasher.HashString(hash, method);
            }
        }
    }
}
