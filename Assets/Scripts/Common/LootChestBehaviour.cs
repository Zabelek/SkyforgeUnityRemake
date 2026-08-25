using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootChestBehaviour : MonoBehaviour, IPlayerInteractable
{
    #region Variables
    public event EventHandler OnChestOpenEvent;
    public List<InventorySlot> Slots;
    [Tooltip("The amound if inventory slots that the chest will contain")]
    [SerializeField] private int _chestSize = 16;
    [SerializeField] private LootManager _lootManager;
    [Tooltip("For opening and closing the chest")]
    [SerializeField] private Animator _animator;
    #endregion

    #region Mono
    private void Awake()
    {
        Slots = new();
        if(SkyforgeLoader.ItemRegistry != null)
        {
            //Loot manager is used to "drop" its items into the chests inventory
            var items = _lootManager.DropItems();
            foreach (var item in items)
            {
                var inventorySlot = new InventorySlot();
                inventorySlot.Item = item;
                Slots.Add(inventorySlot);
            }
            //the remaining empty slots are added to match the set chest size
            while (Slots.Count < _chestSize)
            {
                Slots.Add(new InventorySlot());
            }
        }
    }
    #endregion

    #region Methods
    public string GetInteractionTitle()
    {
        return "Open";
    }
    public IPlayerInteractable.InteractionType GetInteractionType()
    {
        return IPlayerInteractable.InteractionType.OpenClose;
    }
    public void Interact(PlayerBehaviour player)
    {
        if(!player.IsInCombat)
        {
            Globals.Instance.CurrentOpenChest = this;
            _animator.SetBool("IsOpen", true);
            OnChestOpenEvent?.Invoke(this, EventArgs.Empty);
        }
    }
    public void AddItem(Item item)
    {
        var slot = Slots.FirstOrDefault(s => s.Item == null);
        slot.Item = item;
    }
    public void AnimateClose()
    {
        _animator.SetBool("IsOpen", false);
    }
    #endregion
}
