using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;
using TeamCatalyst.Catalyst.Abstractions.Hashing;
using TeamCatalyst.Catalyst.Abstractions.ReferenceModification;

namespace TeamCatalyst.Catalyst.Build.AssemblyRewriting;

internal sealed class PublicizerAssemblyRewriter : IAssemblyRewriter {
    public PublicReferenceManifest Manifest { get; }

    public AssemblyRewritingContext Context { get; }

    public PublicizerAssemblyRewriter(AssemblyRewritingContext context, PublicReferenceManifest manifest) {
        Context = context;
        Manifest = manifest;
    }

    bool IAssemblyRewriter.ProcessAssembly() {
        foreach (var manifestAssembly in Manifest.Assemblies) {
            if (manifestAssembly.Key != Context.Assembly.Name)
                continue;

            var publicizeAll = manifestAssembly.Value.PublicizeAllMembersAndTypes;
            var includeVirtuals = manifestAssembly.Value.AllowVirtualMembers;

            var types = Context.Assembly.Modules.SelectMany(x => x.GetAllTypes()).ToList();

            if (publicizeAll)
                PublicizeAllTypesAndMembers(types, includeVirtuals);
            else {
                foreach (var manifestType in manifestAssembly.Value.Types) {
                    // Fall back to just checking the type name minus the
                    // namespace, which is iffy but user-friendly.
                    var type = types.FirstOrDefault(x => x.FullName == manifestType.TypeName) ?? types.FirstOrDefault(x => x.Name == manifestType.TypeName);

                    // TODO: Log?
                    if (type is null)
                        continue;

                    PublicizeType(type, includeVirtuals, manifestType.PublicizeAllMembers, manifestType.PublicizeSelf);

                    if (manifestType.PublicizeAllMembers)
                        continue;

                    foreach (var requestedField in manifestType.Fields) {
                        var field = type.Fields.FirstOrDefault(x => x.Name == requestedField);

                        // TODO: Log?
                        if (field is not null)
                            PublicizeField(field);
                    }

                    foreach (var requestedProperty in manifestType.Properties) {
                        var property = type.Properties.FirstOrDefault(x => x.Name == requestedProperty);

                        // TODO: Log?
                        if (property is not null)
                            PublicizeProperty(property, includeVirtuals);
                    }

                    foreach (var requestedEvent in manifestType.Events) {
                        var @event = type.Events.FirstOrDefault(x => x.Name == requestedEvent);

                        // TODO: Log?
                        if (@event is not null)
                            PublicizeEvent(@event, includeVirtuals);
                    }

                    foreach (var requestedMethod in manifestType.Methods) {
                        var fullSig = requestedMethod.Contains('(');

                        var methods = new List<MethodDefinition>();

                        // Check the full name and expect only one result.
                        if (fullSig) {
                            methods.Add(type.Methods.FirstOrDefault(x => x.FullName == requestedMethod));
                        }
                        else {
                            // Otherwise expect multiple results to account for
                            // overloads.
                            methods.AddRange(type.Methods.Where(x => x.Name == requestedMethod));
                        }

                        // TODO: Log when methods.Length == 0?

                        foreach (var method in methods)
                            PublicizeMethod(method, includeVirtuals);
                    }
                }
            }

            return true;
        }

        return false;
    }

    IEnumerable<(string name, byte[] data)> IAssemblyRewriter.GetAuxiliaryFiles() {
        yield break;
    }

    void IAssemblyRewriter.Hash(ICryptoTransform hash) {
        hash.HashString( Manifest.Name);

        foreach (var asm in Manifest.Assemblies) {
            hash.HashString( asm.Key);
            hash.HashString( asm.Value.AssemblyName);
            hash.HashBoolean( asm.Value.PublicizeAllMembersAndTypes);
            hash.HashBoolean( asm.Value.AllowVirtualMembers);

            foreach (var type in asm.Value.Types) {
                hash.HashString( type.TypeName);
                hash.HashBoolean( type.PublicizeSelf);
                hash.HashBoolean( type.PublicizeAllMembers);

                foreach (var field in type.Fields)
                    hash.HashString( field);

                foreach (var property in type.Properties)
                    hash.HashString( property);

                foreach (var @event in type.Events)
                    hash.HashString( @event);

                foreach (var method in type.Methods)
                    hash.HashString( method);
            }
        }
    }

    private static void PublicizeAllTypesAndMembers(List<TypeDefinition> types, bool includeVirtuals) {
        foreach (var type in types)
            PublicizeType(type, includeVirtuals, true, true);
    }

    private static void PublicizeType(TypeDefinition typeDefinition, bool includeVirtuals, bool publicizeAllMembers, bool publicizeSelf) {
        if (publicizeSelf) {
            typeDefinition.Attributes &= ~TypeAttributes.VisibilityMask;
            typeDefinition.Attributes |= typeDefinition.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;
        }

        if (!publicizeAllMembers)
            return;

        foreach (var field in typeDefinition.Fields) {
            // Filter out fields that share names with events.
            if (typeDefinition.Events.Any(x => x.Name == field.Name))
                continue;

            PublicizeField(field);
        }

        foreach (var property in typeDefinition.Properties)
            PublicizeProperty(property, includeVirtuals);

        foreach (var @event in typeDefinition.Events)
            PublicizeEvent(@event, includeVirtuals);

        foreach (var method in typeDefinition.Methods)
            PublicizeMethod(method, includeVirtuals);
    }

    private static void PublicizeField(FieldDefinition fieldDefinition) {
        fieldDefinition.Attributes &= ~FieldAttributes.FieldAccessMask;
        fieldDefinition.Attributes |= FieldAttributes.Public;
    }

    private static void PublicizeProperty(PropertyDefinition propertyDefinition, bool includeVirtuals) {
        if (propertyDefinition.GetMethod is not null)
            PublicizeMethod(propertyDefinition.GetMethod, includeVirtuals);

        if (propertyDefinition.SetMethod is not null)
            PublicizeMethod(propertyDefinition.SetMethod, includeVirtuals);
    }

    private static void PublicizeEvent(EventDefinition eventDefinition, bool includeVirtuals) {
        if (eventDefinition.AddMethod is not null)
            PublicizeMethod(eventDefinition.AddMethod, includeVirtuals);

        if (eventDefinition.RemoveMethod is not null)
            PublicizeMethod(eventDefinition.RemoveMethod, includeVirtuals);
    }

    private static void PublicizeMethod(MethodDefinition methodDefinition, bool includeVirtuals) {
        if (methodDefinition.IsVirtual && !includeVirtuals)
            return;

        methodDefinition.Attributes &= ~MethodAttributes.MemberAccessMask;
        methodDefinition.Attributes |= MethodAttributes.Public;
    }
}
