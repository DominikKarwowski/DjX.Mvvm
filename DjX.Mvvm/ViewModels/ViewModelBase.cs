using DjX.Mvvm.Commands.Abstractions;
using DjX.Mvvm.Navigation.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DjX.Mvvm.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged, INavigable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<Type, Type?, object?>? NavigationToRequested;
    public event Action? NavigationCloseRequested;

    public virtual void OnViewModelDestroy() => this.DisposeAsyncCommands();

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        => NavigationToRequested?.Invoke(typeof(TViewModel), null, null);

    protected void NavigateTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase
        where TModel : class
        => NavigationToRequested?.Invoke(typeof(TViewModel), typeof(TModel), model);

    protected void NavigateClose() => NavigationCloseRequested?.Invoke();

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
