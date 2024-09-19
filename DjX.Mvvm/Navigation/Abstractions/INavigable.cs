namespace DjX.Mvvm.Navigation.Abstractions;
public interface INavigable
{
    event Action<Type>? NavigationToRequested;
    event Action? NavigationCloseRequested;
}
