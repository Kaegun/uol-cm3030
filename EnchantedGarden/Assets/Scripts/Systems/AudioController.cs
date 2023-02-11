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
}
