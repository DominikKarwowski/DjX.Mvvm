﻿#if ANDROID21_0_OR_GREATER
using Android.Content;
using Android.Runtime;
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
        }
        else
        {
            throw new InvalidOperationException("Application must be of type DjXApplication");
        }

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