using DjX.Mvvm.Commands.Abstractions;
using DjX.Mvvm.Navigation.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DjX.Mvvm.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged, INavigable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<Type>? NavigationRequested;

    public virtual void OnViewModelDestroy() => this.DisposeAsyncCommands();

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected void NavigateTo<TViewModel>() where TViewModel : ViewModelBase =>
        NavigationRequested?.Invoke(typeof(TViewModel));

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
