using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SurvivalManager : MonoBehaviour
{
    [Inject] GameSetupData _gameSetupData;

    [SerializeField] Slider _hungerBarSlider;
    [SerializeField] Slider _sanityBarSlider;
    [SerializeField] FirstPersonController _playerScript;

    private float _elapsedTime = 0f;
    private float _interval = 1f; // 1 sekunda

    int _hungerValue;
    int _sanityValue;
    int _daysSurvived;
    float _difficultyMultiplier;
    float _sprintingMultiplier = 0.40f;
    float _ticksWithoutChange = 0.1f;

    Difficulty _difficulty;

    private void OnEnable()
    {
        _difficulty = _gameSetupData.Difficulty;
        var result = _difficulty switch
        {
            Difficulty.Normal => 1f,
            Difficulty.Nightmare => 1.2f,
            Difficulty.Insane => 1.4f,
            _ => 0.1f
        };
        _difficultyMultiplier = result;
    }

    private void OnDisable()
    {

    }
    void Update()
    {
        _elapsedTime += Time.unscaledDeltaTime;
        if (_elapsedTime >= _interval)
        {
            Tick();
            _elapsedTime = 0f;
        }
    }

    void Tick()
    {
        Saturation();
    }
    void Saturation()
    {
        float hungerDecreaseChance = _difficultyMultiplier * (_ticksWithoutChange * RandomNumGen.Range(1, 10) / 100f);
        float borderToGet = RandomNumGen.Range(0f, 1f);
        if (borderToGet < (_playerScript.IsSprinting ? _sprintingMultiplier : hungerDecreaseChance))
        {
            Debug.Log("minus 1");
            _hungerBarSlider.value -= 1;
            _ticksWithoutChange = 0;
        }
        else _ticksWithoutChange += 0.1f;
    }
}
