using Android.Content;
using AndroidX.AppCompat.App;
using DjX.Mvvm.Core.Navigation;
using DjX.Mvvm.Core.ViewModels.Attributes;
using DjX.Mvvm.Platforms.Android.Resources;
using System.Reflection;

namespace DjX.Mvvm.Platforms.Android.Navigation;

public static class NavigationHandlers
{
    public static void NavigateTo(
        AppCompatActivity activity,
        NavigationService navigationService,
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
        NavigationService navigationService,
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
        NavigationService navigationService,
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
        bundle.PutBinder(NavigationStrings.ResultData, new NavigationDataBinder(resultData));

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
        NavigationService navigationService,
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
        NavigationService navigationService)
        => AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
                a.FullName?.StartsWith(navigationService.ViewsAssemblyName) ?? false)
            .FirstOrDefault();

    private static Intent? CreateIntent(
        AppCompatActivity activity,
        NavigationService navigationService,
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
            bundle.PutBinder(NavigationStrings.Model, new NavigationDataBinder(model));
            bundle.PutBinder(NavigationStrings.ModelType, new NavigationDataBinder(modelType));

            _ = intent.PutExtras(bundle);
        }

        return intent;
    }
}
