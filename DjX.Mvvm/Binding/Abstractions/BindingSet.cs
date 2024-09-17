using System.ComponentModel;

namespace DjX.Mvvm.Binding.Abstractions;

public abstract class BindingSet<TTargetType>(INotifyPropertyChanged sourceObject, string sourceMemberName, TTargetType targetObject, string targetMemberName) : IDisposable
    where TTargetType : class
{
    protected readonly INotifyPropertyChanged SourceObject = sourceObject;
    protected readonly string SourceMemberName = sourceMemberName;
    protected readonly TTargetType TargetObject = targetObject;
    protected readonly string TargetMemberName = targetMemberName;

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}