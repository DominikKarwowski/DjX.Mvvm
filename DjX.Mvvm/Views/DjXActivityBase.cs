#if ANDROID21_0_OR_GREATER
using Android.Content;
using Android.Runtime;
using Android.Security.Identity;
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
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextView;

namespace DjX.Mvvm.Views;

public abstract class DjXActivityBase<TViewModel> : AppCompatActivity
    where TViewModel : ViewModelBase
{
    private readonly AndroidBindingObject bindingObject = new();

    private AndroidNavigationService NavigationService => ((DjXApplication)this.Application!).NavigationService;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ViewModel is set in OnCreate
    public TViewModel ViewModel { get; private set; }
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

        this.ViewModel = modelType is not null
            ? djXApplication.GetViewModelFactory<TViewModel>().CreateViewModel(model, modelType)
            : djXApplication.GetViewModelFactory<TViewModel>().CreateViewModel();

        base.OnCreate(savedInstanceState);
    }

    protected override void OnStart() => base.OnStart();

    protected override void OnRestart() => base.OnRestart();

    protected override void OnResume()
    {
        this.SubscribeToNavigationEvents();

        base.OnResume();
    }

    protected override void OnPause()
    {
        this.UnsubscribeFromNavigationEvents();

        base.OnPause();
    }

    protected override void OnStop() => base.OnStop();

    protected override void OnDestroy()
    {
        this.ViewModel.OnViewModelDestroy();
        this.bindingObject.Dispose();
        base.OnDestroy();
    }

    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent? data)
    {
        var resultData = (data?.Extras?.GetBinder(AndroidNavigationHandlers.ResultData) as NavigationDataBinder)?.Data;

        if (resultData is null)
        {
            return;
        }

        var resultStatus = resultCode switch
        {
            Result.Ok => ResultStatus.Ok,
            (Result)2 => ResultStatus.Error,
            _ => ResultStatus.Undefined,
        };

        this.ViewModel.OnResultFromView(resultStatus, resultData);
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

    private void SubscribeToNavigationEvents()
    {
        this.NavigationService.NavigationToRequested += this.NavigateTo;
        this.NavigationService.NavigationWithModelToRequested += this.NavigateWithModelTo;
        this.NavigationService.NavigationWithModelForResultToRequested += this.NavigateWithModelForResultTo;
        this.NavigationService.NavigationCloseRequested += this.NavigateClose;
        this.NavigationService.NavigationCloseWithResultRequested += this.NavigateCloseWithResult;
    }

    private void UnsubscribeFromNavigationEvents()
    {
        this.NavigationService.NavigationToRequested -= this.NavigateTo;
        this.NavigationService.NavigationWithModelToRequested -= this.NavigateWithModelTo;
        this.NavigationService.NavigationWithModelForResultToRequested -= this.NavigateWithModelForResultTo;
        this.NavigationService.NavigationCloseRequested -= this.NavigateClose;
        this.NavigationService.NavigationCloseWithResultRequested -= this.NavigateCloseWithResult;
    }

    private void NavigateTo(Type viewModelType)
        => AndroidNavigationHandlers.NavigateTo(this, this.NavigationService, viewModelType);

    private void NavigateWithModelTo(Type viewModelType, Type modelType, object? model)
        => AndroidNavigationHandlers.NavigateWithModelTo(this, this.NavigationService, viewModelType, modelType, model);

    private void NavigateWithModelForResultTo(Type viewModelType, Type modelType, object? model)
        => AndroidNavigationHandlers.NavigateWithModelForResultTo(this, this.NavigationService, viewModelType, modelType, model);

    private void NavigateClose()
        => AndroidNavigationHandlers.NavigateClose(this);

    private void NavigateCloseWithResult(ResultStatus resultStatus, object resultData)
        => AndroidNavigationHandlers.NavigateCloseWithResult(this, resultStatus, resultData);
}
#endif