using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/ItemSO/ItemSO")]
public class ItemSO : ScriptableObject
{
    public enum RarityLevel { Common, Uncommon, Rare, Epic, Legendary }
    public string ID;
    public string Name;
    public string Description;
    public Sprite InterfaceSprite;
    public bool IsStackable = false;
    public RarityLevel Rarity;
    public bool CanBeQuickAccessed = false;
}