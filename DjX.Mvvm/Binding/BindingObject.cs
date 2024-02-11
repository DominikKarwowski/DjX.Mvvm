#if ANDROID21_0_OR_GREATER
using Android.Views;
using System.ComponentModel;

namespace DjX.Mvvm.Binding;
public sealed class AndroidBindingObject : IDisposable
{
    List<AndroidPropertyBindingSet> PropertyBindings { get; } = [];
    List<AndroidEventBindingSet> EventBindings { get; } = [];

    public void Dispose()
    {
        PropertyBindings.ForEach(pb => pb.Dispose());
        EventBindings.ForEach(eb => eb.Dispose());
    }
}


public sealed class AndroidPropertyBindingSet : BindingSet<View>
{
    private bool disposedValue;

    public AndroidPropertyBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }
}

public sealed class AndroidEventBindingSet : BindingSet<View>
{
    private bool disposedValue;

    public AndroidEventBindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, View targetObject, string targetMemberName)
        : base(sourceObject, sourceMemberName, targetObject, targetMemberName)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }
}

#endif

public abstract class BindingSet<TTargetType> : IDisposable
    where TTargetType : class
{
    private readonly INotifyPropertyChanged sourceObject;
    private readonly string sourceMemberName;
    private readonly TTargetType targetObject;
    private readonly string targetMemberName;

    private bool disposedValue;

    public BindingSet(INotifyPropertyChanged sourceObject, string sourceMemberName, TTargetType targetObject, string targetMemberName)
    {
        this.sourceObject = sourceObject;
        this.sourceMemberName = sourceMemberName;
        this.targetObject = targetObject;
        this.targetMemberName = targetMemberName;
    }

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}