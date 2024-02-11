using System.ComponentModel;

namespace DjX.Mvvm.Binding.Abstractions;

public abstract class BindingSet<TTargetType> : IDisposable
    where TTargetType : class
{
    protected readonly INotifyPropertyChanged SourceObject;
    protected readonly string SourceMemberName;
    protected readonly TTargetType TargetObject;
    protected readonly string TargetMemberName;

    private bool disposedValue;

    public BindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, TTargetType targetObject, string targetMemberName)
    {
        SourceObject = sourceObject;
        SourceMemberName = sourceMemberName;
        TargetObject = targetObject;
        TargetMemberName = targetMemberName;
    }

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}