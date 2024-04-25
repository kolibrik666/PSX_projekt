using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AudioPlayer : MonoBehaviour
{
    const float MINIMAL_DESPAWN_TIME_SECONDS = 0.5f;

    [SerializeField] AudioSource _source;

    [Inject] AudioManager _audioManager;

    Sound _sound;
    float _time = -1f;
    float _fadeTime = 0.7f;
    bool _isPlaying = false;
    bool _despawnCalled = false;
    bool _toDespawn;

    public Sound Sound => _sound;

    public void Reinit(Sound sound, bool fadeIn)
    {
        _sound = sound;
        _time = -1;
        _despawnCalled = _toDespawn = false;
        if (!sound.MixerGroup) Debug.LogWarning($"{sound.name}_ mixer group not assigned");
        _source.outputAudioMixerGroup = sound.MixerGroup;
        _source.volume = sound.Volume;
        AudioClip clipToPlay = _audioManager.ClipQueues[sound].Dequeue();
        if (sound.OneShot)
        {
            _source.PlayOneShot(clipToPlay, sound.Volume);
            _time = Mathf.Max(new float[] { clipToPlay.length, MINIMAL_DESPAWN_TIME_SECONDS, _time });
            _isPlaying = true;
        }
        else
        {
            _source.clip = clipToPlay;
            _source.loop = _sound.Loop && !_sound.IsClipSwitchingLoop;
            _source.volume = fadeIn ? 0 : sound.Volume;
            if (!_source.loop)
            {
                _time = Mathf.Max(new float[] { clipToPlay.length, MINIMAL_DESPAWN_TIME_SECONDS, _time });
                _isPlaying = true;
            }
            _source.Play();
            if (fadeIn) FadeIn();
        }
    }

    private void Update()
    {
        if (_toDespawn || _despawnCalled) return;

        if (_time > 0) _time -= Time.unscaledDeltaTime;
        else if (_isPlaying || (!Mathf.Approximately(_time, -1f) && gameObject.activeInHierarchy))
        {
            _isPlaying = false;
            _time = -1;
            if (_sound.Loop && _sound.IsClipSwitchingLoop)
            {
                FadeOut();
                _audioManager.Play(_sound);
                return;
            }

            Despawn();
        }
    }

    public void FadeOut()
    {
        _toDespawn = true;
        _source.DOFade(0, _fadeTime)
            .SetUpdate(true)
            .SetEase(Ease.OutExpo)
            .OnComplete(Despawn);
    }

    public void FadeIn()
    {
        _source.DOFade(_sound.Volume, _fadeTime)
            .SetUpdate(true)
            .SetEase(Ease.InExpo);
    }

    public async void StopLoop(Sound endSound) 
    {
        if (_despawnCalled || _toDespawn) return;
        _toDespawn = true;
        _source.loop = false;
        await Task.Delay(Mathf.RoundToInt((_source.clip.length - _source.time) * 1000));
        _audioManager.PlayOneShot(endSound);
        Despawn();
    }

    public void UpdateVolume(float volume)
    {
        _source.volume = volume;
    }

    public void Despawn()
    {
        if (_despawnCalled) return;
        _despawnCalled = true;
        _audioManager.StopPlayer(this);
    }

    public class Pool : MonoMemoryPool<Sound, bool, AudioPlayer>
    {
        protected override void Reinitialize(Sound sound, bool fadeIn, AudioPlayer item)
        {
            item.Reinit(sound, fadeIn);
        }
    }
}
