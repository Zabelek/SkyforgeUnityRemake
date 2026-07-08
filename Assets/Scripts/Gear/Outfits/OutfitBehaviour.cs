using System.Collections.Generic;
using UnityEngine;

public class OutfitBehaviour : MonoBehaviour
{
    #region Variables
    public OutfitSO OutfitSO;
    public SkinnedMeshRenderer[] OutfitMeshes;
    private List<Cloth> _clothes;
    #endregion

    #region Mono
    protected virtual void Start()
    {
        _clothes = new();
        foreach (var mesh in OutfitMeshes)
        {
            if(mesh.TryGetComponent<Cloth>(out Cloth cloth))
            {
                _clothes.Add(cloth);
            }
        }
    }
    #endregion

    #region Methods
    public void PauseClothSimmulation()
    {
        foreach(var cloth in _clothes)
        {
            cloth.enabled = false;
        }
    }
    public void ResumeClothSimmulation()
    {
        foreach (var cloth in _clothes)
        {
            cloth.enabled = true;
        }
    }
    #endregion
}
