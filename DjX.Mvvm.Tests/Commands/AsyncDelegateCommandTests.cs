using DjX.Mvvm.Commands;
using System.Diagnostics;

namespace BackupTool.ViewModel.Tests.Commands;

[TestFixture]
public class AsyncDelegateCommandTests
{
    [Test]
    public void DelegateCommand_throws_exception_if_instantiated_with_a_null_execute_action()
    {
        Func<Task>? asyncAction = null;
        Func<object?, Task>? asyncActionWithParam = null;

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => new AsyncDelegateCommand(asyncAction!));
            Assert.Throws<ArgumentNullException>(() => new AsyncDelegateCommand(asyncActionWithParam!));
        });
    }

    [Test]
    public void RaiseCanExecuteChanged_invokes_CanExecuteChanged()
    {
        var canExecuteChangedRaised = false;

        var sut = new AsyncDelegateCommand(() => Task.CompletedTask);

        sut.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        sut.RaiseCanExecuteChanged();

        Assert.That(canExecuteChangedRaised, Is.True);
    }

    [Test]
    public void CanExecute_returns_true_if_canExecute_function_is_not_specified_in_DelegateCommand()
    {
        var sut = new AsyncDelegateCommand(() => Task.CompletedTask);

        var result = sut.CanExecute(null);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CanExecute_evaluates_canExecute_function_if_it_is_specified_in_DelegateCommand()
    {
        var canExecuteWasInvoked = false;

        var sut = new AsyncDelegateCommand(
            () => Task.CompletedTask,
            () =>
            {
                canExecuteWasInvoked = true;
                return true;
            });

        sut.CanExecute(null);

        Assert.That(canExecuteWasInvoked, Is.True);
    }

    [Test]
    public async Task ExecuteAsync_invokes_an_action_delegate_provided_on_DelegateCommand_construction()
    {
        var executeWasInvoked = false;

        var sut = new AsyncDelegateCommand(() =>
        {
            executeWasInvoked = true;
            return Task.CompletedTask;
        });

        await sut.ExecuteAsync();

        Assert.That(executeWasInvoked, Is.True);
    }

    [Test]
    public async Task ExecuteAsync_executes_only_one_action_delegate_at_a_time()
    {
        var sut = new AsyncDelegateCommand(() => Task.Delay(25));

        var sw = new Stopwatch();

        sw.Start();

        var execution1 = sut.ExecuteAsync();
        var execution2 = sut.ExecuteAsync();
        var execution3 = sut.ExecuteAsync();
        var execution4 = sut.ExecuteAsync();
        await Task.WhenAll(execution1, execution2, execution3, execution4);

        sw.Stop();

        Assert.That(sw.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(100));
    }
}

[TestFixture]
public class AsyncDelegateCommandOfTTests
{
    [Test]
    public void DelegateCommand_throws_exception_if_instantiated_with_a_null_execute_action()
    {
        Func<string, Task>? asyncAction = null;
        Func<object?, Task>? asyncActionWithParam = null;

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => new AsyncDelegateCommand<string>(asyncAction!));
            Assert.Throws<ArgumentNullException>(() => new AsyncDelegateCommand<string>(asyncActionWithParam!));
        });
    }

    [Test]
    public void RaiseCanExecuteChanged_invokes_CanExecuteChanged()
    {
        var canExecuteChangedRaised = false;

        var sut = new AsyncDelegateCommand<string>(s => Task.CompletedTask);

        sut.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        sut.RaiseCanExecuteChanged();

        Assert.That(canExecuteChangedRaised, Is.True);
    }

    [Test]
    public void CanExecute_returns_true_if_canExecute_function_is_not_specified_in_DelegateCommand()
    {
        var sut = new AsyncDelegateCommand<string>(s => Task.CompletedTask);

        var result = sut.CanExecute(null);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CanExecute_evaluates_canExecute_function_if_it_is_specified_in_DelegateCommand()
    {
        var canExecuteWasInvoked = false;

        var sut = new AsyncDelegateCommand<string>(
            s => Task.CompletedTask,
            s =>
            {
                canExecuteWasInvoked = true;
                return true;
            });

        sut.CanExecute(null);

        Assert.That(canExecuteWasInvoked, Is.True);
    }

    [Test]
    public async Task ExecuteAsync_invokes_an_action_delegate_provided_on_DelegateCommand_construction()
    {
        var executeWasInvoked = false;

        var sut = new AsyncDelegateCommand<string>(s =>
        {
            executeWasInvoked = true;
            return Task.CompletedTask;
        });

        await sut.ExecuteAsync("");

        Assert.That(executeWasInvoked, Is.True);
    }

    [Test]
    public async Task ExecuteAsync_executes_only_one_action_delegate_at_a_time()
    {
        var sut = new AsyncDelegateCommand<string>(s => Task.Delay(25));

        var sw = new Stopwatch();

        sw.Start();

        var execution1 = sut.ExecuteAsync("");
        var execution2 = sut.ExecuteAsync("");
        var execution3 = sut.ExecuteAsync("");
        var execution4 = sut.ExecuteAsync("");
        await Task.WhenAll(execution1, execution2, execution3, execution4);

        sw.Stop();

        Assert.That(sw.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(100));
    }
}
