using System;
using System.Collections.Generic;
using UnityEngine;
public static class TextDatabase
{
    static Dictionary<string, string[]> database = null;
    public static Languages currentLanguage = Languages.English;
    public static Languages CurrentLanguage
    {
        get => currentLanguage;
        set
        {
            if (value == currentLanguage) return;
            currentLanguage = value;
            OnCurrentLanguageChanged?.Invoke();
        }
    }

    public static Dictionary<string, string[]> Database => database;

    public static event Action OnCurrentLanguageChanged = null;
    public static event Action<string> OnTextIDNotFound = null;

    public static void Load()
    {
        if (database != null) return;
        database = new Dictionary<string, string[]>();
        int languageCount = Enum.GetValues(typeof(Languages)).Length;
        string file = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.streamingAssetsPath, "Translations.tsv"));
        string[] lines = file.Split('\n');

        string[] linesAll = new string[lines.Length];
        lines.CopyTo(linesAll, 0);
        for (int i = 0; i < linesAll.Length; i++)
        {
            string[] values = linesAll[i].Trim().Split('\t');
            if (values.Length == 0) continue;
            string key = values[0];
            if (key.Length == 0) continue;

            string[] texts = new string[languageCount];
            for (int j = 0, c = Mathf.Min(values.Length - 1, languageCount); j < c; j++)
            {
                texts[j] = values[j + 1].TrimEnd('·');
            }
            for (int j = values.Length - 1, c = languageCount; j < c; j++)
            {
                texts[j] = "";
            }

            if (database.ContainsKey(key)) Debug.LogError($"'{key}' DUPLICATE Database key");
            else database.Add(key, texts);
        }
    }

    public static string Translate(string textID)
    {
        if (string.IsNullOrEmpty(textID)) return string.Empty;
        if (database.TryGetValue(textID, out string[] text))
        {
            string translation = text[(int)currentLanguage].Replace("\\n", "\n");
            if (string.IsNullOrEmpty(translation)) translation = text[(int)Languages.English].Replace("\\n", "\n");
            return translation;
        }
        else
        {
            OnTextIDNotFound?.Invoke(textID);
            Debug.LogWarning($"<color=orange>{textID} missing !!!</color>");
            return "!!!" + textID + "!!!";
        }
    }

    public static bool TryTranslate(string textID, out string translation)
    {
        translation = textID;
        if (!database.ContainsKey(textID)) return false;
        translation = Translate(textID);
        return !string.IsNullOrEmpty(translation);
    }

    public static string Translate(string textID, string valueReplace)
    {
        return Translate(textID).Replace("{value}", valueReplace);
    }
}
