using Android.Views;
using AndroidX.RecyclerView.Widget;
using DjX.Mvvm.Core.ViewModels;
using DjX.Mvvm.Platforms.Android.Support;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;

namespace DjX.Mvvm.Platforms.Android.Binding;

public sealed class BindingObject : IDisposable
{
    private List<PropertyBindingSet> PropertyBindings { get; } = [];
    private List<EventBindingSet> EventBindings { get; } = [];
    private List<RecyclerViewCollectionBindingSet> CollectionBindings { get; } = [];

    public void RegisterDeclaredBindings(View targetObject, ViewModelBase sourceObject, string bindingDeclaration)
        => ParseBindingDeclaration(bindingDeclaration)
            .ForEach(b => this.RegisterBindingSet(targetObject, sourceObject, b));

    public void RegisterCollectionBindingSet(
        RecyclerView targetObject,
        ViewModelBase sourceObject,
        string sourceCollectionName,
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
                new RecyclerViewCollectionBindingSet((INotifyCollectionChanged)sourceCollection, adapter));
        }
    }

    public void Dispose()
    {
        this.PropertyBindings.ForEach(pb => pb.Dispose());
        this.EventBindings.ForEach(eb => eb.Dispose());
        this.CollectionBindings.ForEach(cb => cb.Dispose());
    }
    private void RegisterBindingSet(View targetObject, ViewModelBase sourceObject, BindingDeclaration parsedBinding)
    {
        var sourceProperty = sourceObject.GetType().GetProperty(parsedBinding.SourceMemberName);

        if (sourceProperty is null)
        {
            return;
        }

        var targetMembers = targetObject.GetType().GetMember(parsedBinding.TargetMemberName);

        if (targetMembers.Length != 1)
        {
            return;
        }

        if (targetMembers[0] is PropertyInfo targetProperty)
        {
            this.PropertyBindings.Add(
                new PropertyBindingSet(targetObject, targetProperty, sourceObject, sourceProperty));
        }

        if (targetMembers[0] is EventInfo targetEvent)
        {
            var sourceCommandParamProperty = parsedBinding.SourceMemberParameterName is null
                ? null
                : sourceObject.GetType().GetProperty(parsedBinding.SourceMemberParameterName); ;

            this.EventBindings.Add(
                new EventBindingSet(targetObject, targetEvent, sourceObject, sourceProperty, sourceCommandParamProperty));
        }
    }

    private static BindingDeclaration? ParseSingleBindingDeclaration(string singleBindingDeclaration)
    {
        var bindingParts = singleBindingDeclaration.Split(' ');

        if (bindingParts.Length != 2)
        {
            return null;
        }

        var sourceBindingParts = bindingParts[1].Split('*');

        return sourceBindingParts.Length switch
        {
            2 => new BindingDeclaration(bindingParts[0], sourceBindingParts[0], sourceBindingParts[1]),
            1 => new BindingDeclaration(bindingParts[0], sourceBindingParts[0]),
            _ => null,
        };
    }

    private static List<BindingDeclaration> ParseBindingDeclaration(string bindingDeclaration)
        => bindingDeclaration
            .Split(';')
            .Select(ParseSingleBindingDeclaration)
            .Where(b => b is not null)
            .ToList()!;
}

public record BindingDeclaration(string TargetMemberName, string SourceMemberName, string? SourceMemberParameterName = null);
