using System.Linq;
using UnityEngine;

public class SpeakingBehaviour : MonoBehaviour
{
    #region Variables
    [Tooltip("Drag all the character's voiceline Sound Effect Scriptable Objects here")]
    [SerializeField] private SoundEffectSO[] _voiceLines;
    [Tooltip("Speaker's Character Behaviour")]
    [SerializeField] private CharacterBehaviour _character;
    #endregion

    #region Methods
    public void PerformCombatShout(float attackWeight)
    {
        if(attackWeight > (float)Random.Range(0, 100)/100f)
        {
            var line = _voiceLines.FirstOrDefault(line => line.Name == "Combat_Shout");
            if(line!=null)
            {
                Globals.Instance.SoundManager.PlayVoiceFast(line.AudioClips[Random.Range(0, line.AudioClips.Length)], _character.transform, line.VolumeModifier);
            }
        }
    }
    public void Sigh()
    {
        var line = _voiceLines.FirstOrDefault(line => line.Name == "Sigh");
        if (line != null)
        {
            Globals.Instance.SoundManager.PlayVoiceFast(line.AudioClips[Random.Range(0, line.AudioClips.Length)], _character.transform, line.VolumeModifier);
        }
    }
    public void PerformHurtSound(float hurtWeight)
    {
        if (hurtWeight > (float)Random.Range(0, 100) / 100f)
        {
            var line = _voiceLines.FirstOrDefault(line => line.Name == "Hurt");
            if (line != null)
            {
                Globals.Instance.SoundManager.PlaySFXFast(line.AudioClips[Random.Range(0, line.AudioClips.Length)], _character.transform, line.VolumeModifier);
            }
        }
    }
    public void SpeakLine(string lineName)
    {
        var line = _voiceLines.FirstOrDefault(line => line.Name == lineName);
        if (line != null)
        {
            Globals.Instance.SoundManager.PlayVoiceFast(line.AudioClips[Random.Range(0, line.AudioClips.Length)], _character.transform, line.VolumeModifier);
        }
    }
    public void PerformIdleSound(float idleWeight)
    {
        if (idleWeight > (float)Random.Range(0, 100) / 100f)
        {
            var line = _voiceLines.FirstOrDefault(line => line.Name == "Idle");
            if (line != null)
            {
                Globals.Instance.SoundManager.PlaySFXFast(line.AudioClips[Random.Range(0, line.AudioClips.Length)], _character.transform, line.VolumeModifier);
            }
        }
    }
    public void PerformHealSound()
    {
        var healsound = Globals.Instance.CommonSoundEffects.FirstOrDefault(e => e.Name == "Heal");
        if (healsound != null)
            Globals.Instance.SoundManager.PlaySFXFast(healsound, this.transform.position);
    }
    public void PerformHitSound(bool playerVariant)
    {
        if (playerVariant)
            Globals.Instance.SoundManager.PlaySFXFast(Globals.Instance.CommonSoundEffects.FirstOrDefault(e => e.Name == "Player_Hit"), this.transform.position);
        else
            Globals.Instance.SoundManager.PlaySFXFast(Globals.Instance.CommonSoundEffects.FirstOrDefault(e => e.Name == "Hit"), this.transform.position);
    }
    #endregion
}