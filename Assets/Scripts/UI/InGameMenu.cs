using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Button _continueButton;
    [SerializeField] Button _settingsButton;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Button _quitButton;

    [SerializeField] MainMenuSettings _settingsMenu;
    [SerializeField] GameObject _mainMenuBlocker;
    [SerializeField] GameObject _backPopupInfo;
    public bool IsOpened => _isOpened;
    public bool SettingsOpened => _settingsOpened;
    bool _isOpened = false;
    bool _settingsOpened = false;

    [Inject] GameSetupData _gameSetupData;

    public event Action OnMenuClosed;

    public void Open()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnMenuClosed?.Invoke();
        _isOpened = true;
        gameObject.SetActive(_isOpened); //can be animation
        _backPopupInfo.SetActive(_isOpened);
        Time.timeScale = 0;
    }
    public void Close()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        OnMenuClosed?.Invoke();
        _isOpened = false;
        _backPopupInfo.SetActive(_isOpened);
        gameObject.SetActive(_isOpened); //can be animation
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        _settingsMenu.SetupSettings();
        _continueButton.onClick.AddListener(Close);
        _settingsButton.onClick.AddListener(OpenSettings);
        _mainMenuButton.onClick.AddListener(OpenMainMenu);
        _quitButton.onClick.AddListener(QuitGame);
    }
    private void OnDisable()
    {
        _continueButton.onClick.RemoveListener(Close);
        _settingsButton.onClick.RemoveListener(OpenSettings);
        _mainMenuButton.onClick.RemoveListener(OpenMainMenu);
        _quitButton.onClick.RemoveListener(QuitGame);
    }
    public void ShowMenu(bool b)
    {
        _settingsOpened = !b;
        _mainMenuBlocker.SetActive(!b);
        if (!b) _settingsMenu.gameObject.SetActive(false);
    }
    void QuitGame()
    {
        Application.Quit();
    }
    void OpenMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        Time.timeScale = 1;
    }
    void OpenSettings()
    {
        ShowMenu(false);
        _settingsMenu.gameObject.SetActive(true);
        _settingsMenu.Setup(_gameSetupData, true);
    }
}
