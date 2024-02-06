#if ANDROID21_0_OR_GREATER
using Android.OS;
using DjX.Mvvm.Platforms.Android;
using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Views;

public abstract class DjXActivityBase<T> : Activity
    where T : ViewModelBase
{
    public T ViewModel { get; set; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        if (Application is DjXApplication djXApplication)
        {
            ViewModel = djXApplication.CreateViewModel<T>();
        }
        else
        {
            throw new InvalidOperationException("Application must be of type DjXApplication");
        }
    }

    protected override void OnDestroy()
    {
        ViewModel.OnViewModelDestroy();
        base.OnDestroy();
    }
}
#endif