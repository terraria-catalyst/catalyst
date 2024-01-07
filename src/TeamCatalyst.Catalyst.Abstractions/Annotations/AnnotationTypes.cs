using System;
using System.Linq;
using AsmResolver.DotNet;
using JetBrains.Annotations;

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

public abstract class AbstractAnnotation<T> : IAnnotation {
    public virtual bool AllowMultiple => false;

    public virtual bool Inherited => true;

    public abstract CustomAttribute GetCustomAttribute(ModuleDefinition module);

    protected abstract ICustomAttributeType GetAttributeCtor(ModuleDefinition module);
}

public abstract class SingleAndEmptyConstructorAnnotation<T> : AbstractAnnotation<T> {
    public override CustomAttribute GetCustomAttribute(ModuleDefinition module) {
        return new CustomAttribute(GetAttributeCtor(module));
    }

    protected override ICustomAttributeType GetAttributeCtor(ModuleDefinition module) {
        var tempMod = ModuleDefinition.FromFile(typeof(T).Assembly.Location);
        var attrType = tempMod.TopLevelTypes.FirstOrDefault(x => x.FullName == typeof(T).FullName);
        if (attrType is null)
            throw new Exception("Could not find attribute type " + typeof(T).FullName);

        module.Assembly!.Modules.Add(module);
        return module.DefaultImporter.ImportMethod(attrType.Methods.Single(x => x.Name == ".ctor")).Resolve()!;
    }
}

public sealed class CanBeNullAnnotation : SingleAndEmptyConstructorAnnotation<CanBeNullAttribute>, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation, IEventAnnotation, IClassAnnotation, IInterfaceAnnotation, IGenericParameterAnnotation { }

public sealed class NotNullAnnotation : SingleAndEmptyConstructorAnnotation<NotNullAttribute>, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation, IEventAnnotation, IClassAnnotation, IInterfaceAnnotation, IGenericParameterAnnotation { }

public sealed class ItemNotNullAnnotation : SingleAndEmptyConstructorAnnotation<ItemNotNullAttribute>, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation { }

public sealed class ItemCanBeNullAnnotation : SingleAndEmptyConstructorAnnotation<ItemCanBeNullAttribute>, IMethodAnnotation, IParameterAnnotation, IPropertyAnnotation, IDelegateAnnotation, IFieldAnnotation { }
