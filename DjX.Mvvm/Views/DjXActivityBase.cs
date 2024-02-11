#if ANDROID21_0_OR_GREATER
using Android.Content;
using Android.Util;
using Android.Views;
using DjX.Mvvm.Binding;
using DjX.Mvvm.Platforms.Android;
using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Views;

public abstract class DjXActivityBase<T> : Activity
    where T : ViewModelBase
{
    private readonly AndroidBindingObject bindingObject = new();
    public T ViewModel { get; private set; }

    public DjXActivityBase()
    {
        if (Application is DjXApplication djXApplication)
        {
            ViewModel = djXApplication.CreateViewModel<T>();
        }
        else
        {
            throw new InvalidOperationException("Application must be of type DjXApplication");
        }
    }

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
        base.OnCreate(savedInstanceState);
    }

    protected override void OnDestroy()
    {
        ViewModel.OnViewModelDestroy();
        bindingObject.Dispose();
        base.OnDestroy();
    }

    private View? CreateView(View? parent, string name, Context context, IAttributeSet attrs)
    {
        return name switch
        {
            "EditText" => new EditText(context, attrs),
            "TextView" => new TextView(context, attrs),
            "Button" => new Button(context, attrs),
            _ => base.OnCreateView(parent, name, context, attrs),
        };
    }
}
#endif