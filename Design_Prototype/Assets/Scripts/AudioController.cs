using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    
    private static AudioController _instance;

    private AudioSource _backgroundSource;

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
    private AudioClip _caldronCombination;

    // Singleton instance
    public static AudioController GetInstance() {
        return _instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        
        var sources = GetComponents<AudioSource>();
        
        _backgroundSource = sources[0];
        _backgroundSource.volume = 0.7f;
        _otherSoundsSource = sources[1];
        
        ChangeBackgroundMusic(1);;
    }

    public void ChangeBackgroundMusic(int level) {
        _backgroundSource.clip =  _backgroundSounds[level-1];
        _backgroundSource.Play();
    }

    public void PlayPossesionPlant() {
        _otherSoundsSource.PlayOneShot(_possessionPlant);
    }

    public  void PlayDispossesionPlant() {
        _otherSoundsSource.PlayOneShot(_dispossessionPlant);
    }

    public void PlaySpawningSpirit() {
        _otherSoundsSource.PlayOneShot(_spawningSpirit);
    }

    public void PlayPossessingPlantSpirit() {
        _otherSoundsSource.PlayOneShot( _possessingPlantSpirit);
    }

    public  void PlayPickUp() {
        _otherSoundsSource.PlayOneShot( _pickUp);
    }

    public void PlayPutDown() {
        _otherSoundsSource.PlayOneShot( _putDown);
    }

    public void PlayCaldronCombination(){
        _otherSoundsSource.PlayOneShot(_caldronCombination);
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
            PlayPossessingPlantSpirit(); 
        }

        else if(Input.GetKeyDown(KeyCode.F)){
            PlayPickUp(); 
        }

        else if(Input.GetKeyDown(KeyCode.G)){
            PlayPutDown(); 
        }

         else if(Input.GetKeyDown(KeyCode.H)){
            PlayCaldronCombination(); 
        }
    }
}
