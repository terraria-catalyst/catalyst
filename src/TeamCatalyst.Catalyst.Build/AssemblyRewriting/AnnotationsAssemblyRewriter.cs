using System.Linq;
using AsmResolver.DotNet;
using TeamCatalyst.Catalyst.Abstractions.Annotations;
using TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;

namespace TeamCatalyst.Catalyst.Build.AssemblyRewriting;

internal sealed class AnnotationsAssemblyRewriter : IAssemblyRewriter {
    private readonly IAnnotationsProvider[] annotationsProviders;

    public AnnotationsAssemblyRewriter(params IAnnotationsProvider[] annotationsProviders) {
        this.annotationsProviders = annotationsProviders;
    }

    bool IAssemblyRewriter.ProcessAssembly(AssemblyDefinition asm) {
        var modified = false;

        foreach (var provider in annotationsProviders) {
            var annotations = provider.GetAnnotationsForAssembly(asm.Name!);
            if (annotations is null)
                continue;

            var module = asm.ManifestModule!;

            foreach (var annotation in annotations.ModuleAnnotations)
                module.CustomAttributes.Add(annotation.GetCustomAttribute(module));

            foreach (var annotation in annotations.AssemblyAnnotations)
                asm.CustomAttributes.Add(annotation.GetCustomAttribute(module));

            var types = module.GetAllTypes().ToList();

            foreach (var (className, annotatedClass) in annotations.Classes) {
                var type = types.FirstOrDefault(x => x.FullName == className) ?? types.FirstOrDefault(x => x.Name == className);
                if (type is null)
                    continue;
            }

            foreach (var (className, annotatedClass) in annotations.Enums) {
                var type = types.FirstOrDefault(x => x.FullName == className) ?? types.FirstOrDefault(x => x.Name == className);
                if (type is null)
                    continue;
            }

            foreach (var (className, annotatedClass) in annotations.Interfaces) {
                var type = types.FirstOrDefault(x => x.FullName == className) ?? types.FirstOrDefault(x => x.Name == className);
                if (type is null)
                    continue;
            }

            foreach (var (className, annotatedClass) in annotations.Structs) {
                var type = types.FirstOrDefault(x => x.FullName == className) ?? types.FirstOrDefault(x => x.Name == className);
                if (type is null)
                    continue;
            }

            foreach (var (className, annotatedClass) in annotations.Delegates) {
                var type = types.FirstOrDefault(x => x.FullName == className) ?? types.FirstOrDefault(x => x.Name == className);
                if (type is null)
                    continue;
            }

            modified = true;
        }

        return modified;
    }
}
