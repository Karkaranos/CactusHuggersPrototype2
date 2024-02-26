/*****************************************************************************
// File Name :         MasterVolumeSettings.cs
// Author :            Cade R. Naylor
// Creation Date :     February 25, 2024
//
// Based on:            Master Audio Mixer by AIAdev
// Tutorial Video:      https://youtube.com/shorts/_m6nTQOMFl0?feature=share
//
// Brief Description :  Handles the game's overall volume
*****************************************************************************/
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolumeSettings : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioMixer masterMixer;

    /// <summary>
    /// Start is called before the first frame update. Gets the player's saved volume.
    /// </summary>
    void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SavedMasterVolume", 100));
    }

    /// <summary>
    /// Sets the current master volume
    /// </summary>
    /// <param name="_value">new volume value as a float</param>
    public void SetVolume(float _value)
    {
        if (_value < 1)
        {
            _value = .001f;
        }

        RefreshSlider(_value);
        PlayerPrefs.SetFloat("SavedMasterVolume", _value);
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(_value / 100) * 20f);

    }

    /// <summary>
    /// Takes volume input from a slider
    /// </summary>
    public void SetVolumeFromSlider()
    {
        SetVolume(volumeSlider.value);
    }

    /// <summary>
    /// Updates slider with the current volume
    /// </summary>
    /// <param name="_value"></param>
    public void RefreshSlider(float _value)
    {
        volumeSlider.value = _value;
    }
}
