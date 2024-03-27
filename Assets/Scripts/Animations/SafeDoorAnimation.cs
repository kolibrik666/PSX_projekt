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
            .Append(_ring.transform.DORotateQuaternion(Quaternion.Euler(180, _ring.transform.rotation.eulerAngles.y, _ring.transform.rotation.eulerAngles.z), 1f))
            .AppendInterval(0.1f)
            .Append(_door.transform.DORotateQuaternion(_door.transform.rotation * Quaternion.Euler(0, -110, 0), 2f));
    }

    private void OnDisable()
    {
        _sequance.Kill();
        _sequance = null;
    }
}
