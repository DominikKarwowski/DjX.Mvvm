using AndroidOS = Android.OS;

namespace DjX.Mvvm.Platforms.Android.Navigation;

public class NavigationDataBinder(object? data) : AndroidOS.Binder
{
    public object? Data { get; } = data;
}
