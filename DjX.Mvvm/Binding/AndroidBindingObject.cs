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

    public void RegisterDeclaredBindings(ViewModelBase sourceObject, View targetObject, string bindingDeclaration)
        => ParseBindingsDeclaration(bindingDeclaration)
            .ForEach(b => this.RegisterBindingSet(sourceObject, targetObject, b));

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
    private void RegisterBindingSet(ViewModelBase sourceObject, View targetObject, ParsedBinding parsedBinding)
    {
        var sourceProperty = sourceObject.GetType().GetProperty(parsedBinding.SourceMemberName);

        if (sourceProperty is null)
        {
            return;
        }

        var targetMembers = targetObject.GetType().GetMember(parsedBinding.TargetMemberName);

        foreach (var targetMember in targetMembers)
        {
            if (targetMember is PropertyInfo targetProperty)
            {
                this.PropertyBindings.Add(
                    new AndroidPropertyBindingSet(sourceObject, sourceProperty, targetObject, targetProperty));
            }

            if (targetMember is EventInfo targetEvent)
            {
                this.EventBindings.Add(
                    new AndroidEventBindingSet(sourceObject, sourceProperty, targetObject, targetEvent));
            }
        }
    }

    private static ParsedBinding? ParseSingleBindingDeclaration(string bindingDeclaration)
    {
        var bindingParts = bindingDeclaration.Split(' ');

        return bindingParts.Length != 2
            ? null
            : new ParsedBinding(bindingParts[1], bindingParts[0]);
    }

    private static List<ParsedBinding> ParseBindingsDeclaration(string bindingDeclaration)
        => bindingDeclaration
            .Split(';')
            .Select(ParseSingleBindingDeclaration)
            .Where(b => b is not null)
            .ToList()!;
}

public record ParsedBinding(string SourceMemberName, string TargetMemberName);
#endif