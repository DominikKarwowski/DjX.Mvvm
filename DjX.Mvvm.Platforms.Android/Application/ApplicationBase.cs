using DjX.Mvvm.Core.ViewModels;
using DjX.Mvvm.Core.ViewModels.Factories;
using DjX.Mvvm.Platforms.Android.Navigation;
using AndroidApp = Android.App;
using AndroidRuntime = Android.Runtime;

namespace DjX.Mvvm.Platforms.Android.Application;
public abstract class ApplicationBase(nint handle, AndroidRuntime.JniHandleOwnership transfer)
    : AndroidApp.Application(handle, transfer)
{
    // TODO: this design still does not imply that navigation service should be registered as a singleton and use this same instance for viewmodels!!!
    // encapsulate it in an extension method for registering?
    private NavigationService? navigationService;

    public NavigationService NavigationService
    {
        get => this.navigationService is null
                ? throw new InvalidOperationException($"Navigation service was not set on {nameof(ApplicationBase)}. Provide an instance of {nameof(Navigation.NavigationService)} before using it.")
                : this.navigationService;
        protected set => this.navigationService = value;
    }

    public abstract ViewModelFactory<TViewModel> GetViewModelFactory<TViewModel>()
        where TViewModel : ViewModelBase;
}