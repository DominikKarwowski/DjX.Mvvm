namespace DjX.Mvvm.Navigation.Abstractions;
public interface INavigable
{
    event Action<Type, Type?, object?>? NavigationToRequested;
    event Action? NavigationCloseRequested;
}
