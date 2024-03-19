using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
[CreateAssetMenu(menuName = "Sounds/AudioManager", order = 1)]

public class AudioManager : ScriptableObject, IInitializable, IDisposable
{
    [Inject] AudioPlayer.Pool _playerPool;

    List<AudioPlayer> _activePlayers = new();
    Dictionary<Sound, Queue<AudioClip>> _clipQueues = new();

    public IReadOnlyDictionary<Sound, Queue<AudioClip>> ClipQueues => _clipQueues;

    public void Dispose()
    {
    }

    public void Initialize()
    {
        _activePlayers.Clear();
        _clipQueues.Clear();
    }

    public void PlayOneShot(Sound sound)
    {
        SpawnPlayer(sound);
    }

    public AudioPlayer Play(Sound sound)
    {
        return SpawnPlayer(sound);
    }

    public void Transition(Sound sound, bool fadeIn = true)
    {
        StopSounds();
        SpawnPlayer(sound, fadeIn, randomizeClips: true);
    }

    public void Transition(List<Sound> sounds, bool fadeIn = true)
    {
        StopSounds();
        foreach (var sound in sounds)
        {
            SpawnPlayer(sound, fadeIn, randomizeClips: true);
        }
    }

    public void StopSounds()
    {
        for (int i = _activePlayers.Count - 1; i >= 0; i--)
        {
            _activePlayers[i].FadeOut();
        }
    }

    public void StopPlayer(AudioPlayer player)
    {
        _activePlayers.Remove(player);
        _playerPool.Despawn(player);
    }

    public AudioPlayer SourceWithSound(Sound sound)
    {
        return _activePlayers.Find(item => item.Sound == sound);
    }

    private AudioPlayer SpawnPlayer(Sound sound, bool fadeIn = false, bool randomizeClips = false)
    {
        if (!_clipQueues.ContainsKey(sound)) _clipQueues.Add(sound, new());
        if (randomizeClips || _clipQueues[sound].Count <= 0)
        {
            var audioClips = new List<AudioClip>(sound.Clips);
            audioClips.Shuffle();
            _clipQueues[sound] = new(audioClips);
        }

        AudioPlayer source = _playerPool.Spawn(sound, fadeIn);
        _activePlayers.Add(source);
        return source;
    }
}
