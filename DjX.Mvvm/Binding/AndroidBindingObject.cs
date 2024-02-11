#if ANDROID21_0_OR_GREATER
using Android.Views;
using DjX.Mvvm.Commands.Abstractions;
using DjX.Mvvm.ViewModels;
using System.Reflection;

namespace DjX.Mvvm.Binding;

public sealed class AndroidBindingObject : IDisposable
{
    private List<AndroidPropertyBindingSet> PropertyBindings { get; } = [];
    private List<AndroidEventBindingSet> EventBindings { get; } = [];

    public void RegisterPropertyBindingSet(ViewModelBase sourceObject, View targetObject, string bindingDeclaration) =>
        RegisterBindingSet(sourceObject, targetObject, bindingDeclaration, RegisterPropertyBindingSet);

    public void RegisterEventBindingSet(ViewModelBase sourceObject, View targetObject, string bindingDeclaration) =>
        RegisterBindingSet(sourceObject, targetObject, bindingDeclaration, RegisterEventBindingSet);

    public void Dispose()
    {
        PropertyBindings.ForEach(pb => pb.Dispose());
        EventBindings.ForEach(eb => eb.Dispose());
    }

    private static void RegisterBindingSet(ViewModelBase sourceObject, View targetObject, string bindingDeclaration,
        Action<ViewModelBase, View, ParsedBinding, PropertyInfo?> registerBindingSet)
    {
        var parsedBinding = ParseBindingDeclaration(bindingDeclaration);

        if (parsedBinding is not null)
        {
            var sourceProperty = sourceObject.GetType().GetProperty(parsedBinding.SourceMemberName);
            registerBindingSet(sourceObject, targetObject, parsedBinding, sourceProperty);
        }
    }

    private void RegisterPropertyBindingSet(ViewModelBase sourceObject, View targetObject, ParsedBinding parsedBinding, PropertyInfo? sourceProperty)
    {
        if (sourceProperty is not null)
        {
            var propertyBinding = new AndroidPropertyBindingSet(sourceObject, parsedBinding.SourceMemberName, targetObject, parsedBinding.TargetMemberName);
            PropertyBindings.Add(propertyBinding);
        }
    }

    private void RegisterEventBindingSet(ViewModelBase sourceObject, View targetObject, ParsedBinding parsedBinding, PropertyInfo? sourceProperty)
    {
        if (sourceProperty is not null)
        {
            var eventBinding = new AndroidEventBindingSet(sourceObject, parsedBinding.SourceMemberName, targetObject, parsedBinding.TargetMemberName);
            EventBindings.Add(eventBinding);
        }
    }

    private static ParsedBinding? ParseBindingDeclaration(string bindingDeclaration)
    {
        var bindingParts = bindingDeclaration.Split(' ');

        if (bindingParts.Length != 2)
        {
            return null;
        }

        return new ParsedBinding(bindingParts[1], bindingParts[0]);
    }
}

public record ParsedBinding(string SourceMemberName, string TargetMemberName);
#endif