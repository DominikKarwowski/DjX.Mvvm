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
