#if ANDROID21_0_OR_GREATER
using DjX.Mvvm.Navigation.Abstractions;
using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Navigation;
public class AndroidNavigationService : INavigationService
{
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {

    }
}
#endif