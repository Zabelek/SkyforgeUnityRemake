using System.Threading.Tasks;
using UnityEngine;

public class GUIGameMenuAbilitiesPanelControls : MonoBehaviour
{
    #region Variables
    [Tooltip("For each of the perk sets, there is one button of this kind needed")]
    [SerializeField] private GUISymbolDropDownIcon[] _dropDowns;
    [SerializeField] private GUIGameMenuEquipmentControls _equipmentControls;
    #endregion

    #region Methods
    public async Task UpdateView(bool animateRig)
    {
        foreach(var dropdown in _dropDowns)
        {
            dropdown.UpdatePerkList();
        }
        await _equipmentControls.UpdateCharacterForView(animateRig);
    }
    #endregion
}
