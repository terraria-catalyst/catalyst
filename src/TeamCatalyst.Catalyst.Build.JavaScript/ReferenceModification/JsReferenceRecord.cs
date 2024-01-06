using System.Collections.Generic;
using JetBrains.Annotations;

namespace TeamCatalyst.Catalyst.Build.JavaScript.ReferenceModification;

internal sealed class JsReferenceRecord {
    public string AssemblyName { get; }

    public bool PublicizeAllMembersAndTypes { get; set; }

    public bool AllowVirtualsForEntireAssembly { get; set; }

    public List<JsReferenceType> Types { get; } = new();

    public JsReferenceRecord(string assemblyName) {
        AssemblyName = assemblyName;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceRecord AllowVirtuals() {
        AllowVirtualsForEntireAssembly = true;
        return this;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceRecord PublicizeAll() {
        PublicizeAllMembersAndTypes = true;
        return this;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceType CreateType(string name) {
        var type = new JsReferenceType(name);
        Types.Add(type);
        return type;
    }
}
