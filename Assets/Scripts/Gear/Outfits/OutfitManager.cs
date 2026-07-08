using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OutfitManager : MonoBehaviour
{
    #region Variables
    private Transform[] _bones;
    private Transform _rootBone;
    [SerializeField] private CharacterBehaviour _character;
    [SerializeField] private SkinnedMeshRenderer _fullBodyMesh;
    [SerializeField] private SkinnedMeshRenderer _onlyHeadAndNeckMesh;
    private List<OutfitBehaviour> _wornOutfits;
    [SerializeField] private CapsuleCollider[] _legColliders;
    public bool _profileSyncAtStart;
    #endregion

    #region Mono
    private void Awake()
    {
        //everything here is temporary and will be changed once a proper outfit system is implemented
        if (_wornOutfits == null)
            _wornOutfits = new();
        _bones = _fullBodyMesh.bones;
        _rootBone = _fullBodyMesh.rootBone;
        if (_profileSyncAtStart && SkyforgeLoader.CurrentProfile!=null)
        {
            _ = EquipOutfit(0, OutfitSO.OutfitSlot.Body);
            _ = EquipOutfit(SkyforgeLoader.CurrentProfile.HatNumber, OutfitSO.OutfitSlot.Head);
        }
    }
    #endregion

    #region Methods
    public async Task<OutfitBehaviour> EquipOutfit(int outfitID, OutfitSO.OutfitSlot slot)
    {
        if (_wornOutfits == null)
            _wornOutfits = new();
        var newMesh = await SkyforgeLoader.LoadOutfit(outfitID, slot, _character.transform);
        var previous = _wornOutfits.FirstOrDefault(o => o.OutfitSO.Slot == slot);
        if (previous != null && newMesh != null)
        {
            _wornOutfits.Remove(previous);
            Destroy(previous.gameObject);
            if(previous.TryGetComponent<ExtraRigMesh>(out ExtraRigMesh extraRigMesh))
            {
                if(extraRigMesh.ExtraRigRoot != null && extraRigMesh.ExtraRigParentDestination != null)
                {
                    RemoveExtraRig(extraRigMesh);
                }
            }
        }
        Transform[] tempBones = null;
        foreach (var outfitMesh in newMesh.OutfitMeshes)
        {
            tempBones = outfitMesh.bones;
            outfitMesh.bones = _bones;
            outfitMesh.rootBone = _rootBone;
            if (outfitMesh.TryGetComponent<Cloth>(out Cloth cloth))
            {
                try
                {
                    var capsules = new CapsuleCollider[4];
                    capsules[0] = _legColliders[0];
                    capsules[1] = _legColliders[1];
                    capsules[2] = _legColliders[2];
                    capsules[3] = _legColliders[3];
                    cloth.capsuleColliders = capsules;
                }
                catch { }
            }
        }
        if (newMesh.TryGetComponent<ExtraRigMesh>(out ExtraRigMesh rigMesh))
        {
            AddExtraRig(rigMesh, tempBones);
        }
        _wornOutfits.Add(newMesh);
        CheckCoverTypes();
        return newMesh;
    }
    private void AddExtraRig(ExtraRigMesh mesh, Transform[] tempBones)
    {
        //in case the outfit mesh has custom bones, they need to be added to the main skeleton and then re-assigned to the mesh in the correct order
        var parent = _bones.FirstOrDefault(b => b.name == mesh.ExtraRigParentDestination.name);
        if(parent!=null)
        {
            mesh.ExtraRigParentDestination = parent;
            mesh.ExtraRigRoot = Instantiate(mesh.ExtraRigRoot, mesh.ExtraRigParentDestination);
            foreach (Transform bone in mesh.ExtraRigRoot.GetComponentsInChildren<Transform>())
            {
                bone.name = bone.name.Replace("(Clone)", "");
            }
            foreach (var outfitMesh in mesh.OutfitBehaviour.OutfitMeshes)
            {
                var newBones = outfitMesh.bones.ToList();
                foreach (var childbone in mesh.ExtraRigRoot.GetComponentsInChildren<Transform>())
                    newBones.Add(childbone);
                for(int i=0; i<tempBones.Length; i++)
                {
                    tempBones[i] = newBones.FirstOrDefault(b => b.name == tempBones[i].name);
                }
                outfitMesh.bones = tempBones;
            }
        }
    }
    private void RemoveExtraRig(ExtraRigMesh mesh)
    {
        //On custom-rigged outfit peace destruction,it's additional bones have to be removed as well to keep the skeleton clean
        Destroy(mesh.ExtraRigRoot.gameObject);
    }
    private void CheckCoverTypes()
    {
        //In the future, here will be more complex script that will decide what type of body and hair mesh has to be activated, based on the clothing to prevent clipping through
        bool fullBodyCovered = false;
        foreach(var outfit in _wornOutfits)
        {
            if (outfit.OutfitSO.Covers == OutfitSO.CoverType.Full_Body)
            {
                fullBodyCovered = true;
            }
        }
        if (fullBodyCovered)
        {
            _fullBodyMesh.gameObject.SetActive(false);
            _onlyHeadAndNeckMesh.gameObject.SetActive(true);
        }
        else
        {
            _fullBodyMesh.gameObject.SetActive(true);
            _onlyHeadAndNeckMesh.gameObject.SetActive(false);
        }
    }
    #endregion
}
