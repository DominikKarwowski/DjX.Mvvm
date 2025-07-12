using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Core.Binding.Abstractions;

public abstract class BindingSet<TTargetType, TTargetMemberInfo>(
    TTargetType targetObject,
    TTargetMemberInfo targetMemberInfo,
    INotifyPropertyChanged sourceObject,
    PropertyInfo sourcePropertyInfo) : IDisposable
    where TTargetType : class
    where TTargetMemberInfo : MemberInfo
{
    protected readonly TTargetType TargetObject = targetObject;
    protected readonly TTargetMemberInfo TargetMemberInfo = targetMemberInfo;
    protected readonly INotifyPropertyChanged SourceObject = sourceObject;
    protected readonly PropertyInfo SourcePropertyInfo = sourcePropertyInfo;

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}