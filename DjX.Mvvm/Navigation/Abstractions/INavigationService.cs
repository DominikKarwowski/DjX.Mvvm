using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Navigation.Abstractions;
public interface INavigationService
{
    string ViewsNamespace { get; }

    event Action<Type, Type?, object?>? NavigationToRequested;

    event Action? NavigationCloseRequested;

    void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase;

    void NavigateTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>;

    void CloseCurrent();
}
