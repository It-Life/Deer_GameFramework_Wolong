using UnityEditor;

public static class DeerMenu
{
    [MenuItem("DeerTools/Settings/DeerSettings", priority = 100)]
    public static void OpenDeerSettings() => SettingsService.OpenProjectSettings("Deer/DeerSettings");
    [MenuItem("DeerTools/Settings/AutoBindGlobalSetting", priority = 200)]
    public static void OpenAutoBindGlobalSettings() => SettingsService.OpenProjectSettings("Deer/AutoBindGlobalSetting");
    [MenuItem("DeerTools/Settings/ConsoleWindowFilterToolbar", priority = 300)]
    public static void OpenConsoleWindowFilterToolbar() => SettingsService.OpenProjectSettings("Deer/ConsoleWindowFilterToolbar");
}