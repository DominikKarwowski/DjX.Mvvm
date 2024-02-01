namespace DjX.Mvvm.Platforms;
public static class LocalStorage
{
    public static string Path =>
#if WINDOWS10_0_17763_0_OR_GREATER
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#else
        AppDomain.CurrentDomain.BaseDirectory;
#endif
}
