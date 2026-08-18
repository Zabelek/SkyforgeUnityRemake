using UnityEngine;

public class ScenarioCutsceneTransformDrawWeapon : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Weapon Related Variables")]
    [Tooltip("The hero has to have, obviously, a weapon equipped.")]
    public HeroBehaviour Hero;
    [Tooltip("If set to true, the weapon will play a normal draw/hide animation in the cutscene. If false, it will just immediately switch to a proper state.")]
    public bool Animate;
    public bool HideInsteadOfDraw;
    private bool _drawApplied;
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (Hero != null && _drawApplied == false)
        {
            Hero.DrawWeaponForCutscene(!HideInsteadOfDraw, Animate);
            _drawApplied = true;
        }
        else if (cutsceneTimer > StartTimer + Duration)
        {
            Hero.EndDrawingWeaponForCutscene(true, Animate);
            Finished = true;
        }
    }
    #endregion
}
