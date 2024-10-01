using DjX.Mvvm.Core.Threading.Abstractions;

namespace DjX.Mvvm.Platforms.Android.Threading;
public class MainThreadDispatcher : IMainThreadDispatcher
{
    public void RunOnMainThread(Action callback) => throw new NotImplementedException();
    public Action<T> ScheduleOnMainThread<T>(Action<T> callback) => throw new NotImplementedException();
}
