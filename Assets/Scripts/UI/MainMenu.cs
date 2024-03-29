using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [Inject] AudioManager _audioManager;
    [Inject] Serializer _serializer;

    [Inject] GameSetupData _gameSetupData;

    [SerializeField] Button _playButton;
    [SerializeField] Button _quitButton;
    [SerializeField] Sound _music;
    [SerializeField] TextMeshProUGUI _daysSurv;

    private void Awake()
    {
        RandomNumGen.Init();
        _gameSetupData = _serializer.LoadData<GameSetupData>(_serializer.FileSaveName);
        _daysSurv.text = _gameSetupData.SurvivedDays.ToString();
    }
    private void OnEnable()
    {
        _playButton.onClick.AddListener(PlayGame);
        _quitButton.onClick.AddListener(QuitGame);
        _audioManager.Transition(_music, true);
    }
    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(PlayGame);
        _quitButton.onClick.AddListener(QuitGame);
    }
    void QuitGame()
    {
        Application.Quit();
    }
    void PlayGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadSceneAsync("GameScene");
    }
}
