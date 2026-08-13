using System.Linq;
using UnityEngine;

public class BerserkerGladiatorEffect : GameplayEffectBehaviour
{
    #region Variables
    [Header("Gladiator Related Variables")]
    [Tooltip("How much bigger the character will be on Gladiator stance")]
    [SerializeField] private float _heightMod;
    private float _glowTimer = 1.5f;
    private float _growthTimer = 0.5f;
    private Vector3 _initialLocalScale;
    [Tooltip("Reference to fire particle systems that will appear when Gladiator is activated")]
    [SerializeField] private ParticleSystem _headFireBase, _armFireBase, _weaponFireBase;
    private ParticleSystem _currentheadFire, _currentLeftArmFire, _currentRightArmFire, _weaponFire;
    #endregion

    #region Methods
    public override void OnApply(CharacterBehaviour character)
    {
        base.OnApply(character);
        _initialLocalScale = character.transform.localScale;
        try { ((HeroBehaviour)character).GetHeroClass().SetStance(1); } catch { Debug.Log("Not berserker got Gladiator stance wtf?!"); }
        Transform headBone = character.RegisteredBones.FirstOrDefault(b => b.name == "spine.006");
        Transform leftArmBone = character.RegisteredBones.FirstOrDefault(b => b.name == "shoulder.L");
        Transform rightArmBone = character.RegisteredBones.FirstOrDefault(b => b.name == "shoulder.R");
        if (headBone != null)
        {
            _currentheadFire = Instantiate(_headFireBase, headBone);
            _currentLeftArmFire = Instantiate(_armFireBase, leftArmBone);
            _currentLeftArmFire.transform.localPosition = new Vector3(0, 0.003f, 0);
            _currentRightArmFire = Instantiate(_armFireBase, rightArmBone);
            _currentRightArmFire.transform.localPosition = new Vector3(0, 0.003f, 0);
            _weaponFire = Instantiate(_weaponFireBase, ((HeroBehaviour)character).EquippedWeapon.transform);
        }
    }
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        if(_growthTimer>0)
        {
            character.transform.localScale = _initialLocalScale * Mathf.Lerp(1, _heightMod, 1 - (_growthTimer * 2));
            _growthTimer -= Time.fixedDeltaTime;
        }
        if(_glowTimer > 0)
        {
            character.SetGlow(0.2f + (_glowTimer / 3), Color.orange);
            _glowTimer -= Time.fixedDeltaTime;
        }
        else
        {
            character.SetGlow(0.2f, Color.orange);
        }
    }
    public override void OnRemove(CharacterBehaviour character)
    {
        character.transform.localScale = new Vector3(
            character.transform.localScale.x / _heightMod,
            character.transform.localScale.y / _heightMod,
            character.transform.localScale.z / _heightMod);
        try 
        {
            ((HeroBehaviour)character).GetHeroClass().SetStance(0);
        } catch { Debug.Log("Not berserker got Gladiator stance wtf?!"); }
        character.SetGlow(0);
        if(_currentheadFire!=null)
        {
            Destroy(_currentheadFire.gameObject);
            _currentheadFire = null;
        }
        if(_currentLeftArmFire!=null)
        {
            Destroy(_currentLeftArmFire.gameObject);
            _currentLeftArmFire = null;
        }
        if(_currentRightArmFire!=null)
        {
            Destroy(_currentRightArmFire.gameObject);
            _currentRightArmFire = null;
        }
        if(_weaponFire!=null)
        {
            Destroy(_weaponFire.gameObject);
            _weaponFire = null;
        }
        character.RemoveEffect(character.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Rage Incarnate"));    
    }
    #endregion
}
