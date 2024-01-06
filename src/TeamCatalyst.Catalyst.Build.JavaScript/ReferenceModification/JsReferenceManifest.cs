using System.Collections.Generic;
using JetBrains.Annotations;
using TeamCatalyst.Catalyst.Abstractions.ReferenceModification;

namespace TeamCatalyst.Catalyst.Build.JavaScript.ReferenceModification;

internal sealed class JsReferenceManifest {
    public string Name { get; }

    public List<JsReferenceRecord> Assemblies { get; } = new();

    public JsReferenceManifest(string name) {
        Name = name;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceRecord CreateAssembly(string name) {
        var record = new JsReferenceRecord(name);
        Assemblies.Add(record);
        return record;
    }

    public PublicReferenceManifest ToManifest() {
        var manifest = new PublicReferenceManifest(Name);

        foreach (var asm in Assemblies) {
            var record = manifest.Assemblies[asm.AssemblyName] = new PublicReferenceRecord(asm.AssemblyName, asm.PublicizeAllMembersAndTypes, asm.AllowVirtualsForEntireAssembly);

            foreach (var type in asm.Types) {
                var pubType = new PublicReferenceType(type.FullName, type.PublicizeThisType, type.PublicizeAllMembers);
                record.Types.Add(pubType);

                pubType.Fields.AddRange(type.FieldsToPublicize);
                pubType.Properties.AddRange(type.PropertiesToPublicize);
                pubType.Events.AddRange(type.EventsToPublicize);
                pubType.Methods.AddRange(type.Methods);
            }
        }

        return manifest;
    }
}
