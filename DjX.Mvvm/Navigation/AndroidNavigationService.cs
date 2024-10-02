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

    public event Action<Type, Type, object?>? NavigationWithModelForResultToRequested;

    public event Action? NavigationCloseRequested;

    public event Action<ResultStatus, object>? NavigationCloseWithResultRequested;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        => NavigationToRequested?.Invoke(typeof(TViewModel));

    public void NavigateWithModelTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>
        => NavigationWithModelToRequested?.Invoke(typeof(TViewModel), typeof(TModel), model);

    public void NavigateWithModelForResultTo<TViewModel, TModel>(TModel model)
        where TViewModel : ViewModelBase<TModel>
        => NavigationWithModelForResultToRequested?
            .Invoke(typeof(TViewModel), typeof(TModel), model);

    public void CloseCurrent()
        => NavigationCloseRequested?.Invoke();

    public void CloseCurrentWithResult(ResultStatus resultStatus, object resultData)
        => NavigationCloseWithResultRequested?.Invoke(resultStatus, resultData);
}

public class NavigationDataBinder(object? data) : Android.OS.Binder
{
    public object? Data { get; } = data;
}

public static class AndroidNavigationHandlers
{
    public const string ResultData = "resultData";

    public static void NavigateTo(
        AppCompatActivity activity,
        AndroidNavigationService navigationService,
        Type viewModelType)
    {
        var intent = CreateIntent(activity, navigationService, viewModelType);

        if (intent is null)
        {
            return;
        }

        activity.StartActivity(intent);
    }

    public static void NavigateWithModelTo(
        AppCompatActivity activity,
        AndroidNavigationService navigationService,
        Type viewModelType,
        Type modelType,
        object? model)
    {
        var intent = CreateIntent(activity, navigationService, viewModelType, modelType, model);

        if (intent is null)
        {
            return;
        }

        activity.StartActivity(intent);
    }

    public static void NavigateWithModelForResultTo(
        AppCompatActivity activity,
        AndroidNavigationService navigationService,
        Type viewModelType,
        Type modelType,
        object? model)
    {
        var intent = CreateIntent(activity, navigationService, viewModelType, modelType, model);

        if (intent is null)
        {
            return;
        }

        activity.StartActivityForResult(intent, 0);
    }

    public static void NavigateClose(AppCompatActivity activity)
        => activity.Finish();

    public static void NavigateCloseWithResult(
        AppCompatActivity activity,
        ResultStatus viewResultStatus,
        object resultData)
    {
        // TODO: check AndroidX Activity Result API: https://developer.android.com/training/basics/intents/result
        // TODO: consider setting requestcode in NavigateWithModelForResultTo and get it back here
        // to allow client code to explicitly couple NavigateTo with Close
        // TODO: handle failed result case - create a custom enum value - but how?!
        var bundle = new Bundle();
        bundle.PutBinder(ResultData, new NavigationDataBinder(resultData));

        var result = viewResultStatus switch
        {
            ResultStatus.Ok => Result.Ok,
            ResultStatus.Error => (Result)2,
            _ => Result.FirstUser,
        };

        activity.SetResult(
            result,
            new Intent().PutExtras(bundle));

        activity.Finish();
    }

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

    private static Intent? CreateIntent(
        AppCompatActivity activity,
        AndroidNavigationService navigationService,
        Type viewModelType,
        Type? modelType = null,
        object? model = null)
    {
        var viewType = GetViewForViewModel(navigationService, viewModelType);

        if (viewType is null)
        {
            return null;
        }

        var intent = new Intent(activity, viewType);

        if (modelType is not null)
        {
            var bundle = new Bundle();
            bundle.PutBinder("model", new NavigationDataBinder(model));
            bundle.PutBinder("modelType", new NavigationDataBinder(modelType));

            _ = intent.PutExtras(bundle);
        }

        return intent;
    }
}
#endif