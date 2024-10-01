using DjX.Mvvm.Core.Messenger.Abstractions;
using DjX.Mvvm.Core.Threading.Abstractions;
using System.Collections.Concurrent;

namespace DjX.Mvvm.Core.Messenger;

public class MessagingService(IMainThreadDispatcher mainThreadDispatcher) : IMessagingService
{
    private readonly object locker = new();
    private readonly ConcurrentDictionary<Type, List<object>> subscriptions = new();

    public void Publish<TMessage>(TMessage message)
    {
        var actions = this.subscriptions.GetOrAdd(
            typeof(TMessage), []);

        try
        {
            var actionsCopy = actions.ToList();
            actionsCopy.ForEach(action =>
            {
                if (action is Action<TMessage> a)
                {
                    a(message);
                }
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
        var actions = this.subscriptions.GetOrAdd(
            typeof(TMessage), []);

        lock (this.locker)
        {
            actions.Add(callback);
        }
    }

    public void SubscribeOnMainThread<TMessage>(Action<TMessage> callback)
    {
        var callbackEnqueuedOnMainThread = mainThreadDispatcher.ScheduleOnMainThread(callback);

        var actions = this.subscriptions.GetOrAdd(
            typeof(TMessage), []);

        lock (this.locker)
        {
            actions.Add(callbackEnqueuedOnMainThread);
        }
    }

    public void Unsubscribe<TMessage>(Action<TMessage> callback)
    {
        var actions = this.subscriptions.GetOrAdd(
            typeof(TMessage), []);

        lock (this.locker)
        {
            _ = actions.Remove(callback);
        }
    }
}
