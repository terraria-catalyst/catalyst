using System;
using System.Linq;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

namespace TeamCatalyst.Catalyst.Abstractions.Annotations;

public interface IAnnotation {
    bool AllowMultiple { get; } // false

    bool Inherited { get; } // true

    CustomAttribute GetCustomAttribute(ModuleDefinition module);
}

public interface IAssemblyAnnotation : IAnnotation { }

public interface IModuleAnnotation : IAnnotation { }

public interface IClassAnnotation : IAnnotation { }

public interface IStructAnnotation : IAnnotation { }

public interface IEnumAnnotation : IAnnotation { }

public interface IConstructorAnnotation : IAnnotation { }

public interface IMethodAnnotation : IAnnotation { }

public interface IPropertyAnnotation : IAnnotation { }

public interface IFieldAnnotation : IAnnotation { }

public interface IEventAnnotation : IAnnotation { }

public interface IInterfaceAnnotation : IAnnotation { }

public interface IParameterAnnotation : IAnnotation { }

public interface IDelegateAnnotation : IAnnotation { }

public interface IReturnValueAnnotation : IAnnotation { }

public interface IGenericParameterAnnotation : IAnnotation { }

public interface IAllAnnotation : IAssemblyAnnotation, IModuleAnnotation, IClassAnnotation, IStructAnnotation, IEnumAnnotation, IConstructorAnnotation, IMethodAnnotation, IPropertyAnnotation, IFieldAnnotation, IEventAnnotation, IInterfaceAnnotation, IParameterAnnotation, IDelegateAnnotation, IReturnValueAnnotation, IGenericParameterAnnotation { }

public abstract class AbstractAnnotation : IAnnotation {
    public virtual bool AllowMultiple => false;

    public virtual bool Inherited => true;

    protected abstract string AttributeFullName { get; }

    public abstract CustomAttribute GetCustomAttribute(ModuleDefinition module);

    protected virtual TypeDefinition GetOrCreateAttributeType(ModuleDefinition module) {
        var attrType = module.GetAllTypes().FirstOrDefault(x => x.FullName == AttributeFullName);
        if (attrType is not null)
            return attrType;

        AttributeTargets targets = 0;

        // ReSharper disable SuspiciousTypeConversion.Global
        if (this is IAllAnnotation) {
            targets = AttributeTargets.All;
        }
        else {
            if (this is IAssemblyAnnotation)
                targets |= AttributeTargets.Assembly;

            if (this is IModuleAnnotation)
                targets |= AttributeTargets.Module;

            if (this is IClassAnnotation)
                targets |= AttributeTargets.Class;

            if (this is IStructAnnotation)
                targets |= AttributeTargets.Struct;

            if (this is IEnumAnnotation)
                targets |= AttributeTargets.Enum;

            if (this is IConstructorAnnotation)
                targets |= AttributeTargets.Constructor;

            if (this is IMethodAnnotation)
                targets |= AttributeTargets.Method;

            if (this is IPropertyAnnotation)
                targets |= AttributeTargets.Property;

            if (this is IFieldAnnotation)
                targets |= AttributeTargets.Field;

            if (this is IEventAnnotation)
                targets |= AttributeTargets.Event;

            if (this is IInterfaceAnnotation)
                targets |= AttributeTargets.Interface;

            if (this is IParameterAnnotation)
                targets |= AttributeTargets.Parameter;

            if (this is IDelegateAnnotation)
                targets |= AttributeTargets.Delegate;

            if (this is IReturnValueAnnotation)
                targets |= AttributeTargets.ReturnValue;

            if (this is IGenericParameterAnnotation)
                targets |= AttributeTargets.GenericParameter;
        }
        // ReSharper restore SuspiciousTypeConversion.Global

        var attrTargetsRef = module.CorLibTypeFactory.CorLibScope.CreateTypeReference("System", "AttributeTargets");
        var attrUsageSig = MethodSignature.CreateInstance(module.CorLibTypeFactory.Void, attrTargetsRef.ToTypeSignature());
        var customSig = new CustomAttributeSignature(new CustomAttributeArgument(attrTargetsRef.ToTypeSignature(), targets));
        customSig.NamedArguments.Add(new CustomAttributeNamedArgument(CustomAttributeArgumentMemberType.Property, "AllowMultiple", module.CorLibTypeFactory.Boolean, new CustomAttributeArgument(module.CorLibTypeFactory.Boolean, AllowMultiple)));
        customSig.NamedArguments.Add(new CustomAttributeNamedArgument(CustomAttributeArgumentMemberType.Property, "Inherited", module.CorLibTypeFactory.Boolean, new CustomAttributeArgument(module.CorLibTypeFactory.Boolean, Inherited)));
        var attrUsage = new CustomAttribute(module.CorLibTypeFactory.CorLibScope.CreateTypeReference("System", "AttributeUsageAttribute").CreateMemberReference(".ctor", attrUsageSig), customSig);

        attrType = new TypeDefinition(AttributeFullName.Split('.').Last(), AttributeFullName, TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, module.CorLibTypeFactory.Object.Type);
        attrType.CustomAttributes.Add(attrUsage);
        module.TopLevelTypes.Add(attrType);

        var ctor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RuntimeSpecialName, MethodSignature.CreateStatic(module.CorLibTypeFactory.Void));
        attrType.Methods.Add(ctor);

        return attrType;
    }
}

public abstract class SingleAndEmptyConstructorAnnotation : AbstractAnnotation {
    public override CustomAttribute GetCustomAttribute(ModuleDefinition module) {
        var attrType = GetOrCreateAttributeType(module);
        var ctor = attrType.Methods.FirstOrDefault(x => x.Name == ".ctor" && x.Parameters.Count == 0);
        if (ctor is null)
            throw new Exception("Could not find constructor for attribute type " + attrType.FullName);

        return new CustomAttribute(ctor);
    }
}

public sealed class CanBeNullAnnotation : SingleAndEmptyConstructorAnnotation, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation, IEventAnnotation, IClassAnnotation, IInterfaceAnnotation, IGenericParameterAnnotation {
    protected override string AttributeFullName => "JetBrains.Annotations.CanBeNullAttribute";
}

public sealed class NotNullAnnotation : SingleAndEmptyConstructorAnnotation, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation, IEventAnnotation, IClassAnnotation, IInterfaceAnnotation, IGenericParameterAnnotation {
    protected override string AttributeFullName => "JetBrains.Annotations.NotNullAttribute";
}

public sealed class ItemNotNullAnnotation : SingleAndEmptyConstructorAnnotation, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation {
    protected override string AttributeFullName => "JetBrains.Annotations.ItemNotNullAttribute";
}

public sealed class ItemCanBeNullAnnotation : SingleAndEmptyConstructorAnnotation, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation {
    protected override string AttributeFullName => "JetBrains.Annotations.ItemCanBeNullAttribute";
}
