using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/PerkSetSO")]
public class PerkSetSO : ScriptableObject
{
    public string Name;
    [Tooltip("Only one of these perks can be active in the same time")]
    public PerkSO[] Perks;
}
