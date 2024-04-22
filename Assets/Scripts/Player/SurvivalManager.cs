using System;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class SurvivalManager : MonoBehaviour
{
    [Inject] GameSetupData _gameSetupData;
    [Inject] GameRunData _gameRunData;
    [Inject] Serializer _serializer;

    [SerializeField] FirstPersonController _playerScript;
    [SerializeField] SanityScreenAnimation _sanityScreenAnimation;
    [SerializeField] BlurScreenAnimation _blurScreenAnimation;

    private float _elapsedTime = 0f;
    private float _tick = 0f;

    private float _interval = 0.1f; // 1 milisekunda
    private float _intervalSecond = 1f; // 1 sekunda

    int _daysSurvived = 0;
    float _difficultyMultiplier;
    float _sprintingMultiplier = 0.35f;
    float _chasedMultiplier = 0.50f;
    float _ticksWithoutChange = 0.1f;
    bool _isChased = false;

    int _intervalExhaustion = 80; // 8 sekund
    float _currentExhaustion = 0f;
    const int _MIN_SPRINT = 20;
    bool _blurAnimCalled = false;
    bool _isAlive => _playerScript.IsAlive;
    bool _isSprinting => _playerScript.IsSprinting;
    bool _sanityCalled = false;

    Difficulty _difficulty;

    public static event Action OnPlayerDeath; // can send trough <> enum / type of death so proper anim will be launched
    public static event Action OnValueChange;
    public static event Action<bool> CanSprint;

    private void OnEnable()
    {
        _gameSetupData = _serializer.LoadData<GameSetupData>(_serializer.FileSaveName);
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
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _interval)
        {
            Tick();
            _tick += 0.1f;
            _elapsedTime = 0f;
        }
    }

    void Tick()
    {
        if (!_isAlive) return;
        CheckSprint();
        if (_tick >= _intervalSecond)
        {
            Saturation();
            CheckLife();
            Sanity();
            _tick = 0f;
        }
    }
    public void SetChaseState(bool b)
    {
        _isChased = b;
    }
    private void CheckSprint()
    {      
        if (_isSprinting) _currentExhaustion++;
        else if(_currentExhaustion > 0) _currentExhaustion = Mathf.Max(0, _currentExhaustion - 1);

        if (!_blurAnimCalled && _currentExhaustion >= 60)
        {
            _blurAnimCalled = true;
            _blurScreenAnimation.PlayAnim(ResetAnim);
        }
        else if (_blurAnimCalled && _currentExhaustion <= 60 && _playerScript.CanSprint)
        {
            _blurScreenAnimation.StopAnim();
            _blurAnimCalled = false;
        }

        if (_currentExhaustion >= _intervalExhaustion) CanSprint?.Invoke(false);
        else if (_currentExhaustion <= _MIN_SPRINT) CanSprint?.Invoke(true);
    }
    private void ResetAnim(bool b)
    {
        _blurAnimCalled = b;
    }
    public void CheckLife(bool killed = false)
    {
        if (killed || _gameRunData.Saturation <= 0)
        {
            _gameSetupData = _serializer.CreateInitialGameData<GameSetupData>();
            _gameSetupData.FirstLaunch = false;
            _serializer.SaveData(_gameSetupData,_serializer.FileSaveName);
            OnPlayerDeath?.Invoke();
            _sanityScreenAnimation.StopAnim();
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

    void Saturation()
    {
        float borderToGet = RandomNumGen.Range(0f, 1f);
        if (borderToGet < (_isSprinting ? _sprintingMultiplier + _difficultyMultiplier : _difficultyMultiplier * (_ticksWithoutChange * RandomNumGen.Range(1, 10) / 100f)))
        {
            if (_gameRunData.Saturation > 0) _gameRunData.Saturation -= 1;
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
            if (_gameRunData.Sanity > 0) _gameRunData.Sanity -= 1;
            OnValueChange?.Invoke();
        }
    }
}
