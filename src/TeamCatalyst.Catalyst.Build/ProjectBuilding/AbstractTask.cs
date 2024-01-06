using System;
using Microsoft.Build.Utilities;

namespace TeamCatalyst.Catalyst.Build.ProjectBuilding;

public abstract class AbstractTask : Task {
    public sealed override bool Execute() {
        try {
            return Run();
        }
        catch (Exception e) {
            Log.LogError("Unhandled exception in task {0}", GetType().Name);
            Log.LogErrorFromException(e);
            return false;
        }
    }

    protected abstract bool Run();
}
