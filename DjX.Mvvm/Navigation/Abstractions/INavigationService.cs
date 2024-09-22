using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Navigation.Abstractions;
public interface INavigationService
{
    string ViewsNamespace { get; }

    void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase;

    void NavigateTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>;

    void CloseCurrent();
}
