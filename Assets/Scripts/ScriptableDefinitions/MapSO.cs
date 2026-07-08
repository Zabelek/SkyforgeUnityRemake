using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/MapSO")]
public class MapSO : ScriptableObject
{
    [Tooltip("Scene name exactly as is in the Scene List in Build Profile")]
    public string SceneName;
    public string DisplayName;
    [Tooltip("The thumbnail that will be displayed on map selection or loading screen")]
    public Sprite Thumbnail;
}
