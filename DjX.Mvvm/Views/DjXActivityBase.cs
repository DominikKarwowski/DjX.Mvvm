#if ANDROID21_0_OR_GREATER
using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using DjX.Mvvm.Binding;
using DjX.Mvvm.Navigation;
using DjX.Mvvm.Platforms.Android;
using DjX.Mvvm.Resources;
using DjX.Mvvm.ViewModels;
using DjX.Mvvm.ViewModels.Attributes;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextView;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DjX.Mvvm.Views;

public abstract class DjXActivityBase<T> : AppCompatActivity
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
        var view = this.CreateView(parent, name, context, attrs);

        if (view is null)
        {
            return view;
        }

        var propertyBindingToParse = attrs.GetAttributeValue(AndroidStrings.AppNamespace, AndroidStrings.BindAttributeName);
        var eventBindingToParse = attrs.GetAttributeValue(AndroidStrings.AppNamespace, "bind_event");
        var collectionToBind = attrs.GetAttributeValue(AndroidStrings.AppNamespace, "item_source");
        var templateResourceId = attrs.GetAttributeResourceValue(AndroidStrings.AppNamespace, "item_template", 0);

        if (propertyBindingToParse is not null)
        {
            this.bindingObject.RegisterBindingSet(this.ViewModel, view, propertyBindingToParse);
        }

        if (eventBindingToParse is not null)
        {
            this.bindingObject.RegisterBindingSet(this.ViewModel, view, eventBindingToParse);
        }

        if (view is RecyclerView recyclerView && collectionToBind is not null && templateResourceId is not 0)
        {
            this.bindingObject.RegisterCollectionBindingSet(this.ViewModel, collectionToBind, recyclerView, templateResourceId);
        }

        return view;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (this.Application is not DjXApplication djXApplication)
        {
            throw new InvalidOperationException($"Application must be of type {nameof(DjXApplication)}");
        }

        this.ViewModel = djXApplication.CreateViewModel<T>();
        this.navigationService = djXApplication.GetNavigationService() as AndroidNavigationService;

        this.ViewModel.NavigationRequested += this.NavigateTo;

        base.OnCreate(savedInstanceState);
    }

    protected override void OnDestroy()
    {
        this.ViewModel.NavigationRequested -= this.NavigateTo;
        this.ViewModel.OnViewModelDestroy();
        this.bindingObject.Dispose();
        base.OnDestroy();
    }

    private View? CreateView(View? parent, string name, Context context, IAttributeSet attrs)
        => name switch
        {
            "EditText" => new EditText(context, attrs),
            "TextView" => new TextView(context, attrs),
            "Button" => new Button(context, attrs),
            "com.google.android.material.textview.MaterialTextView" => new MaterialTextView(context, attrs),
            "androidx.appcompat.widget.AppCompatEditText" => new AppCompatEditText(context, attrs),
            "androidx.appcompat.widget.AppCompatButton" => new AppCompatButton(context, attrs),
            "com.google.android.material.floatingactionbutton.FloatingActionButton" => new FloatingActionButton(context, attrs),
            "androidx.recyclerview.widget.RecyclerView" => new RecyclerView(context, attrs),
            _ => base.OnCreateView(parent, name, context, attrs),
        };

    private void NavigateTo(Type viewModelType)
    {
        Type? viewType = this.GetViewForViewModel(viewModelType);

        if (viewType is not null)
        {
            var intent = new Intent(this, viewType);
            this.StartActivity(intent);
        }
    }

    private Type? GetViewForViewModel(Type viewModelType)
    {
        var linkedViewAttr = viewModelType.GetCustomAttribute<LinkedViewAttribute>();

        if (string.IsNullOrWhiteSpace(this.navigationService?.ViewsNamespace)
            || string.IsNullOrWhiteSpace(linkedViewAttr?.ViewName))
        {
            return default;
        }

        var viewName = string.Join(".",
            this.navigationService.ViewsNamespace,
            linkedViewAttr.ViewName);

        var assembly = this.navigationService.AndroidExecutingAssembly ?? this.TryResolveExecutingAssembly();

        return assembly?.GetType(viewName);
    }

    private Assembly? TryResolveExecutingAssembly()
    {
        if (this.navigationService is null)
            return default;

        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
                a.FullName?.StartsWith(this.navigationService.ViewsAssemblyName) ?? false)
            .FirstOrDefault();

        this.navigationService.AndroidExecutingAssembly = assembly;

        return assembly;
    }
}
#endif