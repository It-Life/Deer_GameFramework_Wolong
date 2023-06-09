using UnityEditor;

public static class DeerMenu
{
    [MenuItem("DeerTools/Settings/Deer Global Settings", priority = 100)]
    public static void OpenDeerSettings() => SettingsService.OpenProjectSettings("Deer/DeerGlobalSettings");
    [MenuItem("DeerTools/Settings/Deer HybridCLR Settings", priority = 110)]
    public static void OpenDeerHybridSettings() => SettingsService.OpenProjectSettings("Deer/DeerHybridSettings");
    [MenuItem("DeerTools/Settings/Path Settings", priority = 120)]
    public static void OpenDeerPathSettings() => SettingsService.OpenProjectSettings("Deer/DeerPathSetting");
    [MenuItem("DeerTools/Settings/Auto Bind Global Setting", priority = 200)]
    public static void OpenAutoBindGlobalSettings() => SettingsService.OpenProjectSettings("Deer/AutoBindGlobalSetting");
    [MenuItem("DeerTools/Settings/Console Window Filter Toolbar", priority = 300)]
    public static void OpenConsoleWindowFilterToolbar() => SettingsService.OpenProjectSettings("Deer/ConsoleWindowFilterToolbar");
}