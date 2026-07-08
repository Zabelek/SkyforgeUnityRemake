using UnityEngine;

public class ExtraRigMesh : MonoBehaviour
{
    #region Variables
    //Helper class to handle extra bones by the Outfit Manager.
    [Tooltip("The highest object in the hierarchy of extra bones. The bone that is a parent to all other extra bones")]
    public Transform ExtraRigRoot;
    [Tooltip("The place in the original skeleton where all the extra bones should go to. Example: When extra bones are related to moving parts of helmet, you should put a head bone here. It can be a bone from other skeleton, as long as bone's name match the original")]
    public Transform ExtraRigParentDestination;
    [HideInInspector] public OutfitBehaviour OutfitBehaviour;
    #endregion

    #region Mono
    private void Awake()
    {
        OutfitBehaviour = GetComponent<OutfitBehaviour>();
    }
    #endregion
}
