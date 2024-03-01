using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlidingWallAnimation : MonoBehaviour
{
    [SerializeField] GameObject _slidingWall;
    [SerializeField] float _duration = 1f;
    [SerializeField] bool _animOnStart = true;

    Sequence _sequence;
    private void OpenDoor()
    {
        
    }
    private void OnEnable()
    {
        if (_animOnStart) OpenDoor();
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}
