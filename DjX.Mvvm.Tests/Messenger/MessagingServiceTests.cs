using DjX.Mvvm.Messenger;
using DjX.Mvvm.Platforms;

namespace BackupTool.ViewModel.Tests.Messenger;

[TestFixture]
public class MessagingServiceTests
{
    [Test]
    public void Subscribe_registers_callbacks_that_are_invoked_on_Publish_for_the_specified_message_type()
    {
        var callback1executed = false;
        var callback2executed = false;
        var callback3executed = false;

        Action<string> callback1 = msg => callback1executed = true;
        Action<string> callback2 = msg => callback2executed = true;
        Action<int> callback3 = msg => callback3executed = true;

        var sut = new MessagingService(new MainThreadScheduler());

        sut.Subscribe(callback1);
        sut.Subscribe(callback2);
        sut.Subscribe(callback3);

        sut.Publish("");

        Assert.Multiple(() =>
        {
            Assert.That(callback1executed, Is.True);
            Assert.That(callback2executed, Is.True);
            Assert.That(callback3executed, Is.False);
        });
    }

    [Test]
    public void Unsubscribed_callbacks_are_no_longer_invoked_on_Publish()
    {
        var callback1executed = false;
        var callback2executed = false;
        var callback3executed = false;

        Action<string> callback1 = msg => callback1executed = true;
        Action<string> callback2 = msg => callback2executed = true;
        Action<int> callback3 = msg => callback3executed = true;

        var sut = new MessagingService(new MainThreadScheduler());

        sut.Subscribe(callback1);
        sut.Subscribe(callback2);
        sut.Subscribe(callback3);

        sut.Unsubscribe(callback1);

        sut.Publish("");

        Assert.Multiple(() =>
        {
            Assert.That(callback1executed, Is.False);
            Assert.That(callback2executed, Is.True);
            Assert.That(callback3executed, Is.False);
        });
    }

    [Test]
    [Ignore("Is it testable?")]
    public void Callbacks_subscribed_on_the_main_thraad_are_invoked_on_main_application_thread()
    {
        var callbackexecuted = false;

        Action<string> callback = msg => callbackexecuted = true;

        var sut = new MessagingService(new MainThreadScheduler());

        sut.SubscribeOnMainThread(callback);

        sut.Publish("");

        Assert.Fail();
    }
}
