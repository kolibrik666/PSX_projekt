using DG.Tweening;
using UnityEngine;

public class SafeDoorAnimation : MonoBehaviour
{
    [SerializeField] GameObject _door;
    [SerializeField] GameObject _ring;

    Sequence _sequance;
    
    public void PlayAnim()
    {
        _sequance = DOTween.Sequence()
            .Append(_ring.transform.DORotateQuaternion(Quaternion.Euler(180, 0, 0), 1f))
            .AppendInterval(0.1f)
            .Append(_door.transform.DORotateQuaternion(Quaternion.Euler(0, -150, 0), 2f));
    }

    private void OnDisable()
    {
        _sequance.Kill();
        _sequance = null;
    }
}
