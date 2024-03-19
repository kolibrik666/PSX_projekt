using DG.Tweening;
using ModestTree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class PlayerDeathAnimation : MonoBehaviour
{
    [Inject] AudioManager _audioManager;
    [Inject] CommonSounds _commonSounds;
    [SerializeField] GameObject _camera;
    [SerializeField] GameObject _deathScreen;
    [SerializeField] CanvasGroup _bloodyScreen;
    [SerializeField] CanvasGroup _blackStripe;
    [SerializeField] CanvasGroup _text;
    [SerializeField] Button _restartButton;
    [SerializeField] CanvasGroup _hint;

    Sequence _sequence;
    private void OnEnable()
    {
        _bloodyScreen.alpha = 0f;
        _blackStripe.alpha = 0f;
        _text.alpha = 0f;
        _hint.alpha = 0f;
    }
    public void Play()
    {
        _deathScreen.SetActive(true);
        _sequence = DOTween.Sequence()
            .SetUpdate(true)
            .SetLoops(0)
            .Append(_camera.transform.DOMoveY(0.3f, 1f).SetEase(Ease.OutQuad))
            .Join(_camera.transform.DORotate(new Vector3(-60f, 0f, 0f), 0.5f, RotateMode.Fast).SetRelative())
            .Join(_bloodyScreen.DOFade(1f, 0.5f))
            .AppendCallback(() =>
            {
                _blackStripe.DOFade(1f, 1f);
                _text.DOFade(1f, 2f);
            })
            .AppendInterval(1f)
            .AppendCallback(() => {
                _restartButton.onClick.AddListener(OnKeyPress);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            })
            .AppendInterval(2f)
            .AppendCallback(() =>
            {
                _hint.DOFade(1f, 1f);
            });
    }
    private void OnKeyPress()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(currentScene.name);
    }
    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(OnKeyPress);
        _sequence?.Kill();
        _sequence = null;
    }
}
