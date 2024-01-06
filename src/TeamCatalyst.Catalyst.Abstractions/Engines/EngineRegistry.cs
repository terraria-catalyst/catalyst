using System.Collections.Generic;

namespace TeamCatalyst.Catalyst.Abstractions.Engines;

public static class EngineRegistry {
    private static readonly Dictionary<string, IEngine> engines = new();

    public static bool TryGetEngineFromExtension(string extension, out IEngine? engine) {
        return engines.TryGetValue(extension, out engine);
    }

    public static void RegisterEngineWithExtensions(IEngine engine, params string[] extensions) {
        foreach (var extension in extensions)
            engines[extension] = engine;
    }
}
