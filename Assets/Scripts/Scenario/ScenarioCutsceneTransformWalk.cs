using UnityEngine;

public class ScenarioCutsceneTransformWalk : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Walking Related Variables")]
    [Tooltip("The character must have an AI Handler Behaviour component to walk anywhere")]
    public CharacterBehaviour WalkingCharacter;
    public Vector3 MovingDestination;
    [Tooltip("This will override the character's speed")]
    public float Speed;
    [Tooltip("WARNING: Kinematic not yet compatible with new movement system, will cause characters to move very slowly")]
    public bool Kinematic;
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (WalkingCharacter != null && WalkingCharacter.TryGetComponent<AIHandlerBehaviour>(out var handler))
        {
            if (Speed == 0)
                handler.SetMovingDestinationForCutscene(MovingDestination, Kinematic);
            else
                handler.SetMovingDestinationForCutscene(MovingDestination, Kinematic, Speed);
        }
        Finished = true;
    }
    #endregion
}
