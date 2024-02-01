namespace DjK.BackupTool.ViewModel.Messenger.Abstractions;

public interface IMessagingService
{
    void Publish<TMessage>(TMessage message);
    void Subscribe<TMessage>(Action<TMessage> callback);
    void SubscribeOnMainThread<TMessage>(Action<TMessage> callback);
    void Unsubscribe<TMessage>(Action<TMessage> callback);
}
