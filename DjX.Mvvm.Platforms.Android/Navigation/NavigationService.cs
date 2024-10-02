using DjX.Mvvm.Core.Navigation;
using DjX.Mvvm.Core.Navigation.Abstractions;
using DjX.Mvvm.Core.ViewModels;
using System.Reflection;

namespace DjX.Mvvm.Platforms.Android.Navigation;

public class NavigationService(string viewsAssemblyName, string viewsNamespace)
    : INavigationService
{
    internal Assembly? AndroidExecutingAssembly { get; set; } = null;

    public string ViewsAssemblyName { get; } = viewsAssemblyName ?? throw new ArgumentNullException(viewsAssemblyName);

    public string ViewsNamespace { get; } = viewsNamespace ?? throw new ArgumentNullException(viewsNamespace);

    public event Action<Type>? NavigationToRequested;

    public event Action<Type, Type, object?>? NavigationWithModelToRequested;

    public event Action<Type, Type, object?>? NavigationWithModelForResultToRequested;

    public event Action? NavigationCloseRequested;

    public event Action<ResultStatus, object>? NavigationCloseWithResultRequested;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        => NavigationToRequested?.Invoke(typeof(TViewModel));

    public void NavigateWithModelTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>
        => NavigationWithModelToRequested?.Invoke(typeof(TViewModel), typeof(TModel), model);

    public void NavigateWithModelForResultTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>
        => NavigationWithModelForResultToRequested?
            .Invoke(typeof(TViewModel), typeof(TModel), model);

    public void CloseCurrent()
        => NavigationCloseRequested?.Invoke();

    public void CloseCurrentWithResult(ResultStatus resultStatus, object resultData)
        => NavigationCloseWithResultRequested?.Invoke(resultStatus, resultData);
}
