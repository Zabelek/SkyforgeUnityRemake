using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region Variables
    private AudioSource _sfxAudioSource, _voiceAudioSource, _musicAudioSource;
    [Header("Pool Settings")]
    [Tooltip("The amount of sounds that can play simultaneously")]
    public int PoolSize = 30;
    private Queue<AudioSource> _pool = new Queue<AudioSource>();
    //to assign proper mixer groups to audio sources
    [SerializeField] private AudioMixerGroup _sfxGroup, _voiceGroup, _musicGroup;
    #endregion

    #region Mono
    void Awake()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            AddNewAudioSourceToThePool();
        }
        _sfxAudioSource = gameObject.AddComponent<AudioSource>();
        _sfxAudioSource.outputAudioMixerGroup = _sfxGroup;
        _voiceAudioSource = gameObject.AddComponent<AudioSource>();
        _voiceAudioSource.outputAudioMixerGroup = _voiceGroup;
        _musicAudioSource = gameObject.AddComponent<AudioSource>();
        _musicAudioSource.outputAudioMixerGroup = _musicGroup;
    }
    #endregion

    #region SFXMethods
    public AudioSource PlaySFXFast(AudioClip clip, Vector3 position, float volume = 1f)
    {
        var src = PrepareFastSoundPlaying(clip, volume);
        if (src != null)
        {
            src.transform.position = position;
            src.outputAudioMixerGroup = _sfxGroup;
        }
        return src;
    }
    public AudioSource PlaySFXFast(AudioClip clip, Transform parent, float volume = 1f)
    {
        var src = PrepareFastSoundPlaying(clip, volume);
        if (src != null)
        {
            src.transform.SetParent(parent);
            src.transform.localPosition = Vector3.zero;
            src.outputAudioMixerGroup = _sfxGroup;
        }
        return src;
    }
    public AudioSource PlaySFXFast(SoundEffectSO soundEffect, Vector3 position)
    {
        return PlaySFXFast(soundEffect.AudioClips[Random.Range(0, soundEffect.AudioClips.Length)], position, soundEffect.VolumeModifier);
    }
    public AudioSource PlaySFXFast(SoundEffectSO soundEffect, Transform parent)
    {
        return PlaySFXFast(soundEffect.AudioClips[Random.Range(0, soundEffect.AudioClips.Length)], parent, soundEffect.VolumeModifier);
    }
    public void PlaySFX(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    public void PlaySFX(SoundEffectSO soundEffect, Vector3 position)
    {
        PlaySFX(soundEffect.AudioClips[Random.Range(0, soundEffect.AudioClips.Length)], position, soundEffect.VolumeModifier);
    }
    public void PlayGlobalSFX(SoundEffectSO soundEffect)
    {
        PlayGlobalSFX(soundEffect.AudioClips[Random.Range(0, soundEffect.AudioClips.Length)], soundEffect.VolumeModifier);
    }
    public void PlayGlobalSFX(AudioClip audioClip, float volume = 1f)
    {
        _sfxAudioSource.volume = volume;
        _sfxAudioSource.PlayOneShot(audioClip);
    }
    #endregion

    #region VoiceMethods
    public AudioSource PlayVoiceFast(AudioClip clip, Vector3 position, float volume = 1f)
    {
        var src = PrepareFastSoundPlaying(clip, volume);
        if (src != null)
        {
            src.transform.position = position;
            src.outputAudioMixerGroup = _voiceGroup;
        }
        return src;
    }
    public AudioSource PlayVoiceFast(AudioClip clip, Transform parent, float volume = 1f)
    {
        var src = PrepareFastSoundPlaying(clip, volume);
        if (src != null)
        {
            src.transform.SetParent(parent);
            src.outputAudioMixerGroup = _voiceGroup;
        }
        return src;
    }
    public void PlayGlobalVoice(AudioClip audioClip, float volume = 1f)
    {
        _voiceAudioSource.volume = volume;
        _voiceAudioSource.PlayOneShot(audioClip);
    }
    #endregion

    #region OtherMethods
    private AudioSource PrepareFastSoundPlaying(AudioClip clip, float volume)
    {
        if (clip == null || _pool.Count == 0)
            return null;
        AudioSource src = _pool.Dequeue();
        src.volume = volume;
        src.PlayOneShot(clip);
        StartCoroutine(ReturnToPoolAfter(src));
        return src;
    }
    private IEnumerator ReturnToPoolAfter(AudioSource src)
    {
        yield return new WaitWhile(() => !src.IsDestroyed() && src.isPlaying);
        //when the sound from the pool gets attrached to the object that's destroyed during sound playing, it will be destroyed as well
        if(src.IsDestroyed())
        {
            AddNewAudioSourceToThePool();
        }
        else
        {
            src.transform.SetParent(null);
            _pool.Enqueue(src);
        }
    }
    private void AddNewAudioSourceToThePool()
    {
        GameObject go;
        go = new GameObject("PooledAudioSource");
        go.transform.parent = transform;
        go.AddComponent<AudioSource>();
        AudioSource src = go.GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0.8f;
        _pool.Enqueue(src);
    }
    public IEnumerator FadeOut(AudioSource source, float fadeDuration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        source.Stop();
        source.volume = startVolume;
    }
    #endregion
}
