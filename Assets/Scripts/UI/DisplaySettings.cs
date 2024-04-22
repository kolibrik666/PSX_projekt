using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DisplaySettings : MonoBehaviour
{
    [SerializeField] MainMenuSettings _mainMenuSettings;
    [SerializeField] TMP_Dropdown _resolution;
    [SerializeField] TMP_Dropdown _screenMode;
    [SerializeField] TextButton _vsync;
    [SerializeField] TMP_Text _vsyncText;

    public TMP_Dropdown Resolution => _resolution;
    public TMP_Dropdown ScreenMode => _screenMode;

    List<(int width, int height, int refreshRate)> _resolutionNumbers = new();

    bool _isOpened;
    public void Setup()
    {
        _isOpened = true;
        _resolution.ClearOptions();
        _screenMode.ClearOptions();

        List<TMP_Dropdown.OptionData> res = new();
        List<TMP_Dropdown.OptionData> screenModes = new();

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            _resolutionNumbers.Add((Screen.resolutions[i].width, Screen.resolutions[i].height, (int)Screen.resolutions[i].refreshRateRatio.value));
            res.Add(new($"{Screen.resolutions[i].width}x{Screen.resolutions[i].height} {Mathf.RoundToInt((float)Screen.resolutions[i].refreshRateRatio.value)}Hz"));
        }
        _resolution.AddOptions(res);

        screenModes.Add(new(TextDatabase.Translate(FullScreenMode.ExclusiveFullScreen.ToString())));
        screenModes.Add(new(TextDatabase.Translate(FullScreenMode.FullScreenWindow.ToString())));
        screenModes.Add(new(TextDatabase.Translate(FullScreenMode.MaximizedWindow.ToString())));
        screenModes.Add(new(TextDatabase.Translate(FullScreenMode.Windowed.ToString())));
        _screenMode.AddOptions(screenModes);

        UpdateUI();
    }
    private void OnEnable()
    {
        _resolution.onValueChanged.AddListener(SetResolution);
        _screenMode.onValueChanged.AddListener(SetScreenMode);
        _vsync.onClick.AddListener(() => SetVSync(QualitySettings.vSyncCount == 1 ? true : false));
    }
    private void OnDisable()
    {
        _resolution.onValueChanged.RemoveListener(SetResolution);
        _screenMode.onValueChanged.RemoveListener(SetScreenMode);
        _vsync.onClick.RemoveListener(() => SetVSync(QualitySettings.vSyncCount == 1 ? true : false));
        _isOpened = false;
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _isOpened)
        {
            gameObject.SetActive(false);
            _mainMenuSettings.gameObject.SetActive(true);
            _mainMenuSettings.SaveToFile(dataSettings);
        }
    }
    private void UpdateUI()
    {
        _resolution.value = Mathf.Clamp(dataSettings.Resolution, 0, Screen.resolutions.Length - 1);
        _screenMode.value = dataSettings.ScreenMode;
        if (dataSettings.Vsync) _vsyncText.text = TextDatabase.Translate("On").ToString();
        else _vsyncText.text = TextDatabase.Translate("Off").ToString();

    }
    private void SetResolution(int index)
    {
        Resolution res;
        if (index >= Screen.resolutions.Length || index < 0) res = Screen.resolutions[^1];
        else res = Screen.resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode, res.refreshRateRatio);
        dataSettings.Resolution = index;
        UpdateUI();
    }
    private void SetScreenMode(int index)
    {
        Screen.fullScreenMode = (FullScreenMode)index;
        dataSettings.ScreenMode = (int)(FullScreenMode)index;
        UpdateUI();
    }
    private void SetVSync(bool b)
    {
        QualitySettings.vSyncCount = !b ? 1 : 0;
        dataSettings.Vsync = b;
        UpdateUI();
    }
    public void SetFromFile(SettingsData data)
    {
        SetResolution(data.Resolution);
        SetScreenMode(data.ScreenMode);
        SetVSync(data.Vsync);
        dataSettings = data;
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
