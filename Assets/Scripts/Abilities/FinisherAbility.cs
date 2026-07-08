using System.Linq;
using UnityEngine;

public class FinisherAbility : AbilityBehaviour
{
    #region Variables
    [HideInInspector] public CharacterBehaviour Casuality;
    #endregion

    #region Methods
    public override bool CheckPerformAvailability(CharacterBehaviour performer)
    {
        if(base.CheckPerformAvailability(performer))
        {
            if (performer is PlayerBehaviour)
            {
                var enemy = ((PlayerBehaviour)performer).SelectedCharacter;
                if (enemy != null && performer.Faction.FactionType != enemy.Faction.FactionType 
                    && performer.Faction.Allies.Contains(enemy.Faction.FactionType) == false)
                {
                    if (enemy.Stats.CurrentHP < (int)(performer.Stats.MaxHP / 5))
                        return true;
                    else
                        return false;
                }
            }   
        }
        return false;
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("Finisher", true);
    }
    #endregion
}