using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/FactionSO")]
public class FactionSO : ScriptableObject
{
    public enum Type
    {
        Aelion, Pythonides
    }
    public string Name;
    [Tooltip("General faction, can be treated as a big container of all creatures coming from one planet")]
    public Type FactionType;
    [Tooltip("Specifies if characters with this class will attack those with classes that have different types")]
    public bool IsAggresive;
    [Tooltip("If not aggresive but has this checked, will attack once someone attack its allies")]
    public bool DefendsOwnKind;
    [Tooltip("The character won't attack anyone of the classes placed here")]
    public Type[] Allies;

}
