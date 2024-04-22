using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HUD : MonoBehaviour
{
    [Inject] GameStartData _gameStartData;
    [Inject] GameSetupData _gameSetupData;
    [Inject] GameRunData _gameRunData;
    [Inject] AudioManager _audioManager;
    [Inject] Serializer _serializer;
    //[Inject] Crosshair.Factory _crosshairFactory;

    [SerializeField] InGameMenu _ingameMenu;
    [SerializeField] FirstPersonController _firstPersonController;
    [SerializeField] BlackInAnimation _blackInAnimation;
    [SerializeField] Slider _hungerBarSlider;
    [SerializeField] Slider _sanityBarSlider;
    [SerializeField] TextMeshProUGUI _survivedDays;
    [SerializeField] Sound _music;

    [SerializeField] DebugConsole _debugConsole;

    public static event Action OnChangeControl;
    private void Start()
    {
        //_crosshairFactory.Create();
        if (_gameStartData.IsDevDebug)
        {
            _debugConsole.enabled = true;
            _debugConsole.Setup();
        }

        _blackInAnimation.PlayAnim();
    }
    private void OnEnable()
    {
        _gameSetupData = _serializer.LoadData<GameSetupData>(_serializer.FileSaveName);
        _survivedDays.text = _gameSetupData.SurvivedDays.ToString();
        _hungerBarSlider.value = _gameSetupData.Saturation;
        _sanityBarSlider.value = _gameSetupData.Sanity;
        Consumable.OnValueChanged += UpdateSliders;
        SurvivalManager.OnValueChange += UpdateSliders;
        DebugConsole.OnValueChange += UpdateSliders;
        _ingameMenu.OnMenuClosed += ChangeControl;
    }
    private void OnDisable()
    {
        Consumable.OnValueChanged -= UpdateSliders;
        SurvivalManager.OnValueChange -= UpdateSliders;
        DebugConsole.OnValueChange -= UpdateSliders;
        _ingameMenu.OnMenuClosed -= ChangeControl;
    }
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) IngameMenu();
    }
    void ChangeControl()
    {
        OnChangeControl.Invoke();
    }
    private void UpdateSliders()
    {
        _survivedDays.text = _gameSetupData.SurvivedDays.ToString();
        _hungerBarSlider.value = _gameRunData.Saturation;
        _sanityBarSlider.value = _gameRunData.Sanity;
    }
    private void IngameMenu()
    {
        if (_firstPersonController.IsAlive && _ingameMenu.SettingsOpened == false)
        {
            if (_ingameMenu.IsOpened) _ingameMenu.Close();
            else _ingameMenu.Open();
        }
    }

}
