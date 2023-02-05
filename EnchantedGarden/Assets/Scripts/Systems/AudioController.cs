using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static void PlayAudio(AudioSource source, ScriptableAudioClip audio)
    {
        source.clip = audio.clip;
        source.volume = audio.volume;
        source.loop = audio.loop;
        source.spatialBlend = audio.spatialBlend;

        source.Play();
    }

    private static AudioController _instance;

    [SerializeField]
    private AudioSource _backgroundSource;

    [SerializeField]
    private AudioSource _otherSoundsSource;

    // AudioClips

    // Background
    [SerializeField]
    private AudioClip[] _backgroundSounds;

    // Other
    [SerializeField]
    private AudioClip _possessionPlant;

    [SerializeField]
    private AudioClip _dispossessionPlant;

    [SerializeField]
    private AudioClip _spawningSpirit;

    [SerializeField]
    private AudioClip _possessingPlantSpirit;

    [SerializeField]
    private AudioClip _pickUp;

    [SerializeField]
    private AudioClip _putDown;

    [SerializeField]
    private AudioClip _cauldronCombination;

    // Singleton instance
    public static AudioController GetInstance()
    {
        return _instance;
    }

    void Start()
    {
        _instance = this;
        ChangeBackgroundMusic(1);
    }

    public void ChangeBackgroundMusic(int level)
    {
        if (_backgroundSounds != null && _backgroundSounds.Length >= level)
        {
            _backgroundSource.clip = _backgroundSounds[level - 1];
            _backgroundSource.Play();
        }
    }

    public void PlayPossesionPlant()
    {
        _otherSoundsSource?.PlayOneShot(_possessionPlant);
    }

    public void PlayDispossesionPlant()
    {
        _otherSoundsSource?.PlayOneShot(_dispossessionPlant);
    }

    public void PlaySpawningSpirit()
    {
        _otherSoundsSource?.PlayOneShot(_spawningSpirit);
    }

    public void PlayPossessingPlantSpirit()
    {
        _otherSoundsSource?.PlayOneShot(_possessingPlantSpirit);
    }

    public void PlayPickUp()
    {
        _otherSoundsSource?.PlayOneShot(_pickUp);
    }

    public void PlayPutDown()
    {
        _otherSoundsSource?.PlayOneShot(_putDown);
    }

    public void PlayCauldronCombination()
    {
        _otherSoundsSource?.PlayOneShot(_cauldronCombination);
    }
}
