#if ANDROID21_0_OR_GREATER
using Android.Text;
using Android.Views;
using DjX.Mvvm.Binding.Abstractions;
using System.ComponentModel;

namespace DjX.Mvvm.Binding;

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
}#endif