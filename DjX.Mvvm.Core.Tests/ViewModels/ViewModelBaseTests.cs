using DjX.Mvvm.Core.Commands;
using DjX.Mvvm.Core.Commands.Abstractions;
using DjX.Mvvm.Core.ViewModels;

namespace DjX.Mvvm.Core.Tests.ViewModels;

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
            _ = Assert.ThrowsAsync<ObjectDisposedException>(async () => await sut.NonGenericAsyncCommand.ExecuteAsync());
            _ = Assert.ThrowsAsync<ObjectDisposedException>(async () => await sut.GenericAsyncCommand.ExecuteAsync(string.Empty));
            _ = Assert.ThrowsAsync<ObjectDisposedException>(async () => await sut.AnotherGenericAsyncCommand.ExecuteAsync(default));
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
            this.NonAsyncCommand = new DjXDelegateCommand(() => { /* do nothing */ });
            this.NonGenericAsyncCommand = new DjXAsyncDelegateCommand(async () => await Task.Run(() => { /* do nothing */ }));
            this.GenericAsyncCommand = new DjXAsyncDelegateCommand<string>(async _ => await Task.Run(() => { /* do nothing */ }));
            this.AnotherGenericAsyncCommand = new DjXAsyncDelegateCommand<int>(async _ => await Task.Run(() => { /* do nothing */ }));
        }

    }
}
