#if ANDROID21_0_OR_GREATER
using Android.Runtime;
using DjX.Mvvm.Navigation.Abstractions;
using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Platforms.Android;
public abstract class DjXApplication(IntPtr handle, JniHandleOwnership transfer) : Application(handle, transfer)
{
    public abstract INavigationService GetNavigationService();

    public abstract T CreateViewModel<T>()
        where T : ViewModelBase;
}
#endif