using Android.Content;
using AndroidX.AppCompat.App;
using DjX.Mvvm.Core.ViewModels.Attributes;
using DjX.Mvvm.Platforms.Android.Resources;
using System.Reflection;
using AndroidOS = Android.OS;

namespace DjX.Mvvm.Platforms.Android.Navigation;

public class NavigationIntentBuilder
{
    private Intent? Intent { get; }

    private Bundle Bundle { get; } = new();

    private NavigationIntentBuilder()
    {
    }

    private NavigationIntentBuilder(Intent? intent, Bundle bundle)
        => (this.Intent, this.Bundle) = (intent, bundle);

    public static NavigationIntentBuilder CreateIntent(
        RequestedNavigationTo navigationType,
        AppCompatActivity activity,
        NavigationService navigationService,
        Type viewModelType)
    {
        var viewType = GetViewForViewModel(navigationService, viewModelType);

        if (viewType is null)
        {
            return new NavigationIntentBuilder();
        }

        var bundle = new Bundle();

        bundle.PutInt(NavigationStrings.NavigationType, (int)navigationType);

        return new NavigationIntentBuilder(new Intent(activity, viewType), bundle);
    }

    public NavigationIntentBuilder SetViewModel(
        Type viewModelType,
        object? viewModel)
    {
        if (this.Intent is null)
        {
            return new NavigationIntentBuilder();
        }

        var binderType = typeof(NavigationDataBinder<>).MakeGenericType(viewModelType);
        var binder = (AndroidOS.Binder)Activator.CreateInstance(binderType, viewModel)!;

        this.Bundle.PutBinder(NavigationStrings.ViewModel, binder);

        return new NavigationIntentBuilder(this.Intent, this.Bundle);
    }

    public NavigationIntentBuilder SetModel(
        Type modelType,
        object? model)
    {
        if (this.Intent is null)
        {
            return new NavigationIntentBuilder();
        }

        this.Bundle.PutBinder(NavigationStrings.Model, new NavigationDataBinder(model));
        this.Bundle.PutBinder(NavigationStrings.ModelType, new NavigationDataBinder(modelType));

        return new NavigationIntentBuilder(this.Intent, this.Bundle);
    }

    public Intent? Build() => this.Intent?.PutExtras(this.Bundle);

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
