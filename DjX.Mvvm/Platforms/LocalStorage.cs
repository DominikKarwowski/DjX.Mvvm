namespace DjX.Mvvm.Platforms;
public static class LocalStorage
{
    public static string Path =>
#if WINDOWS10_0_17763_0_OR_GREATER
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#elif ANDROID21_0_OR_GREATER
        Microsoft.Maui.Storage.FileSystem.Current.AppDataDirectory;
#else
        AppDomain.CurrentDomain.BaseDirectory;
#endif
}
