#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.UI.Dispatching;
#endif

namespace DjK.BackupTool.ViewModel.Platforms;

public class MainThreadScheduler
{
#if WINDOWS10_0_17763_0_OR_GREATER
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
#endif

    public Action<T> ScheduleOnMainThread<T>(Action<T> callback)
    {
#if WINDOWS10_0_17763_0_OR_GREATER
        return (param) =>
        {
            if (_dispatcherQueue.HasThreadAccess)
            {
                callback(param);
            }
            else
            {
                _dispatcherQueue.TryEnqueue(() => callback(param));
            }
        };
#else
        return callback;
#endif
    }

    public void RunOnMainThread(Action callback)
    {
#if WINDOWS10_0_17763_0_OR_GREATER
        if (_dispatcherQueue.HasThreadAccess)
        {
            callback();
        }
        else
        {
            _dispatcherQueue.TryEnqueue(() => callback());
        }
#else
        callback();
#endif
    }
}
