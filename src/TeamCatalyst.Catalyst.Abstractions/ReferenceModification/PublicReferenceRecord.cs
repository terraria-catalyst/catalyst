using System.Collections.Generic;

namespace TeamCatalyst.Catalyst.Abstractions.ReferenceModification;

public sealed class PublicReferenceRecord {
    public string AssemblyName { get; }

    public bool PublicizeAllMembersAndTypes { get; set; }

    public bool AllowVirtualMembers { get; set; }

    public List<PublicReferenceType> Types { get; } = new();

    public PublicReferenceRecord(string assemblyName, bool publicizeAllMembersAndTypes, bool allowVirtualMembers) {
        AssemblyName = assemblyName;
        PublicizeAllMembersAndTypes = publicizeAllMembersAndTypes;
        AllowVirtualMembers = allowVirtualMembers;
    }
}
