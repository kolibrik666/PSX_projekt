using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class ExitDoor : Interactable
{
    [Inject] GameSetupData _gameSetupData;
    [Inject] GameRunData _gameRunData;

    [SerializeField] CanvasGroup _bg;
    [SerializeField] GameObject _bgGo;
    bool _canOpen = false;
    Sequence seq;
    private void OnEnable()
    {
        KeyExit.OnKeyPickedUp += Unlock;
        _bg.alpha = 0f;
    }
    private void OnDisable()
    {
        KeyExit.OnKeyPickedUp -= Unlock;
        seq?.Kill();
        seq = null;
    }
    private void Unlock()
    {
        _canOpen = true;
    }
    public void BlackoutAnimation()
    {
        _bgGo.SetActive(true);
        seq = DOTween.Sequence()
            .SetLoops(0)
            .Append(_bg.DOFade(1, 1))
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                _gameSetupData.SurvivedDays++;
                _gameSetupData.Sanity = _gameRunData.Sanity;
                _gameSetupData.Saturation = _gameRunData.Saturation;
                SceneManager.LoadSceneAsync("GameScene");
            });
    }
    public override void OnInteract()
    {
        if (_canOpen) BlackoutAnimation();
    }
    public override void OnFocus()
    {
    }
    public override void OnLoseFocus()
    {
    }
}
