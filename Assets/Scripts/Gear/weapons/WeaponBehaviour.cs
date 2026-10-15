using UnityEngine;

public class WeaponBehaviour : GearPeaceBehaviour
{
    #region Variables
    [Header("Weapon Related Variables")]
    [Tooltip("Weapon Scriptable Object wich base information")]
    public WeaponSO WeaponSO;
    [SerializeField] private Transform _weaponMesh;
    [Tooltip("Some abilities make the weapon leave trace behind while swinging. The particle effect for it is inside weapon prefab(should be adjusted to weapon size and shape)")]
    [SerializeField] private ParticleSystem _trailEffect;
    [Tooltip("All the sounds that the weapon can make. Usually same across the weapon type, unless some legendaries with custom sounds")]
    [SerializeField] SoundBundleBehaviour _sounds;
    [Header("Draw and Hide Animation")]
    [Tooltip("How much time the hiding/drawing animation will take")]
    [SerializeField] private float WeaponDrawAnimationTime = 0.2f;
    [Tooltip("To start draw animation after some time, to better align with the player character drawing animation")]
    [SerializeField] private float WeaponDrawDelay = 0.2f;
    [Tooltip("To start hide animation after some time, to better align with the player character hiding animation")]
    [SerializeField] private float WeaponHideDelay = 0.2f;
    [Tooltip("if the weapon should enter the scene already drawn, useful for cutscenes or map decoration")]
    [SerializeField] bool StartDrawn = false;
    private bool _drawScheduled, _hideScheduled, _isHidden;
    private float _drawTimer;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        SetTrail(false);
        if (!StartDrawn)
            SetWeaponHide();
        else
            SetWeaponDraw();
    }
    private void Update()
    {
        if (_drawScheduled)
        {
            _drawTimer += Time.deltaTime;
            if (_drawTimer >= WeaponDrawDelay)
            {
                float drawAmount = ((WeaponDrawAnimationTime - (_drawTimer - WeaponDrawDelay)) / WeaponDrawAnimationTime) * 100;
                if (drawAmount < 0)
                {
                    drawAmount = 0;
                    _drawTimer = 0;
                    _drawScheduled = false;
                }
                SetHiddenAmount(drawAmount);
            }

        }
        else if (_hideScheduled)
        {
            _drawTimer += Time.deltaTime;
            if (_drawTimer >= WeaponHideDelay)
            {
                float drawAmount = (1 - ((WeaponDrawAnimationTime - (_drawTimer - WeaponHideDelay)) / WeaponDrawAnimationTime)) * 100;
                if (drawAmount > 100)
                {
                    drawAmount = 100;
                    _drawTimer = 0;
                    _hideScheduled = false;
                }
                SetHiddenAmount(drawAmount);
            }
        }
    }
    #endregion

    #region Methods
    public override void Equip(HeroBehaviour hero, Transform slot, bool onlyVisual)
    {
        base.Equip(hero, slot, onlyVisual);
        if (hero.CombatStance)
            SetWeaponDraw();
    }
    public void PlaySound(string soundName)
    {
        _sounds.PlaySound(this.gameObject, soundName);
    }
    public void PlayLongSound(string soundName)
    {
        _sounds.Playlong(this.gameObject, soundName);
    }
    public void StopLongSound(string soundName)
    {
        _sounds.Fadeout(soundName);
    }
    public void AnimateWeaponDraw()
    {
        if(_isHidden)
        {
            _drawScheduled = true;
            _isHidden = false;
        }
    }
    public void AnimateWeaponHide()
    {
        if(!_isHidden)
        {
            _hideScheduled = true;
            _isHidden = true;
        }
    }
    public void SetWeaponDraw()
    {
        _isHidden = false;
        _hideScheduled = false;
        _drawScheduled = false;
        _drawTimer = 0;
        SetHiddenAmount(0);
    }
    public void SetWeaponHide()
    {
        _isHidden = true;
        _hideScheduled = false;
        _drawScheduled = false;
        _drawTimer = 0;
        SetHiddenAmount(100);
    }
    private void SetHiddenAmount(float amount)
    {
        var smr = _weaponMesh.GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
            smr.SetBlendShapeWeight(0, amount);
    }
    public void SetTrail(bool on)
    {
        _trailEffect.gameObject.SetActive(on);
    }
    public override GearBonus GetBaseStats(Item item)
    {
        return GetGeneralBaseStats(item);
    }
    public override GearBonus GetMarkedStats(Item item)
    {
        return GetGeneralMarkedStats(item);
    }
    #endregion

    #region StaticMethods
    public static GearBonus GetGeneralMarkedStats(Item item)
    {
        var ret = new GearBonus();
        if (item?.ItemUpgradeState != null && item.ItemUpgradeState.EtherFibers[0]!=0)
        {
            if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.KingHalf)
            {
                ret.DamagePercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.DamagePercentBonus += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.King)
            {
                ret.CompanionDamagePercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.CompanionDamagePercentBonus += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.AssassinHalf)
            {
                ret.CriticalChanceBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.CriticalChanceBonus += 0.0035f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Assassin)
            {
                ret.CriticalDamagePercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.CriticalDamagePercentBonus += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.SageHalf)
            {
                ret.AttackSpeedBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.AttackSpeedBonus += 0.002f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Sage)
            {
                ret.MaxDamagePercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.MaxDamagePercentBonus += 0.0075f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.JudgeHalf)
            {
                ret.VampirismBonus += 0.005f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.VampirismBonus += 0.001f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Judge)
            {
                ret.StabilityBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.StabilityBonus += 0.005f * fiber;
                }
            }
        }
        return ret;
    }
    public static GearBonus GetGeneralBaseStats(Item item)
    {
        var ret = new GearBonus();
        if (item?.ItemSO is WeaponSO)
        {
            var damage = 1;
            if (item.ItemUpgradeState != null)
            {
                for (int i = 0; i < item.ItemUpgradeState.Generation; i++)
                {
                    damage += i;
                }
            }
            if (item.ItemSO.Rarity == ItemSO.RarityLevel.Uncommon)
                damage *= 2;
            else if (item.ItemSO.Rarity == ItemSO.RarityLevel.Rare)
                damage *= 3;
            else if (item.ItemSO.Rarity == ItemSO.RarityLevel.Epic || item.ItemSO.Rarity == ItemSO.RarityLevel.Legendary)
                damage *= 4;
            ret.DamageBonus += (int)(damage * (item.ItemSO as WeaponSO).GetDamage());
        }
        return ret;
    }
    #endregion
}
