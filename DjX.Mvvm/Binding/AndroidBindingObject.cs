#if ANDROID21_0_OR_GREATER
using Android.Text;
using Android.Views;
using DjX.Mvvm.Binding.Abstractions;
using DjX.Mvvm.Commands.Abstractions;
using DjX.Mvvm.ViewModels;
using System.ComponentModel;
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

    private void RegisterBindingSet(ViewModelBase sourceObject, View targetObject, string bindingDeclaration,
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
        if (sourceProperty?.PropertyType.GetInterface(nameof(IDjXCommand)) is not null)
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

public sealed class AndroidPropertyBindingSet : BindingSet<View>
{
    private bool disposedValue;

    public AndroidPropertyBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
        SourceObject.PropertyChanged += OnSourcePropertyChanged;

        if (TargetObject is EditText editText)
        {
            editText.TextChanged += EditText_TextChanged;
        }
    }

    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == SourceMemberName)
        {
            var sourceValue = SourceObject.GetType().GetProperty(SourceMemberName)?.GetValue(SourceObject);
            var targetProperty = TargetObject.GetType().GetProperty(TargetMemberName);
            var currentTargetValue = targetProperty?.GetValue(TargetObject);
            if (!Equals(currentTargetValue, sourceValue))
            {
                targetProperty?.SetValue(TargetObject, sourceValue);
            }
        }
    }

    private void EditText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.Text is not null)
        {
            SourceObject.GetType().GetProperty(SourceMemberName)?.SetValue(SourceObject, string.Join("", e.Text));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                SourceObject.PropertyChanged -= OnSourcePropertyChanged;

                if (TargetObject is EditText editText)
                {
                    editText.TextChanged -= EditText_TextChanged;
                }
            }
            disposedValue = true;
        }
    }
}

public sealed class AndroidEventBindingSet : BindingSet<View>
{
    private bool disposedValue;

    public AndroidEventBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
        TargetObject.GetType().GetEvent(TargetMemberName)?.AddEventHandler(TargetObject, OnTargetEventRaisedDelegate);
    }

    public EventHandler? OnTargetEventRaisedDelegate => OnTargetEventRaised;

    private void OnTargetEventRaised(object? sender, EventArgs e)
    {
        var command = SourceObject.GetType().GetProperty(SourceMemberName)?.GetValue(SourceObject) as IDjXCommand;
        if (command is not null && command.CanExecute(null))
        {
            command.Execute();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                TargetObject.GetType().GetEvent(TargetMemberName)?.RemoveEventHandler(TargetObject, OnTargetEventRaisedDelegate);
            }
            disposedValue = true;
        }
    }
}
#endif