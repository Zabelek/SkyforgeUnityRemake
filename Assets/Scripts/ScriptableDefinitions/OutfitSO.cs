using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/OutfitSO")]
public class OutfitSO : ScriptableObject
{
    public enum CoverType
    {
        Full_Body, Nothing, Full_Head
    }
    public enum OutfitSlot
    {
        Body, Head
    }
    [Tooltip("Used to determine which bodyparts should be hidden when the outfit is equipped. Exzample: If you create a bikini armor, you don't select FullBody so that the Outfit Manager doesn't hide the body while wearing said bikini armor. However, if you're creating a full armor, covering 100%, you will select full body, so that the body mesh is invisible and doesn't clip through the armor's mesh")]
    public CoverType Covers;
    [Tooltip("Slot that the outfit is taking. Two outfits can't be in the same slot, so if the player tried to wear something that's already occupied, the previous outfit peace will be removed")]
    public OutfitSlot Slot;
    public string Name;
    [Tooltip("Addressable name of the prefab. If you simplify the prefab name, it should be names the same as the prefab itself if it's unique. Without this field set, the outfit won't be loaded from Addressables")]
    public string Address;
    [Tooltip("Schould be qnique. For now, used by the save system. may be deleted in the future, as better save-outfit system will be developed")]
    public int ObjectID;
}
