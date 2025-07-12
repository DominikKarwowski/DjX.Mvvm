using Android.Views;
using DjX.Mvvm.Core.Binding.Abstractions;
using DjX.Mvvm.Core.Commands;
using DjX.Mvvm.Core.Commands.Abstractions;
using DjX.Mvvm.Core.ViewModels;
using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Platforms.Android.Binding;

public class CollectionItemEventBindingSet : BindingSet<View, EventInfo>
{
    private bool disposedValue;

    private readonly ViewModelBase SourceCommandParameter;

    public CollectionItemEventBindingSet(
        View targetObject,
        EventInfo targetMemberInfo,
        INotifyPropertyChanged sourceObject,
        PropertyInfo sourceCommandPropertyInfo,
        ViewModelBase sourceCommandParameter)
        : base(targetObject, targetMemberInfo, sourceObject, sourceCommandPropertyInfo)
    {
        this.SourceCommandParameter = sourceCommandParameter;
        this.TargetMemberInfo.AddEventHandler(this.TargetObject, this.OnTargetEventRaisedDelegate);

    }

    // TODO: add support for a LongClick event, which requires generic EventHandler<T>
    public EventHandler? OnTargetEventRaisedDelegate => this.OnTargetEventRaised;

    private async void OnTargetEventRaised(object? sender, EventArgs e)
    {
        // TODO: refactor the below monstrosity and also test async commands appropriately
        // refer to EventBindingSet class
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
                // TODO: should I add ConfigureAwait(false) or leave it to the client code to decide?
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

            if (commandTypeGeneric == typeof(DelegateCommand<>))
            {
                if (((ICommandBase)command).CanExecute(this.SourceCommandParameter))
                {
                    var concreteCommandType = commandTypeGeneric.MakeGenericType([paramType]);
                    _ = concreteCommandType.GetMethod("Execute", [paramType])!
                        .Invoke(command, [this.SourceCommandParameter]);
                }
            }
            else if (commandTypeGeneric == typeof(AsyncDelegateCommand<>))
            {
                if (((ICommandBase)command).CanExecute(this.SourceCommandParameter))
                {
                    var concreteCommandType = commandTypeGeneric.MakeGenericType([paramType]);
                    var task = (Task)concreteCommandType.GetMethod("ExecuteAsync", [paramType])!
                        .Invoke(command, [this.SourceCommandParameter])!;
                    // TODO: should I keep ConfigureAwait(false) or leave it to the client code to decide? certainly should be conherent with unparametrised async command
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
