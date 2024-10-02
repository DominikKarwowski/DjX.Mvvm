namespace DjX.Mvvm.Core.Threading.Abstractions;

public interface IMainThreadDispatcher
{
    Action<T> ScheduleOnMainThread<T>(Action<T> callback);

    void RunOnMainThread(Action callback);
}
