﻿namespace DjX.Mvvm.Core.ViewModels.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class LinkedViewAttribute(string viewName) : Attribute
{
    public string ViewName => viewName;
}
