using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GUIGameMenuEquipmentControls : MonoBehaviour
{
    #region Variables
    [SerializeField] private PlayerBehaviour _playerVisual;
    [Header("Slots")]
    [SerializeField] private GUIInventorySlot _weaponSlot;
    [SerializeField] private GUIInventorySlot _armorSlot, _artifactSlot, _ringSlot, _amuletSlot, _broochSlot, _braceletSlot,
        _sapphireSlot, _rubySlot, _emeraldSlot, _topazSlot, _qa1, _qa2, _qa3, _qa4, _qa5, _qa6;
    private List<GUIInventorySlot> _slots;
    [Header("Tooltips")]
    [Tooltip("Prefab used to spawn tooltips")]
    [SerializeField] private GUITooltip _tooltipBase;
    private GUITooltip _currentTooltip;
    [Tooltip("Canvas reference to handle tooltip positioning")]
    [SerializeField] private Canvas _tooltipCanvas;
    [Tooltip("Where tooltips will be spawned")]
    [SerializeField] private Transform _tooltipsParent;
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI _damageDisplay;
    [SerializeField] private TextMeshProUGUI _healthDisplay, _attackSpeedDisplay, _criticalChanceDisplay, _criticalDamageBonusDisplay, _companionDamageDisplay, 
        _vampirismDisplay, _defenseDisplay, _armorDisplay, _stabilityDisplay, _movementSpeedDisplay, _maxDashChargesDisplay, _maxCompanionChargesDisplay;
    //used not to re-add all perks on each update
    private bool _firstSync = true;
    #endregion

    #region Mono
    private void Awake()
    {
        if(SkyforgeLoader.CurrentProfile != null)
        {
            _armorSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.ArmorSlot;
            _weaponSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot;
            _artifactSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.ArtifactSlot;
            _ringSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.RingSlot;
            _amuletSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.AmuletSlot;
            _broochSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.BroochSlot;
            _braceletSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.BraceletSlot;
            _sapphireSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.SapphiteSlot;
            _rubySlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.RubySlot;
            _emeraldSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.EmeraldSlot;
            _topazSlot.InventorySlot = SkyforgeLoader.CurrentProfile.Equipment.TopazSlot;
        }
        _slots = new();
        _slots.Add(_armorSlot);
        _slots.Add(_weaponSlot);
        _slots.Add(_artifactSlot);
        _slots.Add(_ringSlot);
        _slots.Add(_amuletSlot);
        _slots.Add(_broochSlot);
        _slots.Add(_braceletSlot);
        _slots.Add(_sapphireSlot);
        _slots.Add(_rubySlot);
        _slots.Add(_emeraldSlot);
        _slots.Add(_topazSlot);
        _slots.Add(_qa1);
        _slots.Add(_qa2);
        _slots.Add(_qa3);
        _slots.Add(_qa4);
        _slots.Add(_qa5);
        _slots.Add(_qa6);
        foreach(var slotWidget in _slots)
        {
            slotWidget.OnPointerEnterEvent += SlotPointerEnterAction;
            slotWidget.OnPointerExitEvent += SlotPointerExitAction;
            slotWidget.OnPointerUpEvent += SlotPointerUpAction;
            slotWidget.OnPointerDownEvent += SlotPointerDownAction;
        }
    }
    public void OnDisable()
    {
        _currentTooltip?.gameObject.SetActive(false);
    }
    #endregion

    #region Methods
    public void UpdateValues()
    {
        _ = UpdateCharacterForView(true);
        AssignQASlots();
        _armorSlot.UpdateSlot();
        _weaponSlot.UpdateSlot();
        _artifactSlot.UpdateSlot();
        _ringSlot.UpdateSlot();
        _amuletSlot.UpdateSlot();
        _broochSlot.UpdateSlot();
        _braceletSlot.UpdateSlot();
        _sapphireSlot.UpdateSlot();
        _rubySlot.UpdateSlot();
        _emeraldSlot.UpdateSlot();
        _topazSlot.UpdateSlot();
        UpdateStatDisplay();
    }
    private void UpdateStatDisplay()
    {
        int weaponDamage = 0;
        if (_playerVisual.EquippedWeapon != null)
            weaponDamage = _playerVisual.EquippedWeapon.WeaponSO.GetDamage();
        _damageDisplay.text = (_playerVisual.Stats.BaseDamage + weaponDamage).ToString() + " - " + ((_playerVisual.Stats.BaseDamage + weaponDamage) + _playerVisual.Stats.MaxDamage).ToString();
        _healthDisplay.text = _playerVisual.Stats.MaxHP.ToString();
        _attackSpeedDisplay.text = (_playerVisual.Stats.AttackSpeed * 100) + "%";
        _criticalChanceDisplay.text = (_playerVisual.Stats.CriticalChance * 100) + "%";
        _criticalDamageBonusDisplay.text = "100%";
        _companionDamageDisplay.text = _playerVisual.companionDamage.ToString();
        _vampirismDisplay.text = (_playerVisual.Stats.Vampirism * 100) + "%";
        _defenseDisplay.text = (_playerVisual.Stats.Defense * 100) + "%";
        if(_playerVisual.EquippedArmor != null)
            _armorDisplay.text = (_playerVisual.EquippedArmor.ArmorSO.BaseArmorAmount*100) +"%";
        else
            _armorDisplay.text = "0%";
        _stabilityDisplay.text = (_playerVisual.Stats.Stability * 100) + "%";
        _movementSpeedDisplay.text = (_playerVisual.Stats.MovementSpeed * 20) + "%";
        _maxDashChargesDisplay.text = _playerVisual.DashChargeMax.ToString();
        _maxCompanionChargesDisplay.text = _playerVisual.CompanionChargeMax.ToString();
    }
    private void AssignQASlots()
    {
        //Unlike other slots, these one are dynamically attached to whatever slot that contains the item assigned to quick access
        if(SkyforgeLoader.CurrentProfile!= null)
        {
            _qa1.InventorySlot = SkyforgeLoader.CurrentProfile.Inventory.GetQuickAccessItem(Item.QuickAccessSlot.QA1);
            _qa1.UpdateSlot();
            _qa2.InventorySlot = SkyforgeLoader.CurrentProfile.Inventory.GetQuickAccessItem(Item.QuickAccessSlot.QA2);
            _qa2.UpdateSlot();
            _qa3.InventorySlot = SkyforgeLoader.CurrentProfile.Inventory.GetQuickAccessItem(Item.QuickAccessSlot.QA3);
            _qa3.UpdateSlot();
            _qa4.InventorySlot = SkyforgeLoader.CurrentProfile.Inventory.GetQuickAccessItem(Item.QuickAccessSlot.QA4);
            _qa4.UpdateSlot();
            _qa5.InventorySlot = SkyforgeLoader.CurrentProfile.Inventory.GetQuickAccessItem(Item.QuickAccessSlot.QA5);
            _qa5.UpdateSlot();
            _qa6.InventorySlot = SkyforgeLoader.CurrentProfile.Inventory.GetQuickAccessItem(Item.QuickAccessSlot.QA6);
            _qa6.UpdateSlot();
        }
    }
    public async Task UpdateCharacterForView(bool animateRig)
    {
        _playerVisual.SyncEquipment();
        _playerVisual.SyncPerks(_firstSync);
        _firstSync = false;
        await _playerVisual.GetComponent<OutfitManager>().EquipOutfit(SkyforgeLoader.CurrentProfile.HatNumber, OutfitSO.OutfitSlot.Head);
        if (animateRig)
        {
            //this first line is needed so that the player immediately starts from the correct animation, not overriding it by anything
            _playerVisual.SetAnimationState("Menu", true);
            _playerVisual.PlayAnimation("MenuStart", true);
            _playerVisual.ResetAnimation();
            //first setting weapon at draw state so that it doesn't play animation of drawing
            _playerVisual.EquippedWeapon.SetWeaponDraw();
            _playerVisual.ChangeWeaponOutState(true);
        }
    }
    //To be moved into tooltip class
    private void SetUpNewTooltip(ItemSO itemSO)
    {
        _currentTooltip = Instantiate(_tooltipBase, _tooltipsParent);
        _currentTooltip.SetCanvas(_tooltipCanvas);
        _currentTooltip.SetTitle(itemSO.Name);
        _currentTooltip.SetDescription(itemSO.Description);
        _currentTooltip.SetTitleImage(itemSO.InterfaceSprite);
        if (itemSO is WeaponSO)
        {
            _currentTooltip.AddStatBonus("Damage Bonus: ", ((WeaponSO)itemSO).GetDamage(), false);
            _currentTooltip.SetDescription(((WeaponSO)itemSO).Type.Name);
            if (itemSO.Rarity == ItemSO.RarityLevel.Legendary)
            {
                _currentTooltip.SetSpecialDescription(itemSO.Description);
            }
            else
            {
                _currentTooltip.SetDescription(((WeaponSO)itemSO).Type.Name + "\n" + itemSO.Description);
            }
        }
        else if (itemSO is ArmorSO)
        {
            _currentTooltip.AddStatBonus("Defense Bonus: ", ((ArmorSO)itemSO).BaseArmorAmount, true);
        }
    }
    #endregion

    #region EventHandlers
    private void SlotPointerEnterAction(object sender, EventArgs e)
    {
        if (_currentTooltip == null && sender is GUIInventorySlot && (sender as GUIInventorySlot).InventorySlot?.Item != null)
        {
            SetUpNewTooltip((sender as GUIInventorySlot).InventorySlot.Item.ItemSO);
        }
    }
    private void SlotPointerDownAction(object sender, EventArgs e)
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
    }
    private void SlotPointerUpAction(object sender, EventArgs e)
    {
    }
    private void SlotPointerExitAction(object sender, EventArgs e)
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
    }
    #endregion
}
