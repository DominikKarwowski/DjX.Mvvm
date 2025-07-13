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
        public DelegateCommand NonAsyncCommand { get; } = new(() => { });
        public AsyncDelegateCommand NonGenericAsyncCommand { get; } = new(async () => await Task.Run(() => { }));
        public AsyncDelegateCommand<string> GenericAsyncCommand { get; } = new(async _ => await Task.Run(() => { }));
        public AsyncDelegateCommand<int> AnotherGenericAsyncCommand { get; } = new(async _ => await Task.Run(() => { }));
    }
}
