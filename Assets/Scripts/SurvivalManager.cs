using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SurvivalManager : MonoBehaviour
{
    [Inject] GameSetupData _gameSetupData;
    [SerializeField] FirstPersonController _playerScript;

    private float _elapsedTime = 0f;
    private float _interval = 1f; // 1 sekunda

    int _hungerValue => _gameSetupData.Saturation;
    int _sanityValue = 100;
    int _daysSurvived = 0;
    float _difficultyMultiplier;
    float _sprintingMultiplier = 0.35f;
    float _ticksWithoutChange = 0.1f;

    Difficulty _difficulty;

    public static event Action OnPlayerDeath; // can send trough <> enum / type of death so proper anim will be launched
    public static event Action OnValueChange;
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
        CheckLife();
    }

    public void CheckLife(bool killed = false)
    {
        if (killed || _hungerValue <= 0)
        {
            _daysSurvived = 0;
            _gameSetupData.SurvivedDays = _daysSurvived;
            OnPlayerDeath?.Invoke();
        }
    }

    void Saturation()
    {
        float borderToGet = RandomNumGen.Range(0f, 1f);
        if (borderToGet < (_playerScript.IsSprinting ? _sprintingMultiplier : _difficultyMultiplier * (_ticksWithoutChange * RandomNumGen.Range(1, 10) / 100f)))
        {
            _gameSetupData.Saturation -= 1;
            _ticksWithoutChange = 0;
            OnValueChange?.Invoke();
        }
        else _ticksWithoutChange += 0.1f;      
    }
}
