using System.Collections.Generic;
using UnityEngine;

public class SoundBundleBehaviour : MonoBehaviour
{
    private class LongPlayingSoundFromBundle
    {
        public AudioSource AudioSource { get; set; }
        public string Name { get; set; }
        public LongPlayingSoundFromBundle(AudioSource audioSource, string name)
        {
            AudioSource = audioSource;
            Name = name;
        }
    }
    #region Variables
    public List<SoundEffectSO> Effects;
    private List<LongPlayingSoundFromBundle> _currentlyPlayedSounds;
    #endregion

    #region Mono
    protected virtual void Start()
    {
        _currentlyPlayedSounds = new();
    }
    protected virtual private void Update()
    {
        if(_currentlyPlayedSounds.Count > 0)
        {
            var soundsToRemove = new List<LongPlayingSoundFromBundle>();
            foreach (var sound in _currentlyPlayedSounds)
            {
                if (sound.AudioSource == null)
                    soundsToRemove.Add(sound);
                else if (sound.AudioSource.isPlaying == false)
                    soundsToRemove.Add(sound);
            }
            foreach (var sound in soundsToRemove)
            {
                _currentlyPlayedSounds.Remove(sound);
            }
        }
    }
    #endregion
    #region Methods
    public void PlaySound(GameObject source, string soundName)
    {
        var soundEffect = Effects.Find(eff => eff.Name == soundName);
        if(soundEffect!=null)
        {
            if (soundEffect.PlayedFromPool)
            {
                Globals.Instance.SoundManager.PlaySFXFast(soundEffect.AudioClips[Random.Range(0, soundEffect.AudioClips.Length)], source.transform.position, soundEffect.VolumeModifier);
            }
            else
            {
                Globals.Instance.SoundManager.PlaySFX(soundEffect.AudioClips[Random.Range(0, soundEffect.AudioClips.Length)], source.transform.position, soundEffect.VolumeModifier);
            }
        }       
    }
    public void Playlong(GameObject source, string soundName)
    {
        var soundEffect = Effects.Find(eff => eff.Name == soundName);
        if (soundEffect != null)
        {
            if (soundEffect.PlayedFromPool)
            {
                var ret = Globals.Instance.SoundManager.PlaySFXFast(soundEffect, source.transform);
                _currentlyPlayedSounds.Add(new LongPlayingSoundFromBundle( ret, soundName));
            }
            else
            {
                var ret = Globals.Instance.SoundManager.PlaySFXFast(soundEffect, source.transform.position);
                _currentlyPlayedSounds.Add(new LongPlayingSoundFromBundle(ret, soundName));
            }
        }
    }
    public void Fadeout(string soundName)
    {
        var currentSound = _currentlyPlayedSounds.Find(s => s.Name == soundName);
        if(currentSound != null)
        {
            StartCoroutine(Globals.Instance.SoundManager.FadeOut(currentSound.AudioSource, 0.3f));
            _currentlyPlayedSounds.Remove(currentSound);
        }
    }
    #endregion
}