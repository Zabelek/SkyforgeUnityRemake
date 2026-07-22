using System.Threading.Tasks;
using UnityEngine;

public class GUIGameMenuAbilitiesPanelControls : MonoBehaviour
{
    #region Variables
    [SerializeField] private OutfitManager _outfitManager;
    [SerializeField] private PlayerBehaviour _playerVisualization;
    [Tooltip("For each of the perk sets, there is one button of this kind needed")]
    [SerializeField] private GUISymbolDropDownIcon[] _dropDowns;
    #endregion

    #region Mono
    private void Awake()
    {
        _playerVisualization.SetAnimationState("Menu", true);
    }
    #endregion

    #region Methods
    public async Task UpdateCharacterForView(bool animateRig)
    {
        var task1 = _outfitManager.EquipOutfit(0, OutfitSO.OutfitSlot.Body);
        var task2 = _outfitManager.EquipOutfit(SkyforgeLoader.CurrentProfile.HatNumber, OutfitSO.OutfitSlot.Head);
        await Task.WhenAll(task1, task2);
        if (animateRig)
        {
            //this first line is needed so that the player immediately starts from the correct animation, not overriding it by anything
            _playerVisualization.SetAnimationState("Menu", true);
            _playerVisualization.PlayAnimation("MenuStart", true);
            _playerVisualization.ResetAnimation();
            //first setting weapon at draw state so that it doesn't play animation of drawing
            _playerVisualization.EquippedWeapon.SetWeaponDraw();
            _playerVisualization.ChangeWeaponOutState(true);
        }
    }
    public async Task UpdateView(bool animateRig)
    {
        foreach(var dropdown in _dropDowns)
        {
            dropdown.UpdatePerkList();
        }
        await UpdateCharacterForView(animateRig);
    }
    #endregion
}
