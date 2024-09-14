#if ANDROID21_0_OR_GREATER
using Android.Text;
using Android.Views;
using DjX.Mvvm.Binding.Abstractions;
using DjX.Mvvm.Commands.Abstractions;
using System.ComponentModel;

namespace DjX.Mvvm.Binding;

public sealed class AndroidPropertyBindingSet : BindingSet<View>
{
    private bool disposedValue;

    public AndroidPropertyBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
        this.SourceObject.PropertyChanged += this.OnSourcePropertyChanged;

        if (this.TargetObject is EditText editText)
        {
            editText.TextChanged += this.EditText_TextChanged;
        }
    }

    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == this.SourceMemberName)
        {
            var sourceValue = this.SourceObject.GetType().GetProperty(this.SourceMemberName)?.GetValue(this.SourceObject);
            var targetProperty = this.TargetObject.GetType().GetProperty(this.TargetMemberName);
            var currentTargetValue = targetProperty?.GetValue(this.TargetObject);
            if (!Equals(currentTargetValue, sourceValue))
            {
                targetProperty?.SetValue(this.TargetObject, sourceValue);
            }
        }
    }

    private void EditText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.Text is not null)
        {
            this.SourceObject.GetType().GetProperty(this.SourceMemberName)?.SetValue(this.SourceObject, string.Join("", e.Text));
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
    private bool disposedValue;

    public AndroidEventBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
        this.TargetObject.GetType().GetEvent(this.TargetMemberName)?.AddEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);
    }

    public EventHandler? OnTargetEventRaisedDelegate => this.OnTargetEventRaised;

    private void OnTargetEventRaised(object? sender, EventArgs e)
    {
        var command = this.SourceObject.GetType().GetProperty(this.SourceMemberName)?.GetValue(this.SourceObject) as IDjXCommand;
        if (command is not null && command.CanExecute(null))
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
                this.TargetObject.GetType().GetEvent(this.TargetMemberName)?.RemoveEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);
            }
            this.disposedValue = true;
        }
    }
}
#endif