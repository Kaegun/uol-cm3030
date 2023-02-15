using UnityEngine;

public class AudioController
{
    public static void PlayAudio(AudioSource source, ScriptableAudioClip audio)
    {
        source.clip = audio.clip;
        source.volume = audio.volume;
        source.loop = audio.loop;
        source.spatialBlend = audio.spatialBlend;

        source.Play();
    }
    
    public static void PlayAudioDetached(ScriptableAudioClip audio, Vector3 position, float duration = -1f)
    {
        var source = GameManager.Instance.CreateDetachedAudioSource(position);
        var component = source.gameObject.AddComponent<DestroyAfterDuration>();
        component.Duration = duration > 0 ? duration : audio.clip.length;
        PlayAudio(source, audio);        
    }
}
