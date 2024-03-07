using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AsmResolver.DotNet;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;
using TeamCatalyst.Catalyst.Abstractions.Engines;
using TeamCatalyst.Catalyst.Abstractions.Hashing;
using TeamCatalyst.Catalyst.Abstractions.ReferenceModification;
using TeamCatalyst.Catalyst.Build.AssemblyRewriting;

namespace TeamCatalyst.Catalyst.Build.ProjectBuilding.Tasks.ReferenceModification;

public sealed class ModifyReferencesTask : AbstractTask {
    public ITaskItem[] ReferencePaths { get; set; } = null!;

    public ITaskItem[] Publicizers { get; set; } = null!;

    [Required]
    public string OutputDirectory { get; set; } = null!;

    [Output]
    public ITaskItem[]? ReferencesToAdd { get; set; }

    [Output]
    public ITaskItem[]? ReferencesToRemove { get; set; }

    protected override bool Run() {
        if (Publicizers is null || Publicizers.Length == 0)
            return true;

        Directory.CreateDirectory(OutputDirectory);
        ReferencesToAdd ??= Array.Empty<ITaskItem>();
        ReferencesToRemove ??= Array.Empty<ITaskItem>();

        var publicManifest = HandlePublicizers();

        var toAdd = new List<ITaskItem>();
        var toRemove = new List<ITaskItem>();

        foreach (var reference in ReferencePaths) {
            var assemblyName = reference.GetMetadata("Filename");
            var assemblyPath = reference.GetMetadata("FullPath");
            var assemblyBytes = File.ReadAllBytes(assemblyPath);
            var assemblyDefinition = AssemblyDefinition.FromBytes(assemblyBytes);

            var context = new AssemblyRewritingContext(assemblyName, assemblyPath, assemblyDefinition, assemblyDefinition.ManifestModule!);
            var rewriters = new IAssemblyRewriter[] {
                new SummaryProviderAssemblyRewriter(context),
                new PublicizerAssemblyRewriter(context, publicManifest),
                new AnnotationProviderAssemblyRewriter(context),
            };

            var hash = Hasher.ComputeHash(assemblyBytes, rewriters);
            var outputDir = Path.Combine(OutputDirectory, $"{assemblyName}-{hash}");
            var outputPath = Path.Combine(outputDir, $"{assemblyName}.dll");
            Directory.CreateDirectory(outputDir);

            if (File.Exists(outputPath)) {
                Log.LogMessage("Skipping reference '{0}' because it already exists in the output directory", assemblyName);
                goto ModifyReferences;
            }

            if (!RewriteAssembly(rewriters))
                continue;

            Log.LogMessage("Writing modified reference '{0}' to '{1}'", assemblyName, outputPath);
            assemblyDefinition.Write(outputPath);

            Log.LogMessage("Writing auxiliary files");
            foreach (var (name, data) in rewriters.SelectMany(x => x.GetAuxiliaryFiles()))
                File.WriteAllBytes(Path.Combine(outputDir, name), data);

            ModifyReferences:
            var newRef = new TaskItem(outputPath);
            reference.CopyMetadataTo(newRef);
            toRemove.Add(reference);
            toAdd.Add(newRef);
        }

        ReferencesToAdd = toAdd.ToArray();
        ReferencesToRemove = toRemove.ToArray();
        return true;
    }

    private PublicReferenceManifest HandlePublicizers() {
        var publicizerFiles = Publicizers.Select(x => x.ToString()).ToArray();
        Log.LogMessage("Publicizer running with inputs: {0}", string.Join(", ", publicizerFiles));

        var manifests = new List<PublicReferenceManifest>();

        foreach (var publicizerFile in publicizerFiles) {
            if (!File.Exists(publicizerFile)) {
                Log.LogWarning("Publicizer file '{0}' does not exist", publicizerFile);
                continue;
            }

            if (!EngineRegistry.TryGetEngineFromExtension(Path.GetExtension(publicizerFile), out var engine)) {
                Log.LogWarning("Publicizer file '{0}' could not be processed because it does not have a supported extension!");
                continue;
            }

            try {
                manifests.Add(engine!.ProcessAssemblyPublicizer(publicizerFile));
            }
            catch (Exception e) {
                Log.LogWarning("Failed to run publicizer file '{0}': {1}", publicizerFile, e.Message);
            }
        }

        Log.LogMessage("Parsed into {0} manifests", manifests.Count);
        return CombineManifests(manifests);
    }

    private static PublicReferenceManifest CombineManifests(List<PublicReferenceManifest> manifests) {
        var combined = new PublicReferenceManifest("Combined");

        foreach (var manifest in manifests) {
            foreach (var asm in manifest.Assemblies.Values) {
                var asmName = asm.AssemblyName;
                if (asmName.EndsWith(".dll"))
                    asmName = asmName[..^".dll".Length];

                if (!combined.Assemblies.TryGetValue(asmName, out var combinedAsm))
                    combined.Assemblies[asmName] = combinedAsm = new PublicReferenceRecord(asmName, false, false);

                if (asm.PublicizeAllMembersAndTypes)
                    combinedAsm.PublicizeAllMembersAndTypes = true;

                if (asm.AllowVirtualMembers)
                    combinedAsm.AllowVirtualMembers = true;

                foreach (var type in asm.Types) {
                    var combinedType = combinedAsm.Types.FirstOrDefault(x => x.TypeName == type.TypeName);
                    if (combinedType is null)
                        combinedAsm.Types.Add(combinedType = new PublicReferenceType(type.TypeName, false, false));

                    if (type.PublicizeSelf)
                        combinedType.PublicizeSelf = true;

                    if (type.PublicizeAllMembers)
                        combinedType.PublicizeAllMembers = true;

                    combinedType.Fields.AddRange(type.Fields);
                    combinedType.Properties.AddRange(type.Properties);
                    combinedType.Events.AddRange(type.Events);
                    combinedType.Methods.AddRange(type.Methods);
                }
            }
        }

        return combined;
    }

    private static bool RewriteAssembly(params IAssemblyRewriter[] rewriters) {
        return rewriters.Aggregate(false, (current, rewriter) => current | rewriter.ProcessAssembly());
    }
}
