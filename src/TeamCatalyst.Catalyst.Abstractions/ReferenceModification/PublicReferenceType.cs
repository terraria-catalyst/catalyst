using System.Collections.Generic;

namespace TeamCatalyst.Catalyst.Abstractions.ReferenceModification;

public sealed class PublicReferenceType {
    public string TypeName { get; }

    public bool PublicizeSelf { get; set; }

    public bool PublicizeAllMembers { get; set; }

    public List<string> Fields { get; } = new();

    public List<string> Properties { get; } = new();

    public List<string> Events { get; } = new();

    public List<string> Methods { get; } = new();

    public PublicReferenceType(string typeName, bool publicizeSelf, bool publicizeAllMembers) {
        TypeName = typeName;
        PublicizeSelf = publicizeSelf;
        PublicizeAllMembers = publicizeAllMembers;
    }
}
