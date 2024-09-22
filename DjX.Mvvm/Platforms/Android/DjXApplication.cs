#if ANDROID21_0_OR_GREATER
using Android.Runtime;
using DjX.Mvvm.Navigation;
using DjX.Mvvm.ViewModels;
using DjX.Mvvm.ViewModels.Factories;

namespace DjX.Mvvm.Platforms.Android;
public abstract class DjXApplication(IntPtr handle, JniHandleOwnership transfer) : Application(handle, transfer)
{
    // TODO: this design still does not imply that navigation service should be registered as a singleton and use this same instance for viewmodels!!!
    // encapsulate it in an extension method for registering?
    private AndroidNavigationService? navigationService;

    public AndroidNavigationService NavigationService
    {
        get => this.navigationService is null
                ? throw new InvalidOperationException($"Navigation service was not set on {nameof(DjXApplication)}. Provide an instance of {nameof(AndroidNavigationService)} before using it.")
                : this.navigationService;
        protected set => this.navigationService = value;
    }

    public abstract ViewModelFactory<TViewModel> GetViewModelFactory<TViewModel>()
        where TViewModel : ViewModelBase;
}
#endif