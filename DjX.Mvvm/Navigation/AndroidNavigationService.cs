#if ANDROID21_0_OR_GREATER
using DjX.Mvvm.Navigation.Abstractions;
using DjX.Mvvm.ViewModels;
using System.Diagnostics;

namespace DjX.Mvvm.Navigation;
public class AndroidNavigationService : INavigationService
{
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        Debug.WriteLine($"Navigating to {typeof(TViewModel).Name}");
    }
}
#endif