using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HUD : MonoBehaviour
{
    [Inject] GameSetupData _gameSetupData;
    [Inject] GameRunData _gameRunData;
    [Inject] AudioManager _audioManager;

    [SerializeField] InGameMenu _ingameMenu;
    [SerializeField] FirstPersonController _firstPersonController;
    [SerializeField] Slider _hungerBarSlider;
    [SerializeField] Slider _sanityBarSlider;
    [SerializeField] TextMeshProUGUI _survivedDays;
    [SerializeField] Sound _music;

    public static event Action OnChangeControl;
    private void Start()
    {
        _audioManager.Transition(_music);   
    }
    private void OnEnable()
    {
        _survivedDays.text = _gameSetupData.SurvivedDays.ToString();
        _hungerBarSlider.value = _gameSetupData.Saturation;
        _sanityBarSlider.value = _gameSetupData.Sanity;
        Consumable.OnValueChanged += UpdateSliders;
        SurvivalManager.OnValueChange += UpdateSliders;
        _ingameMenu.OnMenuClosed += ChangeControl;
    }
    private void OnDisable()
    {
        Consumable.OnValueChanged -= UpdateSliders;
        SurvivalManager.OnValueChange -= UpdateSliders;
        _ingameMenu.OnMenuClosed -= ChangeControl;
    }
    void Update()
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
        if (!_firstPersonController.IsAlive) return;
        if (_ingameMenu.IsOpened) _ingameMenu.Close();
        else _ingameMenu.Open();
    }

}
