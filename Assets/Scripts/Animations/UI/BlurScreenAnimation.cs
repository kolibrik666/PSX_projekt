using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurScreenAnimation : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvas;

    Sequence _sequence;
    Sequence _sequence2;

    float _timeToFade = 0.5f;

    private void OnDisable()
    {
        KillAnim();
    }
    private void KillAnim()
    {
        _sequence.Kill();
        _sequence2.Kill();
        _sequence = null;
        _sequence2 = null;
    }
    public void PlayAnim(Action<bool> callback = null)
    {
        _sequence = DOTween.Sequence()
            .Append(_canvas.DOFade(1f, _timeToFade).SetEase(Ease.OutSine))
            .AppendInterval(0.2f)
            .Append(_canvas.DOFade(0.5f, _timeToFade).SetEase(Ease.InSine))
            .AppendInterval(0.2f)
            .Append(_canvas.DOFade(1f, _timeToFade).SetEase(Ease.OutSine))
            .AppendInterval(0.2f)
            .Append(_canvas.DOFade(0.2f, _timeToFade).SetEase(Ease.InSine))
            .AppendInterval(0.2f)
            .Append(_canvas.DOFade(1f, _timeToFade).SetEase(Ease.OutSine))
            .AppendInterval(0.2f)
            .Append(_canvas.DOFade(0f, _timeToFade).SetEase(Ease.InSine))
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                callback?.Invoke(false);
            });
        
    }
    public void StopAnim()
    {
        _sequence2 = DOTween.Sequence()
            .Append(_canvas.DOFade(0f, 1f).SetEase(Ease.Linear))
            .AppendInterval(3f)
            .AppendCallback(() => {
                KillAnim();
            });
    }
}
