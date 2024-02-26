/*****************************************************************************
// File Name :         AudioManager.cs
// Author :            Cade R. Naylor
// Creation Date :     February 25, 2024
//
//Based on:             Brackeys Audio Manager
//Tutorial video:       https://youtu.be/6OT43pvUyfY

// Brief Description :  Creates an array of all sound effects and music then adds an
audio source to each of them. Can call sound by using the name of the audio in the
inspector
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;
    public AudioMixerGroup masterMixer;
    public float musicVolume;

    private bool gameStarted = false;

    private string previousTrack;

    //public Texture2D glassTexture;
    //public CursorMode cursorMode = CursorMode.Auto;
    //public Vector2 hotSpot = Vector2.zero;

    /// <summary>
    /// Start is called before the first frame update. It ensures only one instance
    /// of this script and initializes Sound class
    /// </summary>
    void Start()
    {
        int numAM = FindObjectsOfType<AudioManager>().Length;
        if (numAM != 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound sound in Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();


            sound.source.clip = sound.audClip;
            sound.source.outputAudioMixerGroup = masterMixer;
            sound.source.volume = sound.clipVolume;
            sound.source.pitch = sound.clipPitch;
            sound.source.loop = sound.canLoop;
            sound.source.panStereo = sound.panStereo;
            sound.source.spatialBlend = sound.spacialBlend;
            sound.source.minDistance = sound.minSoundDistance;
            sound.source.maxDistance = sound.maxSoundDistance;
            sound.source.rolloffMode = AudioRolloffMode.Linear;
            sound.source.playOnAwake = false;
        }

    }


    #region Sound Controls

    /// <summary>
    /// Gets the name of a sound and plays it at its point
    /// </summary>
    /// <param name="audioName">the sound name to play</param>
    public void Play(string audioName)
    {
        //Searches through the Sound array until it finds a sound with the 
        //specified name
        Sound sound = Array.Find(Sounds, sound => sound.name == audioName);
        if (sound != null)
        {
            sound.source.Play();
            //print(audioName + " started");
        }
    }

    /// <summary>
    /// Gets the name of a sound and stops it at its point
    /// </summary>
    /// <param name="audioName">the sound name to stop</param>
    public void Stop(string audioName)
    {
        Sound sound = Array.Find(Sounds, sound => sound.name == audioName);
        if (sound != null)
        {
            sound.source.Stop();
            //print(audioName + " stopped");
        }

    }


    /// <summary>
    /// Gets the name of a sound and pauses it at its point
    /// </summary>
    /// <param name="audioName">the sound name to pause</param>
    public void Pause(string audioName)
    {
        Sound sound = Array.Find(Sounds, sound => sound.name == audioName);
        if (sound != null)
        {
            sound.source.Pause();
        }
    }


    /// <summary>
    /// Gets the name of a sound and resumes it at its point
    /// </summary>
    /// <param name="audioName">the sound name to resume</param>
    public void UnPause(string audioName)
    {
        Sound sound = Array.Find(Sounds, sound => sound.name == audioName);
        if (sound != null)
        {
            sound.source.UnPause();
        }
    }

    /// <summary>
    /// Gets the name of a sound and disables its volume when it starts playing
    /// </summary>
    /// <param name="audioName">the sound name to play muted</param>
    public void PlayMuted(string audioName)
    {
        Sound sound = Array.Find(Sounds, sound => sound.name == audioName);
        if (sound != null)
        {
            sound.source.Play();
            sound.source.volume = 0.0f;
        }
    }

    /// <summary>
    /// Gets the name of a sound and mutes it
    /// </summary>
    /// <param name="audioName">the sound name to mute</param>
    public void Mute(string audioName)
    {
        Sound sound = Array.Find(Sounds, sound => sound.name == audioName);
        if (sound != null)
        {
            sound.source.volume = 0.0f;
        }
    }

    /// <summary>
    /// Gets the name of a sound and unmutes it
    /// </summary>
    /// <param name="audioName">the sound name to unmute</param>
    public void Unmute(string audioName)
    {
        Sound sound = Array.Find(Sounds, sound => sound.name == audioName);
        if (sound != null)
        {
            sound.source.volume = musicVolume;
        }
    }



    #endregion

    #region Play

    public void StopAllSounds()
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            Sounds[i].source.Stop();
        }
    }

    private void StopAllMusic()
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i].isMusic)
            {
                Stop(Sounds[i].name);
            }
        }
    }

    #endregion
}
