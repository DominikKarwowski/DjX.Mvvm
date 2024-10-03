using Android.Views;
using DjX.Mvvm.Core.Binding.Abstractions;
using DjX.Mvvm.Core.Commands.Abstractions;
using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Platforms.Android.Binding;

public sealed class EventBindingSet : BindingSet<View, EventInfo>
{
    private bool disposedValue;

    private readonly PropertyInfo? SourceCommandParameter;

    public EventBindingSet(
        View targetObject,
        EventInfo targetMemberInfo,
        INotifyPropertyChanged sourceObject,
        PropertyInfo sourceCommandPropertyInfo,
        PropertyInfo? sourceCommandParameterPropertyInfo)
        : base(targetObject, targetMemberInfo, sourceObject, sourceCommandPropertyInfo)
    {
        this.SourceCommandParameter = sourceCommandParameterPropertyInfo;
        this.TargetMemberInfo.AddEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);
    }

    public EventHandler? OnTargetEventRaisedDelegate => this.OnTargetEventRaised;

    private void OnTargetEventRaised(object? sender, EventArgs e)
    {
        var command = this.SourcePropertyInfo.GetValue(this.SourceObject) as IDjXCommand;
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
