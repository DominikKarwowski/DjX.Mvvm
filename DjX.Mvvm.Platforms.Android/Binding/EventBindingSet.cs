using Android.Views;
using DjX.Mvvm.Core.Binding.Abstractions;
using DjX.Mvvm.Core.Commands;
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

    private async void OnTargetEventRaised(object? sender, EventArgs e)
    {
        // TODO: refactor the below mosntrosity and also test async commands appropriately
        var command = this.SourcePropertyInfo.GetValue(this.SourceObject);

        if (command is null)
        {
            return;
        }

        var commandType = command.GetType();

        if (commandType == typeof(DelegateCommand))
        {
            if (((ICommandBase)command).CanExecute(null))
            {
                ((DelegateCommand)command).Execute();
            }
        }
        else if (commandType == typeof(AsyncDelegateCommand))
        {
            if (((ICommandBase)command).CanExecute(null))
            {
                await ((AsyncDelegateCommand)command).ExecuteAsync();
            }
        }
        else
        {
            if (!commandType.IsGenericType)
            {
                return;
            }

            var commandTypeGeneric = commandType.GetGenericTypeDefinition();
            var paramType = commandType.GetGenericArguments()[0];

            if (this.SourceCommandParameter is not null && paramType != this.SourceCommandParameter.PropertyType)
            {
                return;
            }

            var sourceCommandParameterValue = this.SourceCommandParameter?.GetValue(this.SourceObject);

            if (commandTypeGeneric == typeof(DelegateCommand<>))
            {
                if (((ICommandBase)command).CanExecute(this.SourceCommandParameter))
                {
                    var concreteCommandType = commandTypeGeneric.MakeGenericType([paramType]);
                    _ = concreteCommandType.GetMethod("Execute", [paramType])!
                        .Invoke(command, [sourceCommandParameterValue]);
                }
            }
            else if (commandTypeGeneric == typeof(AsyncDelegateCommand<>))
            {
                if (((ICommandBase)command).CanExecute(this.SourceCommandParameter))
                {
                    var concreteCommandType = commandTypeGeneric.MakeGenericType([paramType]);
                    var task = (Task)concreteCommandType.GetMethod("ExecuteAsync", [paramType])!
                        .Invoke(command, [sourceCommandParameterValue])!;
                    await task.ConfigureAwait(false);
                }
            }
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
