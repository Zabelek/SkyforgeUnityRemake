using EZhex1991.EZSoftBone;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class OutfitManager : MonoBehaviour
{
    #region Variables
    private Transform[] _bones;
    private Transform _rootBone;
    [SerializeField] private CharacterBehaviour _character;
    [SerializeField] private SkinnedMeshRenderer _fullBodyMesh;
    [SerializeField] private SkinnedMeshRenderer _noLegsAndHandsMesh;
    [SerializeField] private SkinnedMeshRenderer _onlyHeadAndNeckMesh;
    [SerializeField] private SkinnedMeshRenderer _handsFeetHalf;
    private List<OutfitBehaviour> _wornOutfits;
    public bool _profileSyncAtStart;
    private List<CapsuleCollider> _currentClothColliders;
    #endregion

    #region Mono
    private void Awake()
    {
        //everything here is temporary and will be changed once a proper outfit system is implemented
        if (_wornOutfits == null)
            _wornOutfits = new();
        _currentClothColliders = new();
        _bones = _fullBodyMesh.bones;
        _rootBone = _fullBodyMesh.rootBone;
        if (_profileSyncAtStart && SkyforgeLoader.CurrentProfile!=null)
        {
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
        //Sometimes worn outfits contain destroyed game objects, which are null, but have assigned values in the sane time. They have to be manually removed (Unity is fun)
        try
        {
            if(previous == null && previous.OutfitSO!=null)
            {
                _wornOutfits.Remove(previous);
            }
        }
        catch { }
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
            foreach (var outfitMesh in previous.OutfitMeshes)
            {
                if (outfitMesh.TryGetComponent<Cloth>(out Cloth cloth))
                {
                    RemoveExtraCollider(cloth);
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
                AddExtraCollider(cloth);
            }
        }
        if (newMesh.TryGetComponent<ExtraRigMesh>(out ExtraRigMesh rigMesh))
        {
            AddExtraRig(rigMesh, tempBones);
        }
        var rigToDestroy = newMesh.transform.GetComponentsInChildren<Transform>().FirstOrDefault(c => c.name.Contains("_Rig"));
        if(rigToDestroy!= null)
        {
            Destroy(rigToDestroy.gameObject);
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
                if (bone.TryGetComponent<EZSoftBone>(out EZSoftBone softbone))
                {
                    AddExtraCollider(softbone);
                }
            }
            bool first = true;
            foreach (var outfitMesh in mesh.OutfitBehaviour.OutfitMeshes)
            {
                if(first)
                {
                    var newBones = outfitMesh.bones.ToList();
                    foreach (var childbone in mesh.ExtraRigRoot.GetComponentsInChildren<Transform>())
                        newBones.Add(childbone);
                    for (int i = 0; i < tempBones.Length; i++)
                    {
                        tempBones[i] = newBones.FirstOrDefault(b => b.name == tempBones[i].name);
                    }
                    first = false;
                }
                outfitMesh.bones = tempBones;
            }
        }
    }
    private void AddExtraCollider(EZSoftBone softbone)
    {
        //EZ soft bone has to reference the colliders that are in the character's main skeleton, not in the prefab's skeleton
        CapsuleCollider[] capsules = new CapsuleCollider[softbone.extraColliders.Count];
        for (int i = 0; i <softbone.extraColliders.Count; i++)
        {
            capsules[i] = (softbone.extraColliders[i] as CapsuleCollider);
        }
        capsules = AssignColliders(capsules.ToList());
        softbone.extraColliders.Clear();
        foreach (var capsule in capsules)
        {
            softbone.extraColliders.Add(capsule);
        }
    }
    private void AddExtraCollider(Cloth cloth)
    {
        //Cloth component has to reference the colliders that are in the character's main skeleton, not in the prefab's skeleton
        cloth.capsuleColliders = AssignColliders(cloth.capsuleColliders.ToList());
    }
    private CapsuleCollider[] AssignColliders(List<CapsuleCollider> capsuleColliders)
    {
        CapsuleCollider[] capsules = new CapsuleCollider[capsuleColliders.Count];
        for (int i = 0; i < capsuleColliders.Count(); i++)
        {
            var destinationBone = _bones.FirstOrDefault(b => b.name == capsuleColliders[i].transform.parent.name);
            if (destinationBone != null)
            {
                var currentCol = _currentClothColliders.FirstOrDefault(c => c.name == capsuleColliders[i].name);
                if (currentCol == null)
                {
                    capsules[i] = Instantiate(capsuleColliders[i], destinationBone);
                    capsules[i].name = capsules[i].name.Replace("(Clone)", "");
                    _currentClothColliders.Add(capsules[i]);
                }
                else
                    capsules[i] = currentCol;
            }
            else
            {
                Debug.Log("ERROR! Bone names don't match in the outfit!");
            }
        }
        return capsules;
    }
    private void RemoveExtraCollider(Cloth cloth)
    {
        foreach (var collider in cloth.capsuleColliders)
        {
            if (!collider.IsDestroyed())
            {
                _currentClothColliders.Remove(collider);
                Destroy(collider.gameObject);
            }
        }
    }
    private void RemoveExtraCollider(EZSoftBone softbone)
    {
        foreach (var collider in softbone.extraColliders)
        {
            if (!collider.IsDestroyed())
            {
                _currentClothColliders.Remove(collider as CapsuleCollider);
                Destroy(collider.gameObject);
            }
        }
    }
    private void RemoveExtraRig(ExtraRigMesh mesh)
    {
        //On custom-rigged outfit peace destruction,it's additional bones have to be removed as well to keep the skeleton clean
        var softboneList = mesh.ExtraRigRoot.gameObject.GetComponentsInChildren<EZSoftBone>();
        foreach(var softbone in softboneList)
        {
            RemoveExtraCollider(softbone);
        }
        Destroy(mesh.ExtraRigRoot.gameObject);
    }
    private void CheckCoverTypes()
    {
        //In the future, here will be more complex script that will decide what type of body and hair mesh has to be activated, based on the clothing to prevent clipping through
        bool fullBodyCovered = false;
        bool handsLegsBreastCovered = false;
        bool headCovered = false;
        bool handsFeetHalf = false;
        foreach (var outfit in _wornOutfits)
        {
            if (outfit.OutfitSO.Covers == OutfitSO.CoverType.Full_Body)
            {
                fullBodyCovered = true;
            }
            if (outfit.OutfitSO.Covers == OutfitSO.CoverType.Legs_Arms_Breast)
            {
                handsLegsBreastCovered = true;
            }
            if (outfit.OutfitSO.Covers == OutfitSO.CoverType.Full_Head)
            {
                headCovered = true;
            }            
            if (outfit.OutfitSO.Covers == OutfitSO.CoverType.Hands_Feet_Half)
            {
                handsFeetHalf = true;
            }
        }
        _fullBodyMesh.gameObject.SetActive(false);
        _noLegsAndHandsMesh.gameObject.SetActive(false);
        _handsFeetHalf.gameObject.SetActive(false);
        if (handsLegsBreastCovered)
        {
            _noLegsAndHandsMesh.gameObject.SetActive(true);
        }
        else if (handsFeetHalf)
        {
            _handsFeetHalf.gameObject.SetActive(true);
        }
        else if(fullBodyCovered==false)
        {
            _fullBodyMesh.gameObject.SetActive(true);
        }
        if (headCovered)
        {
            _onlyHeadAndNeckMesh.gameObject.SetActive(false);
        }
        else
        {
            _onlyHeadAndNeckMesh.gameObject.SetActive(true);
        }
    }
    #endregion
}
