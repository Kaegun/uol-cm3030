using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioClip", menuName = "Scriptable/Audio/AudioClip")]
public class ScriptableAudioClip : ScriptableObject
{
	public AudioClip clip;
	[Range(0, 1)]
	public float volume;
	public bool loop;
	[Range(0, 1)]
	public float spatialBlend;
}
