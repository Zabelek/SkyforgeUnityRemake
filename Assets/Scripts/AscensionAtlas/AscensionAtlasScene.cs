using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AscensionAtlasScene : MonoBehaviour
{
    #region Variables
    private List<AscensionAtlasBehaviour> _atlases;
    private AscensionAtlasBehaviour _currentlyDisplayedAtlas;
    [SerializeField] private SimpleStayInsideCollider _cinemachineTarget; 
    #endregion

    #region Mono 
    private void Awake()
    {
        _atlases = new();
        foreach (var atlas in GetComponentsInChildren<AscensionAtlasBehaviour>())
        {
            _atlases.Add(atlas);
        }
    }
    #endregion

    #region Methods
    public void UpdateUnlockedNodes()
    {
        _currentlyDisplayedAtlas.UpdateUnlockedNodes();
    }

    public void ShowCharacterAtlas()
    {
        foreach(var atlasToDisable in _atlases)
        {
            atlasToDisable.gameObject.SetActive(false);
        }
        var atlas = _atlases.FirstOrDefault(a => a.HeroClassSO == null);
        ShowAtlas(atlas);
    }
    public void ShowClassAtlas()
    {
        foreach (var atlasToDisable in _atlases)
        {
            atlasToDisable.gameObject.SetActive(false);
        }
        if (SkyforgeLoader.CurrentProfile != null)
        {
            var atlas = _atlases.FirstOrDefault(a => a.HeroClassSO?.ID == SkyforgeLoader.CurrentProfile.CurrentlyPickedClass);
            ShowAtlas(atlas);
        }
    }
    private void ShowAtlas(AscensionAtlasBehaviour atlas)
    {
        if (atlas != null)
        {
            atlas.gameObject.SetActive(true);
            _cinemachineTarget.Collider = atlas.AtlasBounds;
            _cinemachineTarget.transform.position = new Vector3(atlas.OriginPoint.position.x, _cinemachineTarget.transform.position.y, atlas.OriginPoint.position.z);
            _currentlyDisplayedAtlas = atlas;
        }
        UpdateUnlockedNodes();
    }
    public void SetCameraToCenter()
    {
        _cinemachineTarget.transform.position = new Vector3(_currentlyDisplayedAtlas.OriginPoint.position.x, _cinemachineTarget.transform.position.y, _currentlyDisplayedAtlas.OriginPoint.position.z);
    }
    #endregion
}
