#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.UI.Dispatching;
#endif

namespace DjX.Mvvm.Platforms;

public class MainThreadScheduler
{
#if WINDOWS10_0_17763_0_OR_GREATER
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
#endif

    public Action<T> ScheduleOnMainThread<T>(Action<T> callback)
    {
#if WINDOWS10_0_17763_0_OR_GREATER
        return (param) =>
        {
            if (this.dispatcherQueue.HasThreadAccess)
            {
                callback(param);
            }
            else
            {
                this.dispatcherQueue.TryEnqueue(() => callback(param));
            }
        };
#else
        return callback;
#endif
    }

    public void RunOnMainThread(Action callback)
    {
#if WINDOWS10_0_17763_0_OR_GREATER
        if (this.dispatcherQueue.HasThreadAccess)
        {
            callback();
        }
        else
        {
            this.dispatcherQueue.TryEnqueue(() => callback());
        }
#else
        callback();
#endif
    }
}
