#if ANDROID21_0_OR_GREATER
using Android.Text;
using Android.Views;
using DjX.Mvvm.Binding.Abstractions;
using DjX.Mvvm.Commands.Abstractions;
using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Binding;

public sealed class AndroidPropertyBindingSet : BindingSet<View>
{
    private readonly PropertyInfo? sourceProperty;
    private readonly PropertyInfo? targetProperty;
    private bool disposedValue;

    public AndroidPropertyBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
        this.sourceProperty = this.SourceObject.GetType().GetProperty(this.SourceMemberName);
        this.targetProperty = this.TargetObject.GetType().GetProperty(this.TargetMemberName);

        this.SourceObject.PropertyChanged += this.OnSourcePropertyChanged;

        if (this.TargetObject is EditText editText)
        {
            editText.TextChanged += this.EditText_TextChanged;
        }

        this.OnSourcePropertyChanged(this.SourceObject, new PropertyChangedEventArgs(sourceMemberName));
    }

    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == this.SourceMemberName
            && this.targetProperty is not null)
        {
            var sourceValue = this.sourceProperty?.GetValue(this.SourceObject);
            var currentTargetValue = this.targetProperty.GetValue(this.TargetObject);
            if (!Equals(currentTargetValue, sourceValue))
            {
                this.targetProperty.SetValue(this.TargetObject, sourceValue);
            }
        }
    }

    private void EditText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.Text is not null)
        {
            this.sourceProperty?.SetValue(this.SourceObject, string.Join("", e.Text));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.SourceObject.PropertyChanged -= this.OnSourcePropertyChanged;

                if (this.TargetObject is EditText editText)
                {
                    editText.TextChanged -= this.EditText_TextChanged;
                }
            }
            this.disposedValue = true;
        }
    }
}

public sealed class AndroidEventBindingSet : BindingSet<View>
{
    private readonly PropertyInfo? sourceProperty;
    private readonly EventInfo? targetEvent;
    private bool disposedValue;

    public AndroidEventBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
        this.sourceProperty = this.SourceObject.GetType().GetProperty(this.SourceMemberName);
        this.targetEvent = this.TargetObject.GetType().GetEvent(this.TargetMemberName);

        this.targetEvent?.AddEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);
    }

    public EventHandler? OnTargetEventRaisedDelegate => this.OnTargetEventRaised;

    private void OnTargetEventRaised(object? sender, EventArgs e)
    {
        var command = this.sourceProperty?.GetValue(this.SourceObject) as IDjXCommand;
        if (command?.CanExecute(null) ?? false)
        {
            command.Execute();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.targetEvent?.RemoveEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);
            }
            this.disposedValue = true;
        }
    }
}
#endif