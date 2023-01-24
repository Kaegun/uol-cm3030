using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    private AudioSource _backgroundSource;

    private AudioSource _otherSoundsSource;

    private static AudioController _instance;

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

    public static AudioController GetInstance() {
        return _instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        var sources = GetComponents<AudioSource>();
        _backgroundSource = sources[0];
        _otherSoundsSource = sources[1];
        ChangeBackgroundMusic(1);
        PlaySpawningSpirit();
    }

    void ChangeBackgroundMusic(int level) {
        _backgroundSource.clip =  _backgroundSounds[level-1];
        _backgroundSource.Play();
    }

    void PlayPossesionPlant() {
        _otherSoundsSource.PlayOneShot(_possessionPlant);
    }

    void PlayDispossesionPlant() {
        _otherSoundsSource.PlayOneShot(_dispossessionPlant);
    }

    void PlaySpawningSpirit() {
        _otherSoundsSource.PlayOneShot(_spawningSpirit);
    }

    void PossessingPlantSpirit() {
        _otherSoundsSource.PlayOneShot( _possessingPlantSpirit);
    }


    // Update is called once per frame
    void Update()
    {
    
        if(Input.GetKeyDown(KeyCode.S)){
            PlayDispossesionPlant();
        }

        if(Input.GetKeyDown(KeyCode.W)){
            PlayPossesionPlant();
        }

         else if(Input.GetKeyDown(KeyCode.A)){
            PlaySpawningSpirit();
        }

       else if(Input.GetKeyDown(KeyCode.D)){
          PossessingPlantSpirit(); 
        }
    }
}
