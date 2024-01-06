using System.Collections.Generic;
using JetBrains.Annotations;

namespace TeamCatalyst.Catalyst.Build.JavaScript.ReferenceModification;

internal sealed class JsReferenceType {
    public string FullName { get; }

    public bool PublicizeThisType { get; set; }

    public bool PublicizeAllMembers { get; set; }

    public List<string> FieldsToPublicize { get; } = new();

    public List<string> PropertiesToPublicize { get; } = new();

    public List<string> EventsToPublicize { get; } = new();

    public List<string> Methods { get; } = new();

    public JsReferenceType(string fullName) {
        FullName = fullName;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceType PublicizeType() {
        PublicizeThisType = true;
        return this;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceType PublicizeMembers() {
        PublicizeAllMembers = true;
        return this;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceType PublicizeField(string name) {
        FieldsToPublicize.Add(name);
        return this;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceType PublicizeProperty(string name) {
        PropertiesToPublicize.Add(name);
        return this;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceType PublicizeEvent(string name) {
        EventsToPublicize.Add(name);
        return this;
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public JsReferenceType PublicizeMethod(string name) {
        Methods.Add(name);
        return this;
    }
}
