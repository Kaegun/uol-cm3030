using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    private AudioSource[] sources;

    [SerializeField]
    private AudioClip[] _backgroundSounds;

    private int currentLevel;

    // todo
    // who calls this method -> AudioController or LevelController(?)
    public void changeCurrentLevel(int level){
        sources[0].clip = _backgroundSounds[level-1];
        sources[0].Play();
    }

    // Start is called before the first frame update
    void Start()
    {
       sources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //level == 2
       if(Input.GetKey(KeyCode.S)){
           changeCurrentLevel(2);
        }

        else if(Input.GetKey(KeyCode.W)){
            changeCurrentLevel(1);
        }

        else if(Input.GetKey(KeyCode.A)){
            changeCurrentLevel(3);
        }

        else if(Input.GetKey(KeyCode.D)){
            sources[1].clip = _backgroundSounds[2];
            sources[1].Play();
        }
    }
}
