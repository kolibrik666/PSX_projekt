using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SanityScreenAnimation : MonoBehaviour
{
    [SerializeField] Material _sanityMat;
    const string _intensity = "_vignetteIntensity";

    Sequence _sequence;
    Sequence _sequence2;

    private void OnEnable()
    {
        _sanityMat.SetFloat(_intensity, 0);
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
        _sequence = DOTween.Sequence()
            .Append(_sanityMat.DOFloat(1f, _intensity, 1f).SetEase(Ease.Linear));
    }
    public void StopAnim() 
    {        
        _sequence2 = DOTween.Sequence()
            .Append(_sanityMat.DOFloat(0f, _intensity, 1f).SetEase(Ease.Linear))
            .AppendInterval(1f)
            .AppendCallback(() => {
                KillAnim();
                //_gameObject.SetActive(false);
            });
    }

}

