using Android.Content;
using AndroidX.AppCompat.App;
using DjX.Mvvm.Core.Navigation;
using DjX.Mvvm.Core.ViewModels;
using DjX.Mvvm.Platforms.Android.Resources;

namespace DjX.Mvvm.Platforms.Android.Navigation;

public static class NavigationHandlers
{
    public static void NavigateTo(
        AppCompatActivity activity,
        NavigationService navigationService,
        Type viewModelType)
    {
        var intent = NavigationIntentBuilder
            .CreateIntent(RequestedNavigationTo.NewViewModel, activity, navigationService, viewModelType)
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
            .CreateIntent(RequestedNavigationTo.ExistingViewModel, activity, navigationService, viewModelType)
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
            .CreateIntent(RequestedNavigationTo.NewViewModelWithModel ,activity, navigationService, viewModelType)
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
            .CreateIntent(RequestedNavigationTo.NewViewModelWithModelForResult, activity, navigationService, viewModelType)
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
