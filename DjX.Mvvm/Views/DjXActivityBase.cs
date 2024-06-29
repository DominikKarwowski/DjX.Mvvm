#if ANDROID21_0_OR_GREATER
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using DjX.Mvvm.Binding;
using DjX.Mvvm.Navigation;
using DjX.Mvvm.Platforms.Android;
using DjX.Mvvm.ViewModels;
using DjX.Mvvm.ViewModels.Attributes;
using System.Reflection;

namespace DjX.Mvvm.Views;

public abstract class DjXActivityBase<T> : Activity
    where T : ViewModelBase
{
    private AndroidNavigationService? navigationService;
    private readonly AndroidBindingObject bindingObject = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ViewModel is set in OnCreate

    public T ViewModel { get; private set; }
#pragma warning restore CS8618

    public override View? OnCreateView(View? parent, string name, Context context, IAttributeSet attrs)
    {
        var namespaceUri = "http://schemas.android.com/apk/res-auto";
        var propertyBindingToParse = attrs.GetAttributeValue(namespaceUri, "bind_property");
        var eventBindingToParse = attrs.GetAttributeValue(namespaceUri, "bind_event");

        View? view = CreateView(parent, name, context, attrs);

        if (view is not null)
        {
            if (propertyBindingToParse is not null)
            {
                bindingObject.RegisterPropertyBindingSet(ViewModel, view, propertyBindingToParse);
            }

            if (eventBindingToParse is not null)
            {
                bindingObject.RegisterEventBindingSet(ViewModel, view, eventBindingToParse);
            }
        }

        return view;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (Application is DjXApplication djXApplication)
        {
            ViewModel = djXApplication.CreateViewModel<T>();
            navigationService = djXApplication.GetNavigationService() as AndroidNavigationService;

            ViewModel.NavigationRequested += NavigateTo;
        }
        else
        {
            throw new InvalidOperationException($"Application must be of type {nameof(DjXApplication)}");
        }

        base.OnCreate(savedInstanceState);
    }

    protected override void OnDestroy()
    {
        ViewModel.NavigationRequested -= NavigateTo;
        ViewModel.OnViewModelDestroy();
        bindingObject.Dispose();
        base.OnDestroy();
    }

    private View? CreateView(View? parent, string name, Context context, IAttributeSet attrs) =>
        name switch
        {
            "EditText" => new EditText(context, attrs),
            "TextView" => new TextView(context, attrs),
            "Button" => new Button(context, attrs),
            _ => base.OnCreateView(parent, name, context, attrs),
        };

    private void NavigateTo(Type viewModelType)
    {
        Type? viewType = GetViewForViewModel(viewModelType);

        if (viewType is not null)
        {
            var intent = new Intent(this, viewType);
            StartActivity(intent);
        }
    }

    private Type? GetViewForViewModel(Type viewModelType)
    {
        var linkedViewAttr = viewModelType.GetCustomAttribute<LinkedViewAttribute>();

        if (string.IsNullOrWhiteSpace(navigationService?.ViewsNamespace)
            || string.IsNullOrWhiteSpace(linkedViewAttr?.ViewName))
        {
            return default;
        }

        var viewName = string.Join(".",
            navigationService.ViewsNamespace,
            linkedViewAttr.ViewName);

        var assembly = navigationService.AndroidExecutingAssembly ?? TryResolveExecutingAssembly();

        return assembly?.GetType(viewName);
    }

    private Assembly? TryResolveExecutingAssembly()
    {
        if (navigationService is null)
            return default;

        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
                a.FullName?.StartsWith(navigationService.ViewsAssemblyName) ?? false)
            .FirstOrDefault();

        navigationService.AndroidExecutingAssembly = assembly;

        return assembly;
    }
}
#endif