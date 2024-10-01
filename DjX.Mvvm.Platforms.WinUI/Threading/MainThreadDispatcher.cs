using DjX.Mvvm.Core.Threading.Abstractions;
using Microsoft.UI.Dispatching;

namespace DjX.Mvvm.Platform.WinUI.Threading;

public class MainThreadDispatcher : IMainThreadDispatcher
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    public void RunOnMainThread(Action callback)
    {
        if (this.dispatcherQueue.HasThreadAccess)
        {
            callback();
        }
        else
        {
            _ = this.dispatcherQueue.TryEnqueue(() => callback());
        }
    }

    public Action<T> ScheduleOnMainThread<T>(Action<T> callback)
        => (param) =>
        {
            if (this.dispatcherQueue.HasThreadAccess)
            {
                callback(param);
            }
            else
            {
                _ = this.dispatcherQueue.TryEnqueue(() => callback(param));
            }
        };
}
