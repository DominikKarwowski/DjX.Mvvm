using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Navigation.Abstractions;

public interface INavigationService
{
    string ViewsNamespace { get; }

    void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase;

    void NavigateWithModelTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>;

    //void NavigateForResultTo<TViewModel, TModel>(TModel model)
    //    where TViewModel : ViewModelBase<TModel>;

    void CloseCurrent();

    //void CloseCurrentWithResult();
}
