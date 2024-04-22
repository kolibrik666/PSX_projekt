using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public int Difficulty;
    public int Language;
    public int Resolution;
    public int ScreenMode;
    public bool Headbob;
    public bool Vsync;
}
public class MainMenuSettings : MonoBehaviour
{
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] InGameMenu _inGameMenu;
    [SerializeField] MainMenuSettings _mainMenuSettings;

    [SerializeField] GameplaySettings _gameplaySettings;
    [SerializeField] GameObject _controlSettings;
    [SerializeField] DisplaySettings _displaySettings;
    [SerializeField] GameObject _audioSettings;

    [SerializeField] TextButton _gameplayButton;
    [SerializeField] TextButton _controlButton;
    [SerializeField] TextButton _displayButton;
    [SerializeField] TextButton _audioButton;

    GameSetupData _gameSetupData;
    public GameSetupData GameSetupData => _gameSetupData;

    public static event Action SettingsChanged;

    public bool IsIngame => _isIngame;
    bool _isIngame;
    bool _isOpened;
    public void Setup(GameSetupData gameSetupData, bool IsIngame)
    {
        _gameSetupData = gameSetupData;
        _isIngame = IsIngame;


    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _isOpened)
        {
            SettingsChanged?.Invoke();
            gameObject.SetActive(false);
            if (_isIngame) _inGameMenu.ShowMenu(true);
            else _mainMenu.ShowMenu(true);
        }
    }
    private void OnEnable()
    {
        _isOpened = true;
        _gameplayButton.onClick.AddListener(ShowGameplaySetting);
        _controlButton.onClick.AddListener(ShowControlSetting);
        _displayButton.onClick.AddListener(ShowDisplaySetting);
        _audioButton.onClick.AddListener(ShowAudioSetting);
    }
    private void OnDisable()
    {
        _gameplayButton.onClick.RemoveListener(ShowGameplaySetting);
        _controlButton.onClick.RemoveListener(ShowControlSetting);
        _displayButton.onClick.RemoveListener(ShowDisplaySetting);
        _audioButton.onClick.RemoveListener(ShowAudioSetting);
    } 

    public void SetupSettings()
    {
        if (!TryLoadSettings(out var data))
        {
            File.Create(Constants.SettingsPath).Dispose();
            SettingsData dataDefault = GetDefaultData();
            SaveToFile(dataDefault);
            _gameplaySettings.SetFromFile(dataDefault);
            _displaySettings.SetFromFile(dataDefault);
            return;
        }

        _gameplaySettings.SetFromFile(data);
        _displaySettings.SetFromFile(data);
         
    }
    private void ShowMainMenuSettings(bool b)
    {
        _mainMenuSettings.gameObject.SetActive(b);
        _isOpened = b;
    }
    private void ShowGameplaySetting()
    {
        _gameplaySettings.gameObject.SetActive(true);
        _gameplaySettings.Setup();
       // _gameplaySettings.SetFromFile();

        ShowMainMenuSettings(false);
    }
    private void ShowControlSetting()
    {
        _controlSettings.SetActive(true);
        ShowMainMenuSettings(false);
    }
    private void ShowDisplaySetting()
    {
        _displaySettings.gameObject.SetActive(true);
        _displaySettings.Setup();
        //_displaySettings.SetFromFile();

        ShowMainMenuSettings(false);
    }
    private void ShowAudioSetting()
    {
        _audioSettings.SetActive(true);
        ShowMainMenuSettings(false);
    }
    public bool TryLoadSettings(out SettingsData data)
    {
        data = null;

        if (!File.Exists(Constants.SettingsPath)) return false;

        try
        {
            data = JsonUtility.FromJson<SettingsData>(File.ReadAllText(Constants.SettingsPath));
        }
        catch (Exception)
        {
            Debug.LogWarning("Cannot load SettingsData");
            return false;
        }
        return data != null;
    }
    private SettingsData GetDefaultData()
    {
        SettingsData defaultSettings = new SettingsData
        {
            Difficulty = default,
            Language = default,
            Resolution = default,
            ScreenMode = default,
            Headbob = default,
            Vsync = default,
        };

        defaultSettings.Difficulty = 0;
        Languages language = Application.systemLanguage switch
        {
            SystemLanguage.Spanish => Languages.Spanish,
            SystemLanguage.Slovak => Languages.Slovak,
            SystemLanguage.German => Languages.German,
            SystemLanguage.French => Languages.French,
            SystemLanguage.ChineseSimplified => Languages.ChineseSimplified,
            SystemLanguage.Chinese => Languages.ChineseSimplified,
            SystemLanguage.ChineseTraditional => Languages.ChineseSimplified,
            SystemLanguage.Russian => Languages.Russian,
            SystemLanguage.Polish => Languages.Polish,
            _ => Languages.English,
        };
        defaultSettings.Language = (int)language;
        defaultSettings.Resolution = Mathf.Max(Array.IndexOf(Screen.resolutions, Screen.currentResolution), 0);
        defaultSettings.ScreenMode = (int)Screen.fullScreenMode;
        defaultSettings.Headbob = true;
        defaultSettings.Vsync = false;

        return defaultSettings;
    }
    
    public void SaveToFile(SettingsData data)
    {
        File.WriteAllText(Constants.SettingsPath, JsonUtility.ToJson(new SettingsData()
        {
            Difficulty = data.Difficulty,
            Language = data.Language,
            Resolution = data.Resolution,
            ScreenMode = data.ScreenMode,
            Headbob = data.Headbob,
            Vsync = data.Vsync,
        }));
    }   
}
