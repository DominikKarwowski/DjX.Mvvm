#if ANDROID21_0_OR_GREATER
using DjX.Mvvm.Navigation.Abstractions;
using System.Reflection;

namespace DjX.Mvvm.Navigation;
public class AndroidNavigationService(string viewsAssemblyName, string viewsNamespace) : INavigationService
{
    internal Assembly? AndroidExecutingAssembly { get; set; } = null;
    public string ViewsAssemblyName => viewsAssemblyName;
    public string ViewsNamespace => viewsNamespace;
}
#endif