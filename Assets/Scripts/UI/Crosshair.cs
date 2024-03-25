using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Sprite _dot;
    [SerializeField] Sprite _interactDot;
    [SerializeField] Sprite _interactedDot;
    
    public void ResetDot() 
    {
        _image.sprite = _dot;
    }
    public void CanInteract()
    {
        _image.sprite = _interactDot;
    }
    public void Interacted()
    {
        _image.sprite = _interactedDot;
        Sequence sequance = DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() => _image.sprite = _dot);
    }
    public class Factory : PlaceholderFactory<Crosshair> { }

}
