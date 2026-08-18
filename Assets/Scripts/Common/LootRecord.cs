using UnityEngine;

public class LootRecord : MonoBehaviour
{
    #region Variables
    public ItemSO Item;
    [Tooltip("1 means 100%")]
    public float Chance;
    public short MinAmount, MaxAmount;
    #endregion
}
