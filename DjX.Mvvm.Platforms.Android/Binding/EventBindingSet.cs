using Android.Views;
using DjX.Mvvm.Core.Binding.Abstractions;
using DjX.Mvvm.Core.Commands.Abstractions;
using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Platforms.Android.Binding;

public sealed class EventBindingSet : BindingSet<View, EventInfo>
{
    private bool disposedValue;

    public EventBindingSet(
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
