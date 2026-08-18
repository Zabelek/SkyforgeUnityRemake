using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GUIItemPickView _itemPickWindowBase;
    private GUIItemPickView _currentPickWindow;
    private InventorySlot _tempClickedItemSlot;
    #endregion

    #region Mono
    private void Awake()
    {
        _playerVisual.SetAnimationState("Menu", true);
        if (SkyforgeLoader.CurrentProfile != null)
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
        var heroStats = (_playerVisual.Stats as HeroStats);
        if (_playerVisual.EquippedWeapon != null)
        _damageDisplay.text = (_playerVisual.Stats.BaseDamage).ToString() + " - " + ((_playerVisual.Stats.BaseDamage) + _playerVisual.Stats.MaxDamage).ToString();
        _healthDisplay.text = _playerVisual.Stats.MaxHP.ToString();
        _attackSpeedDisplay.text = (_playerVisual.Stats.AttackSpeed * 100) + "%";
        _criticalChanceDisplay.text = (_playerVisual.Stats.CriticalChance * 100) + "%";
        _criticalDamageBonusDisplay.text = "100%";
        _companionDamageDisplay.text = heroStats.CompanionDamage.ToString();
        _vampirismDisplay.text = (_playerVisual.Stats.Vampirism * 100) + "%";
        _defenseDisplay.text = (_playerVisual.Stats.Defense * 100) + "%";
        _armorDisplay.text = (_playerVisual.Stats.GearStats.Armor * 100) + "%";
        _stabilityDisplay.text = (_playerVisual.Stats.Stability * 100) + "%";
        _movementSpeedDisplay.text = (_playerVisual.Stats.MovementSpeed * 20) + "%";
        _maxDashChargesDisplay.text = heroStats.DashChargeMax.ToString();
        _maxCompanionChargesDisplay.text = heroStats.CompanionChargeMax.ToString();
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
    private void SetUpNewTooltip(ItemSO itemSO)
    {
        _currentTooltip = Instantiate(_tooltipBase, _tooltipsParent);
        _currentTooltip.SetCanvas(_tooltipCanvas);
        _currentTooltip.SetForItem(itemSO);
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
        bool valid = false;
        var itemList = new List<InventorySlot>();
        string title = "";
        //first chech what type of items are desired to display in the window
        if (sender == (object)_armorSlot)
        {
            itemList = SkyforgeLoader.CurrentProfile.Inventory.Slots.Where(s => s.Item?.ItemSO is ArmorSO).ToList();
            valid = true;
            title = "Armor";
        }
        else if (sender == (object)_weaponSlot)
        {
            itemList = SkyforgeLoader.CurrentProfile.Inventory.Slots.Where(s => s.Item?.ItemSO is WeaponSO).ToList();
            valid = true;
            title = "Weapon";
        }
        else if (sender == (object)_artifactSlot)
        {
            itemList = SkyforgeLoader.CurrentProfile.Inventory.Slots.Where(s => s.Item?.ItemSO is ArtifactSO).ToList();
            valid = true;
            title = "Artifact";
        }
        //add an empty slot so that the character can take of an equipment peace completely, but some weapon has to be on the slot so that the character can fight with something
        if (title != "Weapon")
        {
            var emptySlot = SkyforgeLoader.CurrentProfile.Inventory.Slots.FirstOrDefault(s => s.Item == null);
            if (emptySlot != null)
            {
                itemList.Add(emptySlot);
            }
        }
        //display the window only if one of the above are valid
        if (valid)
        {
            _tempClickedItemSlot = (sender as GUIInventorySlot).InventorySlot;
            if (_currentPickWindow != null)
                DestroyPickWindow(this, EventArgs.Empty);
            else
            {
                _currentPickWindow = Instantiate(_itemPickWindowBase, this.transform);
                _currentPickWindow.gameObject.SetActive(true);
                _currentPickWindow.ItemSlotSelectedEvent += ItenPicked;
                _currentPickWindow.WindowDestroyEvent += DestroyPickWindow;
                foreach (var slot in _currentPickWindow.AssignSlots(itemList))
                {
                    //so that the items in the view can also display tooltips
                    slot.OnPointerEnterEvent += SlotPointerEnterAction;
                    slot.OnPointerExitEvent += SlotPointerExitAction;
                }
                _currentPickWindow.transform.position = (sender as GUIInventorySlot).transform.position + new Vector3(200,20,0);
                _currentPickWindow.SetTitle(title);
            }
        }
    }
    private void SlotPointerExitAction(object sender, EventArgs e)
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
    }
    private void DestroyPickWindow(object sender, EventArgs e)
    {
        if(_currentPickWindow!=null)
        {
            _currentPickWindow.ItemSlotSelectedEvent -= ItenPicked;
            _currentPickWindow.WindowDestroyEvent -= DestroyPickWindow;
            foreach (var slot in _currentPickWindow.GetSlots())
            {
                slot.OnPointerEnterEvent -= SlotPointerEnterAction;
                slot.OnPointerExitEvent -= SlotPointerExitAction;
            }
            Destroy(_currentPickWindow.gameObject);
            _currentPickWindow = null;
        }
    }
    private void ItenPicked(object sender, GUIItemPickView.InventorySlotEventArgs e)
    {
        //When the item is picked from the additional menu
        //In the future Activate has to be replaced by a dedicated method for this view, becasue usable items will be used istead of assigned to slots
        if(e.Slot.Item != null)
        {
            e.Slot.Item.Activate(e.Slot);
            DestroyPickWindow(this, EventArgs.Empty); 
            SlotPointerExitAction(this, EventArgs.Empty);
            UpdateValues();
        }
        else if(_tempClickedItemSlot!=null && _tempClickedItemSlot.Item != null)
        {
            //if the item is null, some of the parameters have to be set here (they are normally set in Item.Activate())
            e.Slot.Item = _tempClickedItemSlot.Item;
            _tempClickedItemSlot.Item = null;
            SkyforgeLoader.EquipmentChanged = true;
            DestroyPickWindow(this, EventArgs.Empty);
            SlotPointerExitAction(this, EventArgs.Empty);
            UpdateValues();
            _tempClickedItemSlot = null;
        }
    }
    #endregion
}
