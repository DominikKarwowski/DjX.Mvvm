using System.Runtime.CompilerServices;

namespace DjX.Mvvm.ViewModels;

public abstract class ViewModelBase<TModel> : ViewModelBase
{
    public TModel Model { get; }

    public ViewModelBase(TModel model)
    {
        Model = model;
    }

    protected TValue GetValue<TValue>([CallerMemberName] string? propertyName = null)
    {
        return (TValue)typeof(TModel).GetProperty(propertyName!)!.GetValue(Model)!;
    }

    protected void SetValue<TValue>(TValue value, [CallerMemberName] string? propertyName = null)
    {
        var property = typeof(TModel).GetProperty(propertyName!)!;
        var currentValue = property.GetValue(Model)!;
        if (!Equals(currentValue, value))
        {
            property.SetValue(Model, value);
            RaisePropertyChanged(propertyName);
        }
    }
}
