using UnityEditor;

public static class DeerMenu
{
    [MenuItem("DeerTools/Settings/Deer Settings", priority = 100)]
    public static void OpenDeerSettings() => SettingsService.OpenProjectSettings("Deer/DeerSettings");
    [MenuItem("DeerTools/Settings/Path Settings", priority = 101)]
    public static void OpenDeerPathSettings() => SettingsService.OpenProjectSettings("Deer/DeerPathSetting");
    [MenuItem("DeerTools/Settings/Auto Bind Global Setting", priority = 200)]
    public static void OpenAutoBindGlobalSettings() => SettingsService.OpenProjectSettings("Deer/AutoBindGlobalSetting");
    [MenuItem("DeerTools/Settings/Console Window Filter Toolbar", priority = 300)]
    public static void OpenConsoleWindowFilterToolbar() => SettingsService.OpenProjectSettings("Deer/ConsoleWindowFilterToolbar");
}