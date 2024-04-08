using DG.Tweening;
using System;
using UnityEngine;

public class BlackInAnimation : MonoBehaviour
{
    [SerializeField] GameObject _bgGo;
    [SerializeField] CanvasGroup _bg;
    Sequence _seq;
    public void PlayAnim()
    {
        _bg.alpha = 1.0f;
        _seq = DOTween.Sequence()
            .Append(_bg.DOFade(0, 1))
            .AppendCallback(() =>
            {
                _bgGo.SetActive(false);
            });
    }
    private void OnDisable()
    {
        _seq?.Kill();
        _seq = null;
    }
}
