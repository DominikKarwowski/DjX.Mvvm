using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Navigation.Abstractions;
public interface INavigationService
{
    void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase;
}
