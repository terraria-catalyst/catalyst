using System;
using System.IO;
using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using TeamCatalyst.Catalyst.Abstractions.Engines;
using TeamCatalyst.Catalyst.Abstractions.ReferenceModification;
using TeamCatalyst.Catalyst.Build.JavaScript.ReferenceModification;

namespace TeamCatalyst.Catalyst.Build.JavaScript;

public sealed class JavaScriptEngine : IEngine {
    public PublicReferenceManifest ProcessAssemblyPublicizer(string filePath) {
        using var engine = new Engine();

        engine.Modules.Add(
            "publicizer",
            builder => {
                builder.ExportFunction("createPublicizer", createPublicizer);
            }
        );

        engine.Modules.Add("publicizerModule", File.ReadAllText(filePath));
        var publicizerModule = engine.Modules.Import("publicizerModule");
        return ((JsReferenceManifest)((ObjectWrapper)publicizerModule.Get("publicizer").AsObject()).Target).ToManifest();

        JsValue createPublicizer(JsValue[] arguments) {
            var name = arguments[0].AsString();

            // ReSharper disable once AccessToDisposedClosure
            return JsValue.FromObject(engine, new JsReferenceManifest(name));
        }
    }
}
