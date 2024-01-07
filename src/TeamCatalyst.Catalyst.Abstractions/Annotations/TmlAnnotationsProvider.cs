namespace TeamCatalyst.Catalyst.Abstractions.Annotations;

public sealed class TmlAnnotationsProvider : IAnnotationsProvider {
    private static readonly AnnotatedAssembly tml = new AnnotatedAssembly("tModLoader")
        .AnnotateClass(
            "Terraria.AdvancedPopupRequest",
            type => {
                type.AnnotateField(
                    "Text",
                    field => {
                        field.WithAnnotation(new NotNullAnnotation());
                    }
                );
            }
        );

    public AnnotatedAssembly? GetAnnotationsForAssembly(string assemblyName) {
        return assemblyName != "tModLoader" ? null : tml;
    }
}
