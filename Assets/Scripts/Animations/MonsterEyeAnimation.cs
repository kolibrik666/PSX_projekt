using DG.Tweening;
using UnityEngine;

public class MonsterEyeAnimation : MonoBehaviour
{
    [SerializeField] GameObject _monsterEye;
    private Transform _player;
    float _duration = 0.8f;

    Sequence _sequence;
    private void OnEnable()
    {
        RollEye();
    }
    private void RollEye()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .SetLoops(-1)
            .SetUpdate(true)
            .Append(_monsterEye.transform.DOLocalRotate(new Vector3(10, 10, 0), _duration).SetEase(Ease.InOutQuad))
            .Append(_monsterEye.transform.DOLocalRotate(new Vector3(10, -10, 0), _duration).SetEase(Ease.InOutQuad))
            .Append(_monsterEye.transform.DOLocalRotate(new Vector3(-5, 0, 0), _duration).SetEase(Ease.InOutQuad))
            .Append(_monsterEye.transform.DOLocalRotate(new Vector3(10, 0, 0), _duration).SetEase(Ease.InOutQuad));
    }
    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}
