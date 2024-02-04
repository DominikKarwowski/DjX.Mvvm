using DjX.Mvvm.Commands;
using DjX.Mvvm.Commands.Abstractions;
using DjX.Mvvm.ViewModels;

namespace DjX.Mvvm.Tests.ViewModels;

[TestFixture]
public class ViewModelBaseTests
{
    [Test]
    public void OnViewModelDestroy_disposes_all_async_commands()
    {
        var sut = new TestViewModel();

        sut.OnViewModelDestroy();

        Assert.Multiple(() =>
        {
            Assert.ThrowsAsync<ObjectDisposedException>(async () => await sut.NonGenericAsyncCommand.ExecuteAsync());
            Assert.ThrowsAsync<ObjectDisposedException>(async () => await sut.GenericAsyncCommand.ExecuteAsync(string.Empty));
            Assert.ThrowsAsync<ObjectDisposedException>(async () => await sut.AnotherGenericAsyncCommand.ExecuteAsync(default));
        });
    }

    internal class TestViewModel : ViewModelBase
    {
        public IDjXCommand NonAsyncCommand { get; set; }
        public IDjXAsyncCommand NonGenericAsyncCommand { get; set; }
        public IDjXAsyncCommand<string> GenericAsyncCommand { get; set; }
        public IDjXAsyncCommand<int> AnotherGenericAsyncCommand { get; set; }

        public TestViewModel()
        {
            NonAsyncCommand = new DjXDelegateCommand(() => { /* do nothing */ });
            NonGenericAsyncCommand = new DjXAsyncDelegateCommand(async () => await Task.Run(() => { /* do nothing */ }));
            GenericAsyncCommand = new DjXAsyncDelegateCommand<string>(async _ => await Task.Run(() => { /* do nothing */ }));
            AnotherGenericAsyncCommand = new DjXAsyncDelegateCommand<int>(async _ => await Task.Run(() => { /* do nothing */ }));
        }

    }
}
