using DjX.Mvvm.Commands.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DjX.Mvvm.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void OnViewModelDestroy()
    {
        DisposeAsyncCommands();
    }

    private void DisposeAsyncCommands()
    {
        var disposableCommands = GetType().GetProperties()
            .Where(p =>
                p.PropertyType == typeof(IDjXAsyncCommand)
                || p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IDjXAsyncCommand<>))
            .Select(p => p.GetValue(this));

        foreach (var disposableCommand in disposableCommands)
        {
            if (disposableCommand is IDisposable cmd)
            {
                cmd.Dispose();
            }
        }
    }
}
