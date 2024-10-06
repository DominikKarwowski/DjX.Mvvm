using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using DjX.Mvvm.Core.Navigation;
using DjX.Mvvm.Core.ViewModels;
using DjX.Mvvm.Platforms.Android.Application;
using DjX.Mvvm.Platforms.Android.Binding;
using DjX.Mvvm.Platforms.Android.Navigation;
using DjX.Mvvm.Platforms.Android.Resources;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextView;

namespace DjX.Mvvm.Platforms.Android.Views;

public abstract class ActivityBase<TViewModel> : AppCompatActivity
    where TViewModel : ViewModelBase
{
    private readonly BindingObject bindingObject = new();

    private NavigationService NavigationService => ((ApplicationBase)this.Application!).NavigationService;

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

        var bindingDeclaration = attrs.GetAttributeValue(AttributeStrings.AppNamespace, AttributeStrings.BindAttributeName);

        if (bindingDeclaration is not null)
        {
            this.bindingObject.RegisterDeclaredBindings(view, this.ViewModel, bindingDeclaration);
        }

        if (view is RecyclerView recyclerView)
        {
            var collectionToBind = attrs.GetAttributeValue(AttributeStrings.AppNamespace, AttributeStrings.ItemSourceAttributeName);
            var templateResourceId = attrs.GetAttributeResourceValue(AttributeStrings.AppNamespace, AttributeStrings.ItemTemplateAttributeName, 0);

            if (collectionToBind is not null && templateResourceId is not 0)
            {
                var itemBindingDeclaration = attrs.GetAttributeValue(AttributeStrings.AppNamespace, AttributeStrings.ItemBindAttributeName);

                this.bindingObject.RegisterCollectionBindingSet(
                    recyclerView,
                    this.ViewModel,
                    collectionToBind,
                    templateResourceId,
                    itemBindingDeclaration);
            }
        }

        return view;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (this.Application is not ApplicationBase app)
        {
            throw new InvalidOperationException($"Application must be of type {nameof(ApplicationBase)}");
        }

        var model = (this.Intent?.Extras?.GetBinder(NavigationStrings.Model) as NavigationDataBinder)?.Data;
        var modelType = (this.Intent?.Extras?.GetBinder(NavigationStrings.ModelType) as NavigationDataBinder)?.Data as Type;

        this.ViewModel = modelType is not null
            ? app.GetViewModelFactory<TViewModel>().CreateViewModel(model, modelType)
            : app.GetViewModelFactory<TViewModel>().CreateViewModel();

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
        var resultData = (data?.Extras?.GetBinder(NavigationStrings.ResultData) as NavigationDataBinder)?.Data;

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
        => NavigationHandlers.NavigateTo(this, this.NavigationService, viewModelType);

    private void NavigateWithModelTo(Type viewModelType, Type modelType, object? model)
        => NavigationHandlers.NavigateWithModelTo(this, this.NavigationService, viewModelType, modelType, model);

    private void NavigateWithModelForResultTo(Type viewModelType, Type modelType, object? model)
        => NavigationHandlers.NavigateWithModelForResultTo(this, this.NavigationService, viewModelType, modelType, model);

    private void NavigateClose()
        => NavigationHandlers.NavigateClose(this);

    private void NavigateCloseWithResult(ResultStatus resultStatus, object resultData)
        => NavigationHandlers.NavigateCloseWithResult(this, resultStatus, resultData);
}
