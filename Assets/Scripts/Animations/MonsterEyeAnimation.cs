using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MonsterEyeAnimation : MonoBehaviour
{
    [SerializeField] GameObject _monsterEye;
    float _duration = 0.3f;

    Sequence _sequence;
    private void OnEnable()
    {
        RollEye();
    }
    private void RollEye(Action finishCallback = null)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .SetLoops(-1)
            .SetUpdate(true)
            .Append(_monsterEye.transform.DOLookAt(new Vector3(3, 0, 0), _duration))
            .AppendInterval(0.5f)
            .Append(_monsterEye.transform.DOLookAt(new Vector3(-3, 0, 0), _duration))
            .AppendInterval(0.5f)
            .Append(_monsterEye.transform.DOLookAt(new Vector3(0, -2, 0), _duration))
            .AppendInterval(0.5f)
            .Append(_monsterEye.transform.DOLookAt(new Vector3(0, 1, 0), _duration))
            .AppendInterval(0.5f);
    }
    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}
