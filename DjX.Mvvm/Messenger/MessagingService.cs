using DjX.Mvvm.Messenger.Abstractions;
using DjX.Mvvm.Platforms;
using System.Collections.Concurrent;

namespace DjK.BackupTool.ViewModel.Messenger;

public class MessagingService : IMessagingService
{
    private readonly object _locker = new();
    private readonly ConcurrentDictionary<Type, List<object>> _subscriptions = new();
    private readonly MainThreadScheduler _mainThreadRunner;

    public MessagingService(MainThreadScheduler mainThreadRunner)
    {
        _mainThreadRunner = mainThreadRunner;
    }

    public void Publish<TMessage>(TMessage message)
    {
        var actions = _subscriptions.GetOrAdd(
            typeof(TMessage), new List<object>());

        try
        {
            var actionsCopy = actions.ToList();
            actionsCopy.ForEach(action =>
            {
                if (action is Action<TMessage> a) a(message);
            });
            // TODO: check the possibility for parallel execution
            //Parallel.ForEach(actions, action => action(message));
        }
        catch (Exception ex)
        {
            // TODO: add logging
            throw;
        }
    }

    public void Subscribe<TMessage>(Action<TMessage> callback)
    {
        var actions = _subscriptions.GetOrAdd(
            typeof(TMessage), new List<object>());

        lock (_locker)
        {
            actions.Add(callback);
        }
    }

    public void SubscribeOnMainThread<TMessage>(Action<TMessage> callback)
    {
        var callbackEnqueuedOnMainThread = _mainThreadRunner.ScheduleOnMainThread(callback);

        var actions = _subscriptions.GetOrAdd(
            typeof(TMessage), new List<object>());

        lock (_locker)
        {
            actions.Add(callbackEnqueuedOnMainThread);
        }
    }

    public void Unsubscribe<TMessage>(Action<TMessage> callback)
    {
        var actions = _subscriptions.GetOrAdd(
            typeof(TMessage), new List<object>());

        lock (_locker)
        {
            actions.Remove(callback);
        }
    }
}
