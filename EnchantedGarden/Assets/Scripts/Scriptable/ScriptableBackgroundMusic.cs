using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableBackgroundMusic", menuName = "Scriptable/BackgroundMusic")]
public class ScriptableBackgroundMusic : ScriptableObject
{
    public ScriptableAudioClip lowIntensityAudio;
    public ScriptableAudioClip midIntensityAudio;
    public ScriptableAudioClip highIntensityAudio;
}
