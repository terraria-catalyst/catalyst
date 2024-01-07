using System;
using System.Collections.Generic;

namespace TeamCatalyst.Catalyst.Abstractions.Annotations;

public sealed class AnnotatedAssembly {
    public string AssemblyName { get; }

    public List<IAssemblyAnnotation> AssemblyAnnotations { get; } = new();

    public List<IModuleAnnotation> ModuleAnnotations { get; } = new();

    public Dictionary<string, AnnotatedClass> Classes { get; } = new();

    public Dictionary<string, AnnotatedEnum> Enums { get; } = new();

    public Dictionary<string, AnnotatedInterface> Interfaces { get; } = new();

    public Dictionary<string, AnnotatedStruct> Structs { get; } = new();

    public Dictionary<string, AnnotatedDelegate> Delegates { get; } = new();

    public AnnotatedAssembly(string assemblyName) {
        AssemblyName = assemblyName;
    }

    public AnnotatedAssembly WithAssemblyAnnotation(IAssemblyAnnotation annotation) {
        AssemblyAnnotations.Add(annotation);
        return this;
    }

    public AnnotatedAssembly WithModuleAnnotation(IModuleAnnotation annotation) {
        ModuleAnnotations.Add(annotation);
        return this;
    }

    public AnnotatedAssembly AnnotateClass(string fullName, Action<AnnotatedClass> action) {
        if (!Classes.TryGetValue(fullName, out var annotatedClass)) {
            annotatedClass = new AnnotatedClass(fullName);
            Classes.Add(fullName, annotatedClass);
        }

        action(annotatedClass);
        return this;
    }

    public AnnotatedAssembly AnnotateEnum(string fullName, Action<AnnotatedEnum> action) {
        if (!Enums.TryGetValue(fullName, out var annotatedEnum)) {
            annotatedEnum = new AnnotatedEnum(fullName);
            Enums.Add(fullName, annotatedEnum);
        }

        action(annotatedEnum);
        return this;
    }

    public AnnotatedAssembly AnnotateInterface(string fullName, Action<AnnotatedInterface> action) {
        if (!Interfaces.TryGetValue(fullName, out var annotatedInterface)) {
            annotatedInterface = new AnnotatedInterface(fullName);
            Interfaces.Add(fullName, annotatedInterface);
        }

        action(annotatedInterface);
        return this;
    }

    public AnnotatedAssembly AnnotateStruct(string fullName, Action<AnnotatedStruct> action) {
        if (!Structs.TryGetValue(fullName, out var annotatedStruct)) {
            annotatedStruct = new AnnotatedStruct(fullName);
            Structs.Add(fullName, annotatedStruct);
        }

        action(annotatedStruct);
        return this;
    }

    public AnnotatedAssembly AnnotateDelegate(string fullName, Action<AnnotatedDelegate> action) {
        if (!Delegates.TryGetValue(fullName, out var annotatedDelegate)) {
            annotatedDelegate = new AnnotatedDelegate(fullName);
            Delegates.Add(fullName, annotatedDelegate);
        }

        action(annotatedDelegate);
        return this;
    }
}

public abstract class AnnotatedType {
    public string FullName { get; }

    public Dictionary<string, AnnotatedField> Fields { get; } = new();

    public Dictionary<string, AnnotatedProperty> Properties { get; } = new();

    public Dictionary<string, AnnotatedEvent> Events { get; } = new();

    public Dictionary<string, AnnotatedMethod> Methods { get; } = new();

    public Dictionary<string, AnnotatedGenericParameter> GenericParameters { get; } = new();

    protected AnnotatedType(string fullName) {
        FullName = fullName;
    }

    public AnnotatedType AnnotateField(string name, Action<AnnotatedField> action) {
        if (!Fields.TryGetValue(name, out var annotatedField)) {
            annotatedField = new AnnotatedField(name);
            Fields.Add(name, annotatedField);
        }

        action(annotatedField);
        return this;
    }

    public AnnotatedType AnnotateProperty(string name, Action<AnnotatedProperty> action) {
        if (!Properties.TryGetValue(name, out var annotatedProperty)) {
            annotatedProperty = new AnnotatedProperty(name);
            Properties.Add(name, annotatedProperty);
        }

        action(annotatedProperty);
        return this;
    }

    public AnnotatedType AnnotateEvent(string name, Action<AnnotatedEvent> action) {
        if (!Events.TryGetValue(name, out var annotatedEvent)) {
            annotatedEvent = new AnnotatedEvent(name);
            Events.Add(name, annotatedEvent);
        }

        action(annotatedEvent);
        return this;
    }

    public AnnotatedType AnnotateMethod(string name, Action<AnnotatedMethod> action) {
        if (!Methods.TryGetValue(name, out var annotatedMethod)) {
            annotatedMethod = new AnnotatedMethod(name);
            Methods.Add(name, annotatedMethod);
        }

        action(annotatedMethod);
        return this;
    }

    public AnnotatedType AnnotateGenericParameter(string name, Action<AnnotatedGenericParameter> action) {
        if (!GenericParameters.TryGetValue(name, out var annotatedGenericParameter)) {
            annotatedGenericParameter = new AnnotatedGenericParameter(name);
            GenericParameters.Add(name, annotatedGenericParameter);
        }

        action(annotatedGenericParameter);
        return this;
    }
}

public sealed class AnnotatedClass : AnnotatedType {
    public List<IClassAnnotation> Annotations { get; } = new();

    public AnnotatedClass(string fullName) : base(fullName) { }
}

public sealed class AnnotatedEnum : AnnotatedType {
    public List<IEnumAnnotation> Annotations { get; } = new();

    public AnnotatedEnum(string name) : base(name) { }

    public AnnotatedEnum WithAnnotation(IEnumAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}

public sealed class AnnotatedInterface : AnnotatedType {
    public List<IInterfaceAnnotation> Annotations { get; } = new();

    public AnnotatedInterface(string name) : base(name) { }

    public AnnotatedInterface WithAnnotation(IInterfaceAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}

public sealed class AnnotatedStruct : AnnotatedType {
    public List<IStructAnnotation> Annotations { get; } = new();

    public AnnotatedStruct(string name) : base(name) { }

    public AnnotatedStruct WithAnnotation(IStructAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}

public sealed class AnnotatedDelegate : AnnotatedType {
    public List<IDelegateAnnotation> Annotations { get; } = new();

    public Dictionary<string, AnnotatedParameter> Parameters { get; } = new();

    public AnnotatedReturnValue ReturnValue { get; } = new();

    public AnnotatedDelegate(string name) : base(name) { }

    public AnnotatedDelegate WithAnnotation(IDelegateAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }

    public AnnotatedDelegate AnnotateParameter(string name, Action<AnnotatedParameter> action) {
        if (!Parameters.TryGetValue(name, out var annotatedParameter)) {
            annotatedParameter = new AnnotatedParameter(name);
            Parameters.Add(name, annotatedParameter);
        }

        action(annotatedParameter);
        return this;
    }

    public AnnotatedDelegate AnnotateReturnValue(Action<AnnotatedReturnValue> action) {
        action(ReturnValue);
        return this;
    }
}

public sealed class AnnotatedField {
    public string Name { get; }

    public List<IFieldAnnotation> Annotations { get; } = new();

    public AnnotatedField(string name) {
        Name = name;
    }

    public AnnotatedField WithAnnotation(IFieldAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}

public sealed class AnnotatedEvent {
    public string Name { get; }

    public List<IEventAnnotation> Annotations { get; } = new();

    public AnnotatedEvent(string name) {
        Name = name;
    }

    public AnnotatedEvent WithAnnotation(IEventAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}

public sealed class AnnotatedProperty {
    public string Name { get; }

    public List<IPropertyAnnotation> Annotations { get; } = new();

    public AnnotatedMethod? Getter { get; private set; }

    public AnnotatedMethod? Setter { get; private set; }

    public AnnotatedProperty(string name) {
        Name = name;
    }

    public AnnotatedProperty AnnotateGetter(Action<AnnotatedMethod> action) {
        Getter ??= new AnnotatedMethod("get_" + Name);
        action(Getter);
        return this;
    }

    public AnnotatedProperty AnnotateSetter(Action<AnnotatedMethod> action) {
        Setter ??= new AnnotatedMethod("set_" + Name);
        action(Setter);
        return this;
    }
}

public sealed class AnnotatedMethod {
    public string Name { get; }

    public List<IMethodAnnotation> Annotations { get; } = new();

    public Dictionary<string, AnnotatedParameter> Parameters { get; } = new();

    public AnnotatedReturnValue ReturnValue { get; } = new();

    public Dictionary<string, AnnotatedGenericParameter> GenericParameters { get; } = new();

    public AnnotatedMethod(string name) {
        Name = name;
    }

    public AnnotatedMethod WithAnnotation(IMethodAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }

    public AnnotatedMethod AnnotateParameter(string name, Action<AnnotatedParameter> action) {
        if (!Parameters.TryGetValue(name, out var annotatedParameter)) {
            annotatedParameter = new AnnotatedParameter(name);
            Parameters.Add(name, annotatedParameter);
        }

        action(annotatedParameter);
        return this;
    }

    public AnnotatedMethod AnnotateReturnValue(Action<AnnotatedReturnValue> action) {
        action(ReturnValue);
        return this;
    }

    public AnnotatedMethod AnnotateGenericParameter(string name, Action<AnnotatedGenericParameter> action) {
        if (!GenericParameters.TryGetValue(name, out var annotatedGenericParameter)) {
            annotatedGenericParameter = new AnnotatedGenericParameter(name);
            GenericParameters.Add(name, annotatedGenericParameter);
        }

        action(annotatedGenericParameter);
        return this;
    }
}

public sealed class AnnotatedParameter {
    public string Name { get; }

    public List<IParameterAnnotation> Annotations { get; } = new();

    public AnnotatedParameter(string name) {
        Name = name;
    }

    public AnnotatedParameter WithAnnotation(IParameterAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}

public sealed class AnnotatedReturnValue {
    public List<IReturnValueAnnotation> Annotations { get; } = new();

    public AnnotatedReturnValue WithAnnotation(IReturnValueAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}

public sealed class AnnotatedGenericParameter {
    public string Name { get; }

    public List<IGenericParameterAnnotation> Annotations { get; } = new();

    public AnnotatedGenericParameter(string name) {
        Name = name;
    }

    public AnnotatedGenericParameter WithAnnotation(IGenericParameterAnnotation annotation) {
        Annotations.Add(annotation);
        return this;
    }
}
