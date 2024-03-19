using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SanityScreenAnimation : MonoBehaviour
{
    [SerializeField] CanvasGroup _bg;
    [SerializeField] GameObject _gameObject;

    Sequence _sequence;
    Sequence _sequence2;

    private void OnEnable()
    {
        _bg.alpha = 0f;
    }
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
    public void PlayAnim()
    {
        _gameObject.SetActive(true);
        _sequence = DOTween.Sequence()
            .Append(_bg.DOFade(1, 1f).SetEase(Ease.Linear));
    }
    public void StopAnim() 
    {        
        _sequence2 = DOTween.Sequence()
            .Append(_bg.DOFade(0, 1f).SetEase(Ease.Linear))
            .AppendInterval(1f)
            .AppendCallback(() => {
                KillAnim();
                _gameObject.SetActive(false);
            });
    }

}

