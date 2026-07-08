using System.Linq;

public class BerserkerEndGladiatorAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    private bool _alreadyPerformed;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _demobillizesPerformer = false;
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_alreadyPerformed == false)
            {
                performer.RemoveEffect(performer.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Gladiator"));
                performer.RemoveEffect(performer.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Rage Incarnate"));
                _alreadyPerformed = true;
            }
        }
    }
    public override void Reset()
    {
        base.Reset();
        _alreadyPerformed = false;
    }
    #endregion
}
