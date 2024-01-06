using System.Runtime.CompilerServices;
using TeamCatalyst.Catalyst.Abstractions.Engines;
using TeamCatalyst.Catalyst.Build.JavaScript;
using TeamCatalyst.Catalyst.Build.Json;
using TeamCatalyst.Catalyst.Build.Lua;

namespace TeamCatalyst.Catalyst.Build;

internal static class EngineInitializer {
    [ModuleInitializer]
    internal static void Initialize() {
        EngineRegistry.RegisterEngineWithExtensions(new LuaEngine(), ".lua");
        EngineRegistry.RegisterEngineWithExtensions(new JsonEngine(), ".json", ".jsonc", ".json5");
        // TODO: What does Jint actually support?
        EngineRegistry.RegisterEngineWithExtensions(new JavaScriptEngine(), ".js", ".mjs", ".cjs");
    }
}
