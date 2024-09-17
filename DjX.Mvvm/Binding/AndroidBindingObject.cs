#if ANDROID21_0_OR_GREATER
using Android.Views;
using AndroidX.RecyclerView.Widget;
using DjX.Mvvm.Platforms.Android;
using DjX.Mvvm.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;

namespace DjX.Mvvm.Binding;

public sealed class AndroidBindingObject : IDisposable
{
    private List<AndroidPropertyBindingSet> PropertyBindings { get; } = [];
    private List<AndroidEventBindingSet> EventBindings { get; } = [];
    private List<AndroidRecyclerViewCollectionBindingSet> CollectionBindings { get; } = [];

    public void RegisterPropertyBindingSet(ViewModelBase sourceObject, View targetObject, string bindingDeclaration)
        => RegisterBindingSet(sourceObject, targetObject, bindingDeclaration, this.RegisterPropertyBindingSet);

    public void RegisterEventBindingSet(ViewModelBase sourceObject, View targetObject, string bindingDeclaration)
        => RegisterBindingSet(sourceObject, targetObject, bindingDeclaration, this.RegisterEventBindingSet);

    public void RegisterCollectionBindingSet(
        ViewModelBase sourceObject,
        string sourceCollectionName,
        RecyclerView targetObject,
        int viewTemplateId)
    {
        var sourceCollection = sourceObject
            .GetType()
            .GetProperty(sourceCollectionName)?
            .GetValue(sourceObject);

        if (sourceCollection is null)
        {
            return;
        }

        var sourceCollectionType = sourceCollection.GetType();

        var sourceCollectionItemType = sourceCollectionType.GenericTypeArguments.SingleOrDefault(typeof(object));

        if (sourceCollectionType == typeof(ObservableCollection<>).MakeGenericType(sourceCollectionItemType))
        {
            var adapterType = typeof(BindableRecyclerViewAdapter<>).MakeGenericType(sourceCollectionItemType);
            var adapter = (RecyclerView.Adapter)Activator.CreateInstance(
                adapterType, [sourceCollection, targetObject.Context, viewTemplateId])!;

            targetObject.SetAdapter(adapter);

            this.CollectionBindings.Add(
                new AndroidRecyclerViewCollectionBindingSet((INotifyCollectionChanged)sourceCollection, adapter));
        }
    }

    public void Dispose()
    {
        this.PropertyBindings.ForEach(pb => pb.Dispose());
        this.EventBindings.ForEach(eb => eb.Dispose());
        this.CollectionBindings.ForEach(cb => cb.Dispose());
    }

    private static void RegisterBindingSet(
        ViewModelBase sourceObject,
        View targetObject,
        string bindingDeclaration,
        Action<ViewModelBase, View, ParsedBinding, PropertyInfo?> registerBindingSet)
    {
        var parsedBinding = ParseBindingDeclaration(bindingDeclaration);

        if (parsedBinding is null)
        {
            return;
        }

        var sourceProperty = sourceObject.GetType().GetProperty(parsedBinding.SourceMemberName);

        if (sourceProperty is null)
        {
            return;
        }

        registerBindingSet(sourceObject, targetObject, parsedBinding, sourceProperty);
    }

    private void RegisterPropertyBindingSet(
        ViewModelBase sourceObject,
        View targetObject,
        ParsedBinding parsedBinding,
        PropertyInfo? sourceProperty)
        => this.PropertyBindings.Add(
            new AndroidPropertyBindingSet(sourceObject, parsedBinding.SourceMemberName, targetObject, parsedBinding.TargetMemberName));

    private void RegisterEventBindingSet(
        ViewModelBase sourceObject,
        View targetObject,
        ParsedBinding parsedBinding,
        PropertyInfo? sourceProperty)
        => this.EventBindings.Add(
            new AndroidEventBindingSet(sourceObject, parsedBinding.SourceMemberName, targetObject, parsedBinding.TargetMemberName));

    private static ParsedBinding? ParseBindingDeclaration(string bindingDeclaration)
    {
        var bindingParts = bindingDeclaration.Split(' ');

        return bindingParts.Length != 2
            ? null
            : new ParsedBinding(bindingParts[1], bindingParts[0]);
    }
}

public record ParsedBinding(string SourceMemberName, string TargetMemberName);
#endif