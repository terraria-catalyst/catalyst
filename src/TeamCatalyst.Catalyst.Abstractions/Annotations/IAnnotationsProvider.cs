namespace TeamCatalyst.Catalyst.Abstractions.Annotations;

public interface IAnnotationsProvider {
    AnnotatedAssembly? GetAnnotationsForAssembly(string assemblyName);
}
