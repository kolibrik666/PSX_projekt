using System;
using UnityEngine;
using Zenject;

public class SurvivalManager : MonoBehaviour
{
    [Inject] GameSetupData _gameSetupData;
    [Inject] GameRunData _gameRunData;
    [SerializeField] FirstPersonController _playerScript;
    [SerializeField] SanityScreenAnimation _sanityScreenAnimation;

    private float _elapsedTime = 0f;
    private float _interval = 1f; // 1 sekunda

    int _daysSurvived = 0;
    float _difficultyMultiplier;
    float _sprintingMultiplier = 0.35f;
    float _chasedMultiplier = 0.50f;
    float _ticksWithoutChange = 0.1f;
    bool _isChased = false;
    bool _isAlive => _playerScript.IsAlive;
    bool _sanityCalled = false;

    Difficulty _difficulty;

    public static event Action OnPlayerDeath; // can send trough <> enum / type of death so proper anim will be launched
    public static event Action OnValueChange;
    private void OnEnable()
    {
        _difficulty = _gameSetupData.Difficulty;
        _gameRunData.Saturation = _gameSetupData.Saturation;
        _gameRunData.Sanity = _gameSetupData.Sanity;
        _gameRunData.SurvivedDays = _daysSurvived;

        var result = _difficulty switch
        {
            Difficulty.Normal => 0.1f,
            Difficulty.Nightmare => 0.12f,
            Difficulty.Insane => 0.14f,
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
        if (!_isAlive) return;
        Saturation();
        CheckLife();
        Sanity();
    }
    public void CheckLife(bool killed = false)
    {
        if (killed || _gameRunData.Saturation <= 0)
        {
            _daysSurvived = 0;
            _gameSetupData.SurvivedDays = _daysSurvived;
            OnPlayerDeath?.Invoke();
        }
        if (_gameRunData.Sanity <= 10 && !_sanityCalled)
        {
            _sanityCalled = true;
            _sanityScreenAnimation.PlayAnim();
        }
        else if (_gameRunData.Sanity > 10 && _sanityCalled) 
        { 
            _sanityScreenAnimation.StopAnim();
            _sanityCalled = false;
        }
    }

    public void SetChaseState(bool b)
    {
        _isChased = b;
    }
    void Saturation()
    {
        float borderToGet = RandomNumGen.Range(0f, 1f);
        if (borderToGet < (_playerScript.IsSprinting ? _sprintingMultiplier + _difficultyMultiplier : _difficultyMultiplier * (_ticksWithoutChange * RandomNumGen.Range(1, 10) / 100f)))
        {
            _gameRunData.Saturation -= 1;
            _ticksWithoutChange = 0;
            OnValueChange?.Invoke();
        }
        else _ticksWithoutChange += 0.1f;
    }
    void Sanity()
    {
        float borderToGet = RandomNumGen.Range(0f, 1f);
        if (borderToGet < (_isChased ? _chasedMultiplier + _difficultyMultiplier : _difficultyMultiplier * (RandomNumGen.Range(1, 100) / 100f)))
        {
            _gameRunData.Sanity -= 1;
            OnValueChange?.Invoke();
        }
    }
}
