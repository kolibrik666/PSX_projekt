using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Slider _slider;
    [SerializeField] Sprite _dot;
    [SerializeField] Sprite _interactDot;
    [SerializeField] Sprite _interactedDot;
    float _holdInterval = 80f;
    public void ResetDot() 
    {
        _image.enabled = true;
        _image.sprite = _dot;
        _slider.value = 0;
    }
    public void ResetSlider()
    {
        _image.enabled = true;
        _slider.value = 0;
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
    public void Holding(float time)
    {
        _image.enabled = false;
        float normalizedTime = Mathf.Clamp01(time / _holdInterval);
        _slider.value = normalizedTime;
    }
    public class Factory : PlaceholderFactory<Crosshair> { }

}
