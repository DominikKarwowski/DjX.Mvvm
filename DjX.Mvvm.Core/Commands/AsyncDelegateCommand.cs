using System.Windows.Input;
using DjX.Mvvm.Core.Commands.Abstractions;
using DjX.Mvvm.Core.Commands.Helpers;

namespace DjX.Mvvm.Core.Commands;

//https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/april/async-programming-patterns-for-asynchronous-mvvm-applications-commands
//https://johnthiriet.com/mvvm-going-async-with-async-command/

public sealed class AsyncDelegateCommand<T> : ICommandBase, IDisposable
{
    private readonly Func<T?, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposedValue;

    public AsyncDelegateCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    public bool CanExecute(T? parameter) => this._canExecute is null || this._canExecute(parameter);
    
    public async Task ExecuteAsync(T? parameter)
    {
        await this._semaphore.WaitAsync();
        
        try
        {
            await this._execute(parameter);
        }
        finally
        {
            _ = this._semaphore.Release();
        }
    }

    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this._semaphore.Dispose();
            }

            this._disposedValue = true;
        }
    }

    public void Dispose() => this.Dispose(disposing: true);

    bool ICommand.CanExecute(object? parameter)
    {
        ArgumentValidator.ThrowIfNotOfType<T?>(parameter);
        
        return this.CanExecute((T?)parameter);
    }

    void ICommand.Execute(object? parameter)
    {
        ArgumentValidator.ThrowIfNotOfType<T?>(parameter);
        
        this.ExecuteAsync((T?)parameter).Wait();
    }
}

public sealed class AsyncDelegateCommand : ICommandBase, IDisposable
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposedValue;

    public AsyncDelegateCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute, nameof(execute));
        this._execute = execute;
        this._canExecute = canExecute;
    }
    
    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    public bool CanExecute() => this._canExecute is null || this._canExecute();
    
    public async Task ExecuteAsync()
    {
        await this._semaphore.WaitAsync();
        
        try
        {
            await this._execute();
        }
        finally
        {
            _ = this._semaphore.Release();
        }
    }

    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this._semaphore.Dispose();
            }

            this._disposedValue = true;
        }
    }
    
    public void Dispose() => this.Dispose(disposing: true);
    
    bool ICommand.CanExecute(object? parameter) => this.CanExecute();
    void ICommand.Execute(object? parameter) => this.ExecuteAsync().Wait();
}
