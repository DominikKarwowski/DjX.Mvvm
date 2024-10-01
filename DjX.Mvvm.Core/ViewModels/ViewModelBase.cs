using DjX.Mvvm.Core.Commands.Abstractions;
using DjX.Mvvm.Core.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DjX.Mvvm.Core.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public virtual void OnViewModelDestroy() => this.DisposeAsyncCommands();

    public virtual void OnResultFromView(ResultStatus resultStatus, object resultData)
    {
        // do nothing
    }

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void DisposeAsyncCommands()
    {
        var commands = this.GetType().GetProperties()
            .Where(p =>
                p.PropertyType == typeof(IDjXAsyncCommand)
                || p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IDjXAsyncCommand<>))
            .Select(p => p.GetValue(this));

        foreach (var command in commands)
        {
            if (command is IDisposable disposableCommand)
            {
                disposableCommand.Dispose();
            }
        }
    }
}
