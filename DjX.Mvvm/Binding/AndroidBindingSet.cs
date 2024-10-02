#if ANDROID21_0_OR_GREATER
using Android.Text;
using Android.Views;
using DjX.Mvvm.Binding.Abstractions;
using DjX.Mvvm.Commands.Abstractions;
using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Binding;

public sealed class AndroidPropertyBindingSet : BindingSet<View, PropertyInfo>
{
    private bool disposedValue;

    public AndroidPropertyBindingSet(
        INotifyPropertyChanged sourceObject,
        PropertyInfo sourceMemberInfo,
        View targetObject,
        PropertyInfo targetMemberName)
        : base(sourceObject, sourceMemberInfo, targetObject, targetMemberName)
    {
        this.SourceObject.PropertyChanged += this.OnSourcePropertyChanged;

        if (this.TargetObject is EditText editText)
        {
            editText.TextChanged += this.EditText_TextChanged;
        }

        this.OnSourcePropertyChanged(this.SourceObject, new PropertyChangedEventArgs(sourceMemberInfo.Name));
    }

    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == this.SourceMemberInfo.Name)
        {
            var sourceValue = this.SourceMemberInfo.GetValue(this.SourceObject);
            var currentTargetValue = this.TargetMemberInfo.GetValue(this.TargetObject);
            if (!Equals(currentTargetValue, sourceValue))
            {
                this.TargetMemberInfo.SetValue(this.TargetObject, sourceValue);
            }
        }
    }

    private void EditText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.Text is not null)
        {
            this.SourceMemberInfo.SetValue(this.SourceObject, string.Join("", e.Text));
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

public sealed class AndroidEventBindingSet : BindingSet<View, EventInfo>
{
    private bool disposedValue;

    public AndroidEventBindingSet(
        INotifyPropertyChanged sourceObject,
        PropertyInfo sourceMemberInfo,
        View targetObject,
        EventInfo targetMemberInfo)
        : base(sourceObject, sourceMemberInfo, targetObject, targetMemberInfo)
        => this.TargetMemberInfo.AddEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);

    public EventHandler? OnTargetEventRaisedDelegate => this.OnTargetEventRaised;

    private void OnTargetEventRaised(object? sender, EventArgs e)
    {
        var command = this.SourceMemberInfo.GetValue(this.SourceObject) as IDjXCommand;
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
                this.TargetMemberInfo.RemoveEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);
            }
            this.disposedValue = true;
        }
    }
}
#endif