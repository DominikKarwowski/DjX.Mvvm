namespace DjX.Mvvm.Navigation.Abstractions;
public interface INavigable
{
    event Action<Type>? NavigationRequested;
}
