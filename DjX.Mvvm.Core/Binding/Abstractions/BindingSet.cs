using System.ComponentModel;
using System.Reflection;

namespace DjX.Mvvm.Core.Binding.Abstractions;

public abstract class BindingSet<TTargetType, TTargetMemberInfo>(
    INotifyPropertyChanged sourceObject,
    PropertyInfo sourceMemberInfo,
    TTargetType targetObject,
    TTargetMemberInfo targetMemberInfo) : IDisposable
    where TTargetType : class
    where TTargetMemberInfo : MemberInfo
{
    protected readonly INotifyPropertyChanged SourceObject = sourceObject;
    protected readonly PropertyInfo SourceMemberInfo = sourceMemberInfo;
    protected readonly TTargetType TargetObject = targetObject;
    protected readonly TTargetMemberInfo TargetMemberInfo = targetMemberInfo;

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}