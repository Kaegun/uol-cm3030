using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewVolumeSettings", menuName = "Scriptable/Audio/VolumeSettings")]
public class ScriptableVolumeSettings : ScriptableObject
{
    [Header("Master")]
    [SerializeField]
    private AudioMixerGroup _masterMixer;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _masterVolume = 0.5f;
    public float MasterVolume => _masterVolume;

    [Header("Music")]
    [SerializeField]
    private AudioMixerGroup _musicMixer;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _musicVolume = 0.5f;
    public float MusicVolume => _musicVolume;

    [Header("SFX")]
    [SerializeField]
    private AudioMixerGroup _sfxMixer;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _sfxVolume = 0.5f;
    public float SfxVolume => _sfxVolume;

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        UpdateAudioMixerVolume(_masterMixer, CommonTypes.Volume.MasterVolume, _masterVolume);
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        UpdateAudioMixerVolume(_musicMixer, CommonTypes.Volume.MusicVolume, _musicVolume);
    }

    public void SetSfxVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        UpdateAudioMixerVolume(_sfxMixer, CommonTypes.Volume.SFXVolume, _sfxVolume);
    }

    public void UpdateAudioMixerSettings()
    {
        UpdateAudioMixerVolume(_masterMixer, CommonTypes.Volume.MasterVolume, _masterVolume);
        UpdateAudioMixerVolume(_musicMixer, CommonTypes.Volume.MusicVolume, _musicVolume);
        UpdateAudioMixerVolume(_sfxMixer, CommonTypes.Volume.SFXVolume, _sfxVolume);
    }

    private void UpdateAudioMixerVolume(AudioMixerGroup mixerGroup, string property, float volume)
    {
        if (Mathf.Approximately(volume, 0.0f))
        {
            mixerGroup.audioMixer.SetFloat(property, -80f);
        }
        else
        {
            mixerGroup.audioMixer.SetFloat(property, Mathf.Lerp(-15f, 15f, volume));
        }
    }
}
