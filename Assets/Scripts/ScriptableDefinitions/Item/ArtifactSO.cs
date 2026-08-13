using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/ItemSO/ArtifactSO")]
public class ArtifactSO : ItemSO
{
    #region Variables
    public float HealthBonus;
    public float DamageBonus;
    [SerializeField] private GearPeaceBehaviour _prefab;
    #endregion

    #region Getters
    public GearPeaceBehaviour GetPrefab()
    {
        return _prefab;
    }
    #endregion
}
