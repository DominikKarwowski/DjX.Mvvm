using Android.Content;
using AndroidX.AppCompat.App;
using DjX.Mvvm.Core.Navigation;
using DjX.Mvvm.Core.ViewModels;
using DjX.Mvvm.Core.ViewModels.Attributes;
using DjX.Mvvm.Platforms.Android.Resources;
using System.Reflection;
using AndroidOS = Android.OS;

namespace DjX.Mvvm.Platforms.Android.Navigation;

public static class NavigationHandlers
{
    public static void NavigateTo(
        AppCompatActivity activity,
        NavigationService navigationService,
        Type viewModelType)
    {
        var intent = NavigationIntentBuilder
            .CreateIntent(activity, navigationService, viewModelType)
            .Build();

        if (intent is null)
        {
            return;
        }

        activity.StartActivity(intent);
    }

    public static void NavigateWithViewModelTo(
        AppCompatActivity activity,
        NavigationService navigationService,
        Type viewModelType,
        ViewModelBase viewModel)
    {
        var intent = NavigationIntentBuilder
            .CreateIntent(activity, navigationService, viewModelType)
            .SetViewModel(viewModelType, viewModel)
            .Build();

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
        var intent = NavigationIntentBuilder
            .CreateIntent(activity, navigationService, viewModelType)
            .SetModel(modelType, model)
            .Build();

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
        var intent = NavigationIntentBuilder
            .CreateIntent(activity, navigationService, viewModelType)
            .SetModel(modelType, model)
            .Build();

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
}

public class NavigationIntentBuilder
{
    private Intent? Intent { get; }

    private NavigationIntentBuilder()
    {
    }

    private NavigationIntentBuilder(Intent? intent)
        => this.Intent = intent;

    public static NavigationIntentBuilder CreateIntent(
        AppCompatActivity activity,
        NavigationService navigationService,
        Type viewModelType)
    {
        var viewType = GetViewForViewModel(navigationService, viewModelType);

        return viewType is null
            ? new NavigationIntentBuilder()
            : new NavigationIntentBuilder(new Intent(activity, viewType));
    }

    public NavigationIntentBuilder SetViewModel(
        Type viewModelType,
        object? viewModel)
    {
        if (this.Intent is null)
        {
            return new NavigationIntentBuilder();
        }

        var bundle = new Bundle();

        var binderType = typeof(NavigationDataBinder<>).MakeGenericType(viewModelType);
        var binder = (AndroidOS.Binder)Activator.CreateInstance(binderType, viewModel)!;

        bundle.PutBinder(NavigationStrings.ViewModel, binder);

        var intent = this.Intent.PutExtras(bundle);

        return new NavigationIntentBuilder(intent);
    }

    public NavigationIntentBuilder SetModel(
        Type modelType,
        object? model)
    {
        if (this.Intent is null)
        {
            return new NavigationIntentBuilder();
        }

        var bundle = new Bundle();
        bundle.PutBinder(NavigationStrings.Model, new NavigationDataBinder(model));
        bundle.PutBinder(NavigationStrings.ModelType, new NavigationDataBinder(modelType));

        var intent = this.Intent.PutExtras(bundle);

        return new NavigationIntentBuilder(intent);
    }

    public Intent? Build() => this.Intent;

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
}
