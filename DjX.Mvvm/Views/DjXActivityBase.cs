#if ANDROID21_0_OR_GREATER
using Android.Content;
using Android.OS;
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

        var bindingsToParse = attrs.GetAttributeValue(AndroidStrings.AppNamespace, AndroidStrings.BindAttributeName);
        var collectionToBind = attrs.GetAttributeValue(AndroidStrings.AppNamespace, AndroidStrings.ItemSourceAttributeName);
        var templateResourceId = attrs.GetAttributeResourceValue(AndroidStrings.AppNamespace, AndroidStrings.ItemTemplateAttributeName, 0);

        if (bindingsToParse is not null)
        {
            this.bindingObject.RegisterDeclaredBindings(this.ViewModel, view, bindingsToParse);
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

        var model = (this.Intent?.Extras?.GetBinder("model") as NavigationDataBinder)?.Data;
        var modelType = (this.Intent?.Extras?.GetBinder("modelType") as NavigationDataBinder)?.Data as Type;

        this.ViewModel = model is not null && modelType is not null
            ? djXApplication.GetViewModelFactory<T>().CreateViewModel(model, modelType)
            : djXApplication.GetViewModelFactory<T>().CreateViewModel();
        this.navigationService = djXApplication.GetNavigationService() as AndroidNavigationService;

        this.ViewModel.NavigationToRequested += this.NavigateTo;
        this.ViewModel.NavigationCloseRequested += this.NavigateClose;

        base.OnCreate(savedInstanceState);
    }

    protected override void OnDestroy()
    {
        this.ViewModel.NavigationToRequested -= this.NavigateTo;
        this.ViewModel.NavigationCloseRequested -= this.NavigateClose;
        this.ViewModel.OnViewModelDestroy();
        this.bindingObject.Dispose();
        base.OnDestroy();
    }

    private View? CreateView(View? parent, string name, Context context, IAttributeSet attrs)
        => name switch
        {
            "com.google.android.material.textview.MaterialTextView" => new MaterialTextView(context, attrs),
            "androidx.appcompat.widget.AppCompatEditText" => new AppCompatEditText(context, attrs),
            "androidx.appcompat.widget.AppCompatButton" => new AppCompatButton(context, attrs),
            "com.google.android.material.floatingactionbutton.FloatingActionButton" => new FloatingActionButton(context, attrs),
            "androidx.recyclerview.widget.RecyclerView" => new RecyclerView(context, attrs),
            "EditText" => new EditText(context, attrs),
            "TextView" => new TextView(context, attrs),
            "Button" => new Button(context, attrs),
            _ => base.OnCreateView(parent, name, context, attrs),
        };

    private void NavigateTo(Type viewModelType, Type? modelType, object? model)
    {
        var viewType = this.GetViewForViewModel(viewModelType);

        if (viewType is null)
        {
            return;
        }

        var intent = new Intent(this, viewType);

        if (modelType is not null && model is not null)
        {
            var bundle = new Bundle();
            bundle.PutBinder("model", new NavigationDataBinder(model));
            bundle.PutBinder("modelType", new NavigationDataBinder(modelType));
            _ = intent.PutExtras(bundle);
        }

        this.StartActivity(intent);
    }

    private void NavigateClose() => this.Finish();

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

public class NavigationDataBinder(object data) : Android.OS.Binder
{
    public object Data { get; } = data;
}
#endif