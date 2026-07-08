using System.Linq;
using UnityEngine;

public class HeroClassStance : MonoBehaviour
{
    #region Variables
    [Header("Stance Related Variables")]
    [Tooltip("Abilities not used in mouse combo")]
    [SerializeField] private LockableAbility[] _lockableRegularAbilities;
    [Tooltip("Abilities used in mouse combo")]
    [SerializeField] private LockableAbility[] _lockableComboAbilities;
    [Tooltip("Abilities panel that will be desplayed at the bottom of the screen when the stance is picked.")]
    public GUIAbilitiesPanel GUIPanel;
    #endregion

    #region Mono
    public void Awake()
    {
        foreach (var ability in _lockableRegularAbilities)
        {
            ability.Ability.Init();
        }
        foreach (var ability in _lockableComboAbilities)
        {
            ability.Ability.Init();
        }
    }
    #endregion

    #region Methods
    public bool GetAbilityToPerform(int abilityNumber, out AbilityBehaviour ability)
    {
        if(_lockableRegularAbilities.Count() > abilityNumber && _lockableRegularAbilities[abilityNumber].Unlocked)
        {
            ability = _lockableRegularAbilities[abilityNumber].Ability;
            return true;
        }
        ability = null;
        return false;
    }
    public bool GetComboToPerform(HeroClassBehaviour.ComboState comboState, out AbilityBehaviour ability)
    {
        var lAbility = _lockableComboAbilities.FirstOrDefault(ab => ab.Ability.AbilitySO.StateType == comboState && ab.Unlocked);
        if (lAbility != null)
        {
            ability = lAbility.Ability;
            return true;
        }
        else
        {
            lAbility = _lockableComboAbilities.FirstOrDefault(ab => ab.Ability.AbilitySO.StateType == HeroClassBehaviour.ComboState.N && ab.Unlocked);
            if(lAbility != null)
            {
                ability = lAbility.Ability;
                return true;
            }
        }
        ability = null;
        return false;
    }
    public LockableAbility GetAbility(string soName)
    {
        LockableAbility ret = _lockableRegularAbilities.FirstOrDefault(a => a.Ability.AbilitySO.Name == soName);
        if(ret is null)
        {
            ret = _lockableComboAbilities.FirstOrDefault(a => a.Ability.AbilitySO.Name == soName);
        }
        return ret;
    }
    public bool TryFindEscapeAbility(out EscapeAbilityBehaviour ability)
    {
        var lAbility = _lockableRegularAbilities.FirstOrDefault(a => a.Ability is EscapeAbilityBehaviour);
        if (lAbility != null)
        {
            ability = lAbility.Ability as EscapeAbilityBehaviour;
            return true;
        }
        else
        {
            ability = null;
            return false;
        }
    }
    public void ManageAbilityCooldowns()
    {
        foreach (var ability in _lockableRegularAbilities)
        {
            ability.Ability.UpdateCooldown();
        }
    }
    public virtual void GetOpportunityAbilities(out AbilityBehaviour leftAbility, out AbilityBehaviour RightAbility, out string leftKey, out string rightKey)
    {
        leftAbility = null;
        RightAbility = null;
        leftKey = "";
        rightKey = "";
    }
    #endregion
}
