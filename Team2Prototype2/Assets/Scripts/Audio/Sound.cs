/*****************************************************************************
// File Name :         Sound.cs
// Author :            Cade R. Naylor
// Creation Date :     February 25, 2024
//
//Based on:             Brackeys Audio Manager
//Tutorial video:       https://youtu.be/6OT43pvUyfY

// Brief Description :  Creates the settings for audio sources in each sound in 
AudioManager
*****************************************************************************/
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip audClip;

    [Range(0, 1)]
    public float clipVolume;
    [Range(0.1f, 3)]
    public float clipPitch;
    public bool canLoop;

    [Range(-1, 1)]
    public float panStereo;
    [Range(0, 1)]
    public float spacialBlend;
    public int minSoundDistance;
    public int maxSoundDistance;

    [HideInInspector]
    public AudioSource source;

    public bool isMusic;
}

