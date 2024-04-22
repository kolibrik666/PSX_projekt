using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour, ITranslatable
{
    [Inject] AudioManager _audioManager;
    [Inject] Serializer _serializer;
    [Inject] GameSetupData _gameSetupData;

    [SerializeField] TextButton _newGameButton;
    [SerializeField] TextButton _continueButton;
    [SerializeField] Button _settingsButton;
    [SerializeField] TextButton _quitButton;
    [SerializeField] Sound _music;

    [SerializeField] MainMenuSettings _settingsMenu;
    [SerializeField] GameObject _mainMenuBlocker;
    [SerializeField] GameObject _backPopupInfo;

    [SerializeField] TextMeshProUGUI _continue;
    [SerializeField] TextMeshProUGUI _new;
    [SerializeField] TextMeshProUGUI _settings;
    [SerializeField] TextMeshProUGUI _quit;
    [SerializeField] TextMeshProUGUI _daysSurv;
    [SerializeField] TextMeshProUGUI _daysSurvInfo;
    [SerializeField] TextMeshProUGUI _howToPlay;
    [SerializeField] TextMeshProUGUI _howToControl;

    private void Awake()
    {
        TextDatabase.Load();
        RandomNumGen.Init();
        _gameSetupData = _serializer.LoadData<GameSetupData>(_serializer.FileSaveName);
        if (_gameSetupData.FirstLaunch) _continueButton.interactable = false;
        _daysSurv.text = _gameSetupData.SurvivedDays.ToString();
    }
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        _settingsMenu.SetupSettings();
        _newGameButton.onClick.AddListener(NewGame);
        _continueButton.onClick.AddListener(ContinueGame);
        _settingsButton.onClick.AddListener(ShowSettings);
        _quitButton.onClick.AddListener(QuitGame);
        _audioManager.Transition(new List<Sound>() { _music });

        TextDatabase.OnCurrentLanguageChanged += UpdateTexts;
        UpdateTexts();
    }
    private void OnDisable()
    {
        _newGameButton.onClick.RemoveListener(NewGame);
        _continueButton.onClick.RemoveListener(ContinueGame);
        _settingsButton.onClick.RemoveListener(ShowSettings);
        _quitButton.onClick.AddListener(QuitGame);
        TextDatabase.OnCurrentLanguageChanged -= UpdateTexts;
    }
    void QuitGame()
    {
        Application.Quit();
    }
    public void ShowMenu(bool b)
    {
        _backPopupInfo.SetActive(!b);
        _mainMenuBlocker.SetActive(!b);
        if (!b) _settingsMenu.gameObject.SetActive(false);
    }
    void ShowSettings()
    {
        ShowMenu(false);
        _settingsMenu.gameObject.SetActive(true);
        _settingsMenu.Setup(_gameSetupData, false);
    }
    void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadSceneAsync("GameScene");
        _gameSetupData.FirstLaunch = true;
        _serializer.SaveData(_gameSetupData, _serializer.FileSaveName);
    }
    void NewGame()
    {
        _gameSetupData = _serializer.CreateInitialGameData<GameSetupData>();
        _gameSetupData.FirstLaunch = true;
        _serializer.SaveData(_gameSetupData, _serializer.FileSaveName);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void UpdateTexts()
    {
        _continue.text = TextDatabase.Translate("Continue_game_button");
        _new.text = TextDatabase.Translate("New_game_button");
        _settings.text = TextDatabase.Translate("Settings_button");
        _quit.text = TextDatabase.Translate("Quit_game_button");
        _howToPlay.text = TextDatabase.Translate("How_to_play");
        _howToControl.text = TextDatabase.Translate("How_to_control");
        _daysSurvInfo.text = TextDatabase.Translate("Days_survived_info");
    }
}
