#if ANDROID21_0_OR_GREATER
using Android.Runtime;
using DjX.Mvvm.Navigation.Abstractions;
using DjX.Mvvm.ViewModels;
using DjX.Mvvm.ViewModels.Factories;

namespace DjX.Mvvm.Platforms.Android;
public abstract class DjXApplication(IntPtr handle, JniHandleOwnership transfer) : Application(handle, transfer)
{
    public abstract INavigationService GetNavigationService();

    public abstract ViewModelFactory<TViewModel> GetViewModelFactory<TViewModel>()
        where TViewModel : ViewModelBase;
}
#endif