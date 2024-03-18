using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAudioBehaviourScript : MonoBehaviour
{
    private bool musicToggle = true;
    private AudioSource musicSource;
    //private bool soundEffectsToggle = true;
    //was going to allow sound effects to be toggled but it seems quite clunky as they splay on instantiation of particle effect object.

    public bool IsMusicOn()
    {
        return musicToggle;
    }

    public void ToggleMusic()
    {
        musicToggle = !musicToggle;
        if (musicToggle)
        {
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }

    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
    }
}
