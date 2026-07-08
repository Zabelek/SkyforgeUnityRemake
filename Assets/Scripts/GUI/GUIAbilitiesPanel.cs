using UnityEngine;

public class GUIAbilitiesPanel : MonoBehaviour
{
    #region Variables
    private PlayerBehaviour _player;
    //Abilities
    [SerializeField] private GUIAbilityWidget _ability1;
    [SerializeField] private GUIAbilityWidget _ability2;
    [SerializeField] private GUIAbilityWidget _ability3;
    [SerializeField] private GUIAbilityWidget _ability4;
    [SerializeField] private GUIAbilityWidget _ability5;
    [SerializeField] private GUIAbilityWidget _ability6;
    [SerializeField] private GUIAbilityWidget _ability7;
    [SerializeField] private GUIAbilityWidget _ability8;
    [SerializeField] private GUIAbilityWidget _ability9;
    //combo
    [SerializeField] private GUIComboGroup _mouseGroup1;
    [SerializeField] private GUIComboGroup _mouseGroup2;
    [SerializeField] private GUIComboGroup _mouseGroup3;
    [SerializeField] private GUIComboGroup _mouseGroup4;
    #endregion

    #region Mono
    void Update()
    {
        UpdateMouseCombo();
        UpdateAbilityWidgets();
    }
    #endregion

    #region Methods
    public void UpdateMouseCombo()
    {
        if (!_player.IsDead)
        {
            if (_player.GetHeroClass().CurrentlyPerformedComboState == HeroClassBehaviour.ComboState.L)
            {
                _mouseGroup1.Hide();
                _mouseGroup2.Show();
                _mouseGroup3.Hide();
                _mouseGroup4.Hide();
            }
            else if (_player.GetHeroClass().CurrentlyPerformedComboState == HeroClassBehaviour.ComboState.LL)
            {
                _mouseGroup1.Hide();
                _mouseGroup2.Hide();
                _mouseGroup3.Show();
                _mouseGroup4.Hide();
            }
            else if (_player.GetHeroClass().CurrentlyPerformedComboState == HeroClassBehaviour.ComboState.LLL)
            {
                _mouseGroup1.Hide();
                _mouseGroup2.Hide();
                _mouseGroup3.Hide();
                _mouseGroup4.Show();
            }
            else
            {
                _mouseGroup1.Show();
                _mouseGroup2.Hide();
                _mouseGroup3.Hide();
                _mouseGroup4.Hide();
            }
        }
    }
    public void UpdateAbilityWidgets()
    {
        _ability1.CheckAvailability();
        _ability2.CheckAvailability();
        _ability3.CheckAvailability();
        _ability4.CheckAvailability();
        _ability5.CheckAvailability();
        _ability6.CheckAvailability();
        _ability7.CheckAvailability();
        _ability8.CheckAvailability();
        _ability9.CheckAvailability();
    }
    public void SetPlayer(PlayerBehaviour player)
    {
        _player = player;
    }
    #endregion
}
