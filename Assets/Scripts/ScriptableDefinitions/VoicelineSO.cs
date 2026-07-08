using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/VoicelineSO")]
public class VoicelineSO : ScriptableObject
{

    public string Text;
    public AudioClip Voice;
    public SpeakerSO Speaker;
    public float Volume = 1;
    [Tooltip("time that the chat box will be visible. This is not an audio clip time. Make sure the player will be able to read the text in the text box before it fades out")] 
    public float Time;
}
