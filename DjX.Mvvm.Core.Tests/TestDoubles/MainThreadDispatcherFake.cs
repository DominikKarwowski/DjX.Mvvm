using DjX.Mvvm.Core.Threading.Abstractions;

namespace DjX.Mvvm.Core.Tests.TestDoubles;
public class MainThreadDispatcherFake : IMainThreadDispatcher
{
    public void RunOnMainThread(Action callback) => callback();
    public Action<T> ScheduleOnMainThread<T>(Action<T> callback) => callback;
}
