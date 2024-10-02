using DjX.Mvvm.Core.Commands;

namespace DjX.Mvvm.Core.Tests.Commands;

[TestFixture]
public class DelegateCommandTests
{
    [Test]
    public void DelegateCommand_throws_exception_if_instantiated_with_a_null_execute_action()
    {
        Action? action = null;
        Action<object?>? actionWithParam = null;

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => new DjXDelegateCommand(action!));
            Assert.Throws<ArgumentNullException>(() => new DjXDelegateCommand(actionWithParam!));
        });
    }

    [Test]
    public void RaiseCanExecuteChanged_invokes_CanExecuteChanged()
    {
        var canExecuteChangedRaised = false;

        var sut = new DjXDelegateCommand(() => { });

        sut.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        sut.RaiseCanExecuteChanged();

        Assert.That(canExecuteChangedRaised, Is.True);
    }

    [Test]
    public void CanExecute_returns_true_if_canExecute_function_is_not_specified_in_DelegateCommand()
    {
        var sut = new DjXDelegateCommand(() => { });

        var result = sut.CanExecute(null);

        Assert.That(result, Is.True);
    }

    [Test]
    public void CanExecute_evaluates_canExecute_function_if_it_is_specified_in_DelegateCommand()
    {
        var canExecuteWasInvoked = false;

        var sut = new DjXDelegateCommand(
            () => { },
            () =>
            {
                canExecuteWasInvoked = true;
                return true;
            });

        sut.CanExecute(null);

        Assert.That(canExecuteWasInvoked, Is.True);
    }

    [Test]
    public void Execute_invokes_an_action_delegate_provided_on_DelegateCommand_construction()
    {
        var executeWasInvoked = false;

        var sut = new DjXDelegateCommand(() => executeWasInvoked = true);

        sut.Execute();

        Assert.That(executeWasInvoked, Is.True);
    }
}

[TestFixture]
public class DelegateCommandOfTTests
{
    [Test]
    public void DelegateCommand_throws_exception_if_instantiated_with_a_null_execute_action()
    {
        Action<string?>? actionWithParam = null;

        Assert.Throws<ArgumentNullException>(() => new DjXDelegateCommand<string>(actionWithParam!));
    }

    [Test]
    public void RaiseCanExecuteChanged_invokes_CanExecuteChanged()
    {
        var canExecuteChangedRaised = false;

        var sut = new DjXDelegateCommand<string>(s => { });

        sut.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        sut.RaiseCanExecuteChanged();

        Assert.That(canExecuteChangedRaised, Is.True);
    }

    [Test]
    public void CanExecute_returns_true_if_canExecute_function_is_not_specified_in_DelegateCommand()
    {
        var sut = new DjXDelegateCommand<string>(s => { });

        var result = sut.CanExecute("");

        Assert.That(result, Is.True);
    }

    [Test]
    public void CanExecute_evaluates_canExecute_function_if_it_is_specified_in_DelegateCommand()
    {
        var canExecuteWasInvoked = false;

        var sut = new DjXDelegateCommand<string>(
            s => { },
            s =>
            {
                canExecuteWasInvoked = true;
                return true;
            });

        sut.CanExecute("");

        Assert.That(canExecuteWasInvoked, Is.True);
    }

    [Test]
    public void Execute_invokes_an_action_delegate_provided_on_DelegateCommand_construction()
    {
        var executeWasInvoked = false;

        var sut = new DjXDelegateCommand<string>(s => executeWasInvoked = true);

        sut.Execute("");

        Assert.That(executeWasInvoked, Is.True);
    }
}
