using UnityEngine;
using DG.Tweening;
using System;

public class SlidingWallAnimation : MonoBehaviour
{
    [SerializeField] GameObject _slidingWall;
    [SerializeField] float _duration = 4f;
    [SerializeField] float _targetPosY = -8.769f;

    Sequence _sequence;
    private void SetPosY(int y)
    {
        _slidingWall.transform.DOMoveY(y, 0f);
    }
    public void OpenDoor(Action finishCallback = null)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(_slidingWall.transform.DOMoveY(_targetPosY, _duration))
            .AppendCallback(() =>
            {
                _sequence = null;
                finishCallback?.Invoke();
            });

    }
    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
        SetPosY(0);
    }
}
