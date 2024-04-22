using UnityEngine;
public static class Constants
{
    public static string SettingsPath => $"{Application.persistentDataPath}/{SettingsFileName}";
    public static string SettingsFileName => $"PlayerSettings.txt";
}
