using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class ExitDoor : Interactable
{
    [Inject] GameSetupData _gameSetupData;
    [Inject] GameRunData _gameRunData;
    [Inject] Serializer _serializer;

    [SerializeField] CanvasGroup _bg;
    [SerializeField] GameObject _bgGo;
    bool _canOpen = false;
    Sequence _seq;
    private void OnEnable()
    {
        KeyExit.OnKeyPickedUp += Unlock;
    }
    private void OnDisable()
    {
        KeyExit.OnKeyPickedUp -= Unlock;
        _seq?.Kill();
        _seq = null;
    }
    private void Unlock()
    {
        _canOpen = true;
        _bg.alpha = 0f;
    }
    public void BlackoutAnimation()
    {
        _bgGo.SetActive(true);
        _seq = DOTween.Sequence()
            .SetLoops(0)
            .Append(_bg.DOFade(1, 1))
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                _gameSetupData.SurvivedDays++;
                _gameSetupData.Sanity = _gameRunData.Sanity;
                _gameSetupData.Saturation = _gameRunData.Saturation;
                _serializer.SaveData(_gameSetupData, _serializer.FileSaveName);
                int seed = RandomNumGen.Range(0, int.MaxValue);
                RandomNumGen.Init(seed);
                SceneManager.LoadSceneAsync("GameScene");
            });
    }
    public override void OnInteract()
    {
        if (_canOpen) BlackoutAnimation();
    }
    public override void OnHold()
    {
    }
    public override void OnFocus()
    {
    }
    public override void OnLoseFocus()
    {
    }
}
