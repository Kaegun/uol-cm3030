using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable/AudioClip")]
public class ScriptableAudioClip : ScriptableObject
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;
    public bool loop;
    [Range(0, 1)]
    public float spatialBlend;
}
