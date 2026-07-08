using UnityEngine;

public class PYTHEntidBehaviour : MonsterBehaviour
{
    #region Variables
    [Header("Entid Related Variables")]
    public AbilityBehaviour FirstSkill; //spawning healing triffids
    public AbilityBehaviour SecondSkill; //shooting poison if ranged combat
    public AbilityBehaviour ThirdSkill; //step into the ground, huge damage and slow
    [SerializeField] private Transform[] _triffidSpawnPoints;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        if (FirstSkill != null)
        {
            FirstSkill = Instantiate(FirstSkill, this.transform);
            FirstSkill.Init();
            FirstSkill.gameObject.SetActive(false);
            if (FirstSkill is PYTTHEntidFirstAbilityBehaviour)
            {
                ((PYTTHEntidFirstAbilityBehaviour)FirstSkill).TriffidSpawnPoints = _triffidSpawnPoints;
            }
        }
        if (SecondSkill != null)
        {
            SecondSkill = Instantiate(SecondSkill, this.transform);
            SecondSkill.Init();
            SecondSkill.gameObject.SetActive(false);
        }
        if (ThirdSkill != null)
        {
            ThirdSkill = Instantiate(ThirdSkill, this.transform);
            ThirdSkill.Init();
            ThirdSkill.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Methods
    public override void EnterCombat(CharacterBehaviour character, bool fightProvokedByGroup)
    {
        //reseting abilities cooldowns on combat enter, so if player died and returns to try again, entid won't immediately spawn triffids etc
        if(!IsInCombat)
        {
            FirstSkill.CurrentCooldown = 0;
            SecondSkill.CurrentCooldown = 0;
            ThirdSkill.CurrentCooldown = 0;
        }
        base.EnterCombat(character, fightProvokedByGroup);
    }
    public override void UpdateAbilitiesCooldown()
    {
        base.UpdateAbilitiesCooldown();
        FirstSkill.UpdateCooldown();
        SecondSkill.UpdateCooldown();
        ThirdSkill.UpdateCooldown();
    }
    #endregion
}
