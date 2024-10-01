using System.Runtime.CompilerServices;

namespace DjX.Mvvm.ViewModels;

public abstract class ViewModelBase<TModel>(TModel model) : ViewModelBase
{
    public TModel? Model { get; } = model;

    protected TValue GetValue<TValue>([CallerMemberName] string? propertyName = null)
    {
        return (TValue)typeof(TModel).GetProperty(propertyName!)!.GetValue(this.Model)!;
    }

    protected void SetValue<TValue>(TValue value, [CallerMemberName] string? propertyName = null)
    {
        var property = typeof(TModel).GetProperty(propertyName!)!;
        var currentValue = property.GetValue(this.Model)!;
        if (!Equals(currentValue, value))
        {
            property.SetValue(this.Model, value);
            this.RaisePropertyChanged(propertyName);
        }
    }
}
