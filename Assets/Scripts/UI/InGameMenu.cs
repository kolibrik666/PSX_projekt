using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Button _continueButton;
    [SerializeField] Button _settingsButton;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Button _quitButton;
    public bool IsOpened => _isOpened;
    bool _isOpened = false;
    GameSetupData _gameSetupData;
    FirstPersonController _firstPersonController;

    public event Action OnMenuClosed;
    public void Setup(FirstPersonController firstPersonController)
    {
        _firstPersonController = firstPersonController;
    }
    public void Open()
    {
        OnMenuClosed?.Invoke();
        _isOpened = true;
        gameObject.SetActive(_isOpened); //can be animation
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Close()
    {
        OnMenuClosed?.Invoke();
        _isOpened = false;
        gameObject.SetActive(_isOpened); //can be animation
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
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
    void QuitGame()
    {
        Application.Quit();
    }
    void OpenMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    void OpenSettings()
    {

    }
}
