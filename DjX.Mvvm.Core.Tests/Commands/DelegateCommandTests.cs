using DjX.Mvvm.Core.Commands;

namespace DjX.Mvvm.Core.Tests.Commands;

[TestFixture]
public class DelegateCommandTests
{
    [Test]
    public void DelegateCommand_throws_exception_if_instantiated_with_a_null_execute_action()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<ArgumentNullException>(() => new DelegateCommand(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Test]
    public void RaiseCanExecuteChanged_invokes_CanExecuteChanged()
    {
        var canExecuteChangedRaised = false;

        var sut = new DelegateCommand(() => { });

        sut.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        sut.RaiseCanExecuteChanged();

        Assert.That(canExecuteChangedRaised, Is.True);
    }

    [Test]
    public void CanExecute_returns_true_if_canExecute_function_is_not_specified_in_DelegateCommand()
    {
        var sut = new DelegateCommand(() => { });

        var result = sut.CanExecute();

        Assert.That(result, Is.True);
    }

    [Test]
    public void CanExecute_evaluates_canExecute_function_if_it_is_specified_in_DelegateCommand()
    {
        var canExecuteWasInvoked = false;

        var sut = new DelegateCommand(
            () => { },
            () =>
            {
                canExecuteWasInvoked = true;
                return true;
            });

        sut.CanExecute();

        Assert.That(canExecuteWasInvoked, Is.True);
    }

    [Test]
    public void Execute_invokes_an_action_delegate_provided_on_DelegateCommand_construction()
    {
        var executeWasInvoked = false;

        var sut = new DelegateCommand(() => executeWasInvoked = true);

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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<ArgumentNullException>(() => new DelegateCommand<string>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Test]
    public void RaiseCanExecuteChanged_invokes_CanExecuteChanged()
    {
        var canExecuteChangedRaised = false;

        var sut = new DelegateCommand<string>(s => { });

        sut.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        sut.RaiseCanExecuteChanged();

        Assert.That(canExecuteChangedRaised, Is.True);
    }

    [Test]
    public void CanExecute_returns_true_if_canExecute_function_is_not_specified_in_DelegateCommand()
    {
        var sut = new DelegateCommand<string>(s => { });

        var result = sut.CanExecute("whatever for the test");

        Assert.That(result, Is.True);
    }

    [Test]
    public void CanExecute_evaluates_canExecute_function_if_it_is_specified_in_DelegateCommand()
    {
        var canExecuteWasInvoked = false;

        var sut = new DelegateCommand<string>(
            s => { },
            s =>
            {
                canExecuteWasInvoked = true;
                return true;
            });

        sut.CanExecute("whatever for the test");

        Assert.That(canExecuteWasInvoked, Is.True);
    }

    [Test]
    public void Execute_invokes_an_action_delegate_provided_on_DelegateCommand_construction()
    {
        var executeWasInvoked = false;

        var sut = new DelegateCommand<string>(s => executeWasInvoked = true);

        sut.Execute("");

        Assert.That(executeWasInvoked, Is.True);
    }
}
