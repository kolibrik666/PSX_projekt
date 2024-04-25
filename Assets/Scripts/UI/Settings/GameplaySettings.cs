using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

public class GameplaySettings : MonoBehaviour, ITranslatable
{   
    [SerializeField] MainMenuSettings _mainMenuSettings;
    [SerializeField] TMP_Dropdown _language;
    [SerializeField] TMP_Dropdown _difficulty;
    [SerializeField] TextButton _headbobButton;
    [SerializeField] TextButton _difficultyButton;
    [SerializeField] TextMeshProUGUI _textHeadbob;

    [Inject] GameSetupData _gameSetupData;

    bool _isOpened;
    public void Setup()
    {      
        _isOpened = true;       
        _language.ClearOptions();
        _difficulty.ClearOptions();

        List<TMP_Dropdown.OptionData> languages = new();
        List<TMP_Dropdown.OptionData> difficulties = new();

        if (_mainMenuSettings.IsIngame)
        {
            _difficulty.interactable = false;
            _difficultyButton.interactable = false;
        }

        foreach (var language in Enum.GetValues(typeof(Languages)))
        {
            languages.Add(new(TextDatabase.Translate(language.ToString())));
        }
        _language.AddOptions(languages);

        foreach (var difficulty in Enum.GetValues(typeof(Difficulty)))
        {
            difficulties.Add(new(TextDatabase.Translate(difficulty.ToString())));
        }
        _difficulty.AddOptions(difficulties);

        UpdateUI();
    }

    private void LateUpdate()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && _isOpened)
        {
            gameObject.SetActive(false);
            _mainMenuSettings.gameObject.SetActive(true);
            _mainMenuSettings.SaveToFile(dataSettings);
        }
    }
    private void OnEnable()
    {
        _language.onValueChanged.AddListener(LanguageChange);
        _difficulty.onValueChanged.AddListener(DifficultyChange);
        _headbobButton.onClick.AddListener(HeadbobChange);
        TextDatabase.OnCurrentLanguageChanged += UpdateTexts;
    }
    private void OnDisable()
    {
        _language.onValueChanged.RemoveListener(LanguageChange);
        _difficulty.onValueChanged.RemoveListener(DifficultyChange);
        _headbobButton.onClick.RemoveListener(HeadbobChange);
        TextDatabase.OnCurrentLanguageChanged -= UpdateTexts;
    }

    private void UpdateUI()
    {
        _difficulty.value = dataSettings.Difficulty;
        _language.value = dataSettings.Language;
        if (_gameSetupData.Headbob) _textHeadbob.text = TextDatabase.Translate("On").ToString();
        else _textHeadbob.text = TextDatabase.Translate("Off").ToString();
    }
    private void HeadbobChange()
    {
        _gameSetupData.Headbob = !_gameSetupData.Headbob;
        dataSettings.Headbob = _gameSetupData.Headbob;
        UpdateUI();
    }
    private void LanguageChange(int value)
    {
        TextDatabase.CurrentLanguage = (Languages)value;
        dataSettings.Language = (int)(Languages)value;
        UpdateUI();
    }
    private void DifficultyChange(int value)
    {
        _gameSetupData.Difficulty = (Difficulty)value;
        dataSettings.Difficulty = (int)(Difficulty)value;
        UpdateUI();
    }

    public void SetFromFile(SettingsData data)
    {
        LanguageChange(data.Language);  
        DifficultyChange(data.Difficulty);
        _gameSetupData.Headbob = data.Headbob;
        dataSettings = data;
    }

    public void UpdateTexts()
    {
        Setup();
    }

    SettingsData dataSettings = new SettingsData
    {
        Difficulty = default,
        Language = default,
        Resolution = default,
        ScreenMode = default,
        Headbob = default,
        Vsync = default,
    };
}
