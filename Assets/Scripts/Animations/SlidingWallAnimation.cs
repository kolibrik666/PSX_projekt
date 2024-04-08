using UnityEngine;
using DG.Tweening;
using System;
using Zenject;

public class SlidingWallAnimation : MonoBehaviour
{
    [Inject] AudioManager _audioManager;

    [SerializeField] Sound _sound;
    [SerializeField] GameObject _slidingWall;
    [SerializeField] float _duration = 3.5f;
    [SerializeField] float _targetPosY = -8.769f;

    Sequence _sequence;
    private void SetPosY(int y)
    {
        _slidingWall.transform.DOMoveY(y, 0f);
    }

    public void OpenDoor(Action finishCallback = null)
    {
        _audioManager.PlayOneShot(_sound);
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
