using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SetupSpotlightPuzzle : MonoBehaviour
{
    [Inject] AudioManager audioManager;
    [Inject] CommonSounds sounds;
    private void OnEnable()
    {
        audioManager.PlayOneShot(sounds.TakePaper);
    }

    public class Factory : PlaceholderFactory<SetupSpotlightPuzzle> { }

}
