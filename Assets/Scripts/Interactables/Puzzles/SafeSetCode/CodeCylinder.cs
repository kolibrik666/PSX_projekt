using DG.Tweening;
using System;
using UnityEngine;

public class CodeCylinder : Interactable
{
    float _rotation = 0;
    float _duration = 0.15f;
    bool _canInteract = true;
    bool _isCompleted = false;
    int _codeNumber;
    Sequence _sequence;
    public static event Action<bool> OnCompleted;
    public void Setup(int codeNumber)
    {
        _codeNumber = codeNumber;
        if(codeNumber == 0) _isCompleted = true;
    }
    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
    private void RotateCylinder()
    {
        _rotation -= 36;
        if (_rotation <= -360) _rotation = 0;
        _sequence = DOTween.Sequence()
            .Append(transform.DORotateQuaternion(Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, _rotation), _duration))
            .AppendCallback(() => {
                _canInteract = true;
            });
    }
    public override void OnInteract()
    {
        if (_canInteract) RotateCylinder();
        if ((_rotation / -36) == _codeNumber && !_isCompleted)
        {
            _isCompleted = true;
            OnCompleted?.Invoke(true);
            Debug.Log("DOKOCENY" + _codeNumber);
        }
        else if (_isCompleted)
        {
            _isCompleted = false;
            OnCompleted?.Invoke(false);
        }
        _canInteract = false;
    }

    public override void OnLoseFocus()
    {

    }
    public override void OnFocus()
    {

    }
    

}
