﻿using Android.Text;
using Android.Views;
using DjX.Mvvm.Core.Binding.Abstractions;
using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Platforms.Android.Binding;

public sealed class PropertyBindingSet : BindingSet<View, PropertyInfo>
{
    private bool disposedValue;

    public PropertyBindingSet(
        INotifyPropertyChanged sourceObject,
        PropertyInfo sourceMemberInfo,
        View targetObject,
        PropertyInfo targetMemberName)
        : base(sourceObject, sourceMemberInfo, targetObject, targetMemberName)
    {
        this.SourceObject.PropertyChanged += this.OnSourcePropertyChanged;

        if (this.TargetObject is EditText editText)
        {
            editText.TextChanged += this.EditText_TextChanged;
        }

        this.OnSourcePropertyChanged(this.SourceObject, new PropertyChangedEventArgs(sourceMemberInfo.Name));
    }

    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == this.SourceMemberInfo.Name)
        {
            var sourceValue = this.SourceMemberInfo.GetValue(this.SourceObject);
            var currentTargetValue = this.TargetMemberInfo.GetValue(this.TargetObject);
            if (!Equals(currentTargetValue, sourceValue))
            {
                this.TargetMemberInfo.SetValue(this.TargetObject, sourceValue);
            }
        }
    }

    private void EditText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.Text is not null)
        {
            this.SourceMemberInfo.SetValue(this.SourceObject, string.Join("", e.Text));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.SourceObject.PropertyChanged -= this.OnSourcePropertyChanged;

                if (this.TargetObject is EditText editText)
                {
                    editText.TextChanged -= this.EditText_TextChanged;
                }
            }
            this.disposedValue = true;
        }
    }
}
