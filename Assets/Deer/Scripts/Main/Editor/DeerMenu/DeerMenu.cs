using UnityEditor;

public static class DeerMenu
{
    [MenuItem("MyTools/Settings/DeerSettings", priority = 100)]
    public static void OpenDeerSettings() => SettingsService.OpenProjectSettings("Deer/DeerSettings");
    [MenuItem("MyTools/Settings/AutoBindGlobalSetting", priority = 200)]
    public static void OpenAutoBindGlobalSettings() => SettingsService.OpenProjectSettings("Deer/AutoBindGlobalSetting");
    [MenuItem("MyTools/Settings/ConsoleWindowFilterToolbar", priority = 300)]
    public static void OpenConsoleWindowFilterToolbar() => SettingsService.OpenProjectSettings("Deer/ConsoleWindowFilterToolbar");
}