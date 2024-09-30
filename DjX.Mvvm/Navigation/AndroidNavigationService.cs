#if ANDROID21_0_OR_GREATER
using Android.Content;
using AndroidX.AppCompat.App;
using DjX.Mvvm.Navigation.Abstractions;
using DjX.Mvvm.ViewModels;
using DjX.Mvvm.ViewModels.Attributes;
using System.Reflection;

namespace DjX.Mvvm.Navigation;
public class AndroidNavigationService(string viewsAssemblyName, string viewsNamespace)
    : INavigationService
{
    internal Assembly? AndroidExecutingAssembly { get; set; } = null;

    public string ViewsAssemblyName { get; } = viewsAssemblyName ?? throw new ArgumentNullException(viewsAssemblyName);

    public string ViewsNamespace { get; } = viewsNamespace ?? throw new ArgumentNullException(viewsNamespace);

    public event Action<Type>? NavigationToRequested;

    public event Action<Type, Type, object?>? NavigationWithModelToRequested;

    public event Action? NavigationCloseRequested;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        => NavigationToRequested?.Invoke(typeof(TViewModel));

    public void NavigateWithModelTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>
        => NavigationWithModelToRequested?.Invoke(typeof(TViewModel), typeof(TModel), model);

    //public void NavigateForResultTo<TViewModel, TModel>(TModel model)
    //    where TViewModel : ViewModelBase<TModel>;

    public void CloseCurrent() => NavigationCloseRequested?.Invoke();
}

public class NavigationDataBinder(object? data) : Android.OS.Binder
{
    public object? Data { get; } = data;
}

public static class AndroidNavigationHandlers
{
    public static void NavigateTo(
        AppCompatActivity activity,
        AndroidNavigationService navigationService,
        Type viewModelType)
    {
        var viewType = GetViewForViewModel(navigationService, viewModelType);

        if (viewType is null)
        {
            return;
        }

        var intent = new Intent(activity, viewType);

        activity.StartActivity(intent);
    }

    public static void NavigateWithModelTo(
        AppCompatActivity activity,
        AndroidNavigationService navigationService,
        Type viewModelType,
        Type modelType,
        object? model)
    {
        var viewType = GetViewForViewModel(navigationService, viewModelType);

        if (viewType is null)
        {
            return;
        }

        var bundle = new Bundle();
        bundle.PutBinder("model", new NavigationDataBinder(model));
        bundle.PutBinder("modelType", new NavigationDataBinder(modelType));

        var intent = new Intent(activity, viewType)
            .PutExtras(bundle);

        activity.StartActivity(intent);
    }

    public static void NavigateClose(AppCompatActivity activity)
        => activity.Finish();

    private static Type? GetViewForViewModel(
        AndroidNavigationService navigationService,
        Type viewModelType)
    {
        var linkedViewAttr = viewModelType.GetCustomAttribute<LinkedViewAttribute>();

        if (string.IsNullOrWhiteSpace(linkedViewAttr?.ViewName))
        {
            return default;
        }

        var viewName = string.Join(".",
            navigationService.ViewsNamespace,
            linkedViewAttr.ViewName);

        navigationService.AndroidExecutingAssembly ??= TryResolveViewsAssembly(navigationService);

        var assembly = navigationService.AndroidExecutingAssembly;

        return assembly?.GetType(viewName);
    }

    private static Assembly? TryResolveViewsAssembly(
        AndroidNavigationService navigationService)
        => AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
                a.FullName?.StartsWith(navigationService.ViewsAssemblyName) ?? false)
            .FirstOrDefault();
}
#endif