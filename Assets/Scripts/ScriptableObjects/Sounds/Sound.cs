using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Sound")]
public class Sound : ScriptableObject
{
    public List<AudioClip> Clips;
    public AudioMixerGroup MixerGroup;
    [Header("Ambient")]
    public float Volume = 1f;
    public bool Loop = false;
    [Header("Loop")]
    public bool IsClipSwitchingLoop = true;
    public bool OneShot = true;
    public bool Ambient = false;
    [Header("Ambient")]
    public float MinVolume = 0.5f;
    [Header("Ambient")]
    public float MaxVolume = 1f;

    public AudioClip Clip()
    {
        return Clips[RandomNumGen.Range(0, Clips.Count)];
    }
}