#if ANDROID21_0_OR_GREATER
using DjX.Mvvm.Navigation.Abstractions;
using DjX.Mvvm.ViewModels;
using System.Reflection;

namespace DjX.Mvvm.Navigation;
public class AndroidNavigationService(string viewsAssemblyName, string viewsNamespace) : INavigationService
{
    internal Assembly? AndroidExecutingAssembly { get; set; } = null;

    public string ViewsAssemblyName { get; } = viewsAssemblyName ?? throw new ArgumentNullException(viewsAssemblyName);

    public string ViewsNamespace { get; } = viewsNamespace ?? throw new ArgumentNullException(viewsNamespace);

    public event Action<Type, Type?, object?>? NavigationToRequested;

    public event Action? NavigationCloseRequested;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        => NavigationToRequested?.Invoke(typeof(TViewModel), null, null);

    public void NavigateTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>
        => NavigationToRequested?.Invoke(typeof(TViewModel), typeof(TModel), model);

    public void CloseCurrent() => NavigationCloseRequested?.Invoke();
}
#endif