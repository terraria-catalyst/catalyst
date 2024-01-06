namespace TeamCatalyst.Catalyst.Abstractions.Annotations;

public sealed class TmlAnnotationsProvider : IAnnotationsProvider {
    private static readonly AnnotatedAssembly tml = new AnnotatedAssembly("tModLoader")
        .AnnotateClass(
            "Terraria.Main",
            type => {
                type.AnnotateMethod(
                    "SomeMethod",
                    method => {
                        method.WithAnnotation(null!).WithAnnotation(null!);
                    }
                );
            }
        )
        .AnnotateClass(
            "Terraria.Namespace.SomethingElse",
            type => {
                type.AnnotateGenericParameter("T1", t1 => t1.WithAnnotation(null!));
            }
        );

    public AnnotatedAssembly? GetAnnotationsForAssembly(string assemblyName) {
        if (assemblyName != "tModLoader")
            return null;

        return tml;
    }
}
