using UnityEngine;

public class PYTHDryadHealAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    public CharacterBehaviour HealingTarget;
    [SerializeField] private ParticleSystem _particlesBase;
    private ParticleSystem _currentParticles;
    [SerializeField] private float _healingInterval;
    private float _healingTimesAlreadyDone;
    [Tooltip("When the healed ally gets to ofar from the user, the healing is interrupted")]
    public float MaxHealingDistance = 15f;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if(_performingTimer > _healingInterval * _healingTimesAlreadyDone)
        {
            _healingTimesAlreadyDone++;
            HealingTarget.Heal((int)(performer.GetEffectiveDamage() * AbilitySO.DamageMultiplier), true);
        }
        //rotates the dryad towards the healed ally
        if(HealingTarget!=null)
            performer.transform.forward = HealingTarget.transform.position - performer.transform.position;
        else
            Interrupt(performer);
        if ((HealingTarget.transform.position - performer.transform.position).magnitude > MaxHealingDistance || HealingTarget.IsDead)
        {
            Interrupt(performer);
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        _currentParticles = Instantiate(_particlesBase, performer.transform);
        _currentParticles.gameObject.SetActive(true);
        performer.PlayAnimation("Skill1");
        performer.SetAnimationState("Skill1_Performing", true);
    }
    public override void EndAbility(CharacterBehaviour performer)
    {
        base.EndAbility(performer);
        _currentParticles?.Stop();
        _currentParticles = null;
        performer.SetAnimationState("Skill1_Performing", false);
    }
    public override void Reset()
    {
        base.Reset();
        _healingTimesAlreadyDone = 0;
    }
    #endregion
}
