using System.IO;

namespace TeamCatalyst.Catalyst.Annotations;

public static class AnnotationsProvider {
    public static bool TryGetAnnotationsForAssembly(string assemblyName, out string? annotations) {
        var asm = typeof(AnnotationsProvider).Assembly;
        var resourceName = $"{asm.GetName().Name}.{assemblyName}.Attributes.xml";
        try {
            using var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null) {
                annotations = null;
                return false;
            }

            using var reader = new StreamReader(stream);
            annotations = reader.ReadToEnd();
            return true;
        }
        catch {
            annotations = null;
            return false;
        }
    }
}
