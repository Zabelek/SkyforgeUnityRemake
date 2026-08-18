using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public const float DROP_AFTER_DEATH_TIME = 0.5f;

    #region Variables
    [Tooltip("If set to true, from all generated loot, only one will bew chosen to spawn. The amount of chosen item stack can still be greater than 1")]
    public bool DropsOneThing;
    public List<LootRecord> LootRecords { get; set; }
    [Tooltip("CharacterBehaviour from which the loot will drop on death")]
    [SerializeField] private CharacterBehaviour _character;
    [Tooltip("Prefab of lootbox to spawn")]
    [SerializeField] private LootBoxBehaviour _lootBoxBase;
    #endregion

    #region Mono
    private void Awake()
    {
        LootRecords = new();
        foreach(var record in GetComponentsInChildren<LootRecord>())
        {
            LootRecords.Add(record);
        }
        if(_character != null)
        {
            _character.OnDeathEvent += ScheduleDrop;
        }
    }
    #endregion

    #region Methods
    private IEnumerator HandleItemDrop()
    {
        //small delay so that the loot doesn't spawn immediately, as it looks a bit weird
        yield return new WaitForSeconds(DROP_AFTER_DEATH_TIME);
        if(_lootBoxBase != null)
        {
            List<Item> items = new();
            Dictionary<GameplayResources.ResourceType, int> resources = new();
            var droppedItems = DropItems();
            if (droppedItems.Any())
            {
                var lootbox = Instantiate(_lootBoxBase, this.transform.position, this.transform.rotation);
                lootbox.transform.position += new Vector3(0, 0.7f, 0);
                foreach (var item in droppedItems)
                {
                    lootbox.AddItem(item);
                }
            }
        }
    }
    public List<Item> DropItems()
    {
        List<Item> items = new();
        List<Item> ret = new();
        bool any = false;
        //Here the random drop chance is calculated for each drop record
        foreach (var lootRecord in LootRecords)
        {
            float chance = UnityEngine.Random.Range(0f, 1f);
            if (chance < lootRecord.Chance)
            {
                if (lootRecord.Item?.IsStackable == true)
                {
                    int amount = UnityEngine.Random.Range(lootRecord.MinAmount, lootRecord.MaxAmount+1);
                    items.Add(new Item(lootRecord.Item.ID, amount));
                    any = true;
                }
                else
                {
                    items.Add(new Item(lootRecord.Item.ID, 1));
                    any = true;
                }
            }
        }
        if (any)
        {
            if (DropsOneThing)
            {
                int indexToAdd = UnityEngine.Random.Range(0, items.Count);
                ret.Add(items[indexToAdd]);
            }
            else
            {
                foreach (var item in items)
                {
                    ret.Add(item);
                }
            }
        }
        return ret;
    }
    #endregion

    #region EventHandlers
    private void ScheduleDrop(object sender, EventArgs e)
    {
        StartCoroutine(HandleItemDrop());
    }
    #endregion
}
