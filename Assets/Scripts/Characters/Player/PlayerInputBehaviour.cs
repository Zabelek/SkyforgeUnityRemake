using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputBehaviour : MonoBehaviour
{
    #region Variables
    public event EventHandler OnDrawAction;
    public event EventHandler OnLeftClickAction;
    public event EventHandler OnRightClickAction;
    public event EventHandler OnLeftHoldReleaseAction;
    public event EventHandler OnRightHoldReleaseAction;
    public event EventHandler OnEmoteClickAction;
    public event EventHandler OnDashClickAction;
    public event EventHandler OnAbility1Action;
    public event EventHandler OnAbility2Action;
    public event EventHandler OnAbility3Action;
    public event EventHandler OnAbility4Action;
    public event EventHandler OnAbility5Action;
    public event EventHandler OnAbility6Action;
    public event EventHandler OnAbility7Action;
    public event EventHandler OnAbility8Action;
    public event EventHandler OnAbility9Action;
    public event EventHandler OnAbility1ReleaseAction;
    public event EventHandler OnAbility2ReleaseAction;
    public event EventHandler OnAbility3ReleaseAction;
    public event EventHandler OnAbility4ReleaseAction;
    public event EventHandler OnAbility5ReleaseAction;
    public event EventHandler OnAbility6ReleaseAction;
    public event EventHandler OnAbility7ReleaseAction;
    public event EventHandler OnAbility8ReleaseAction;
    public event EventHandler OnAbility9ReleaseAction;
    public event EventHandler OnAppExitAction;
    private bool _ability1Pressed, _ability2Pressed, _ability3Pressed, _ability4Pressed, _ability5Pressed, _ability6Pressed,
        _ability7Pressed, _ability8Pressed, _ability9Pressed;
    public event EventHandler OpenMenuAction;
    public event EventHandler OpenSettingsAction;
    public event EventHandler OnInteractionAction;
    private PlayerInputActions _inputActions;
    #endregion

    #region Mono
    private void Awake()
    {
        _inputActions = new();
        _inputActions.Enable();
        _inputActions.Default.DrawWeapon.performed += DrawWeapon_Perfordmed;
        _inputActions.Default.LeftClick.performed += LeftClick_Perfordmed;
        _inputActions.Default.RightClick.performed += RightClick_Performed;
        _inputActions.Default.LeftClick.canceled += LeftClick_Released;
        _inputActions.Default.RightClick.canceled += RightClick_Released;
        _inputActions.Default.Emote.performed += Emote_Performed;
        _inputActions.Default.Ability1.performed += Ability1_Performed;
        _inputActions.Default.Ability2.performed += Ability2_Performed;
        _inputActions.Default.Ability3.performed += Ability3_Performed;
        _inputActions.Default.Ability4.performed += Ability4_Performed;
        _inputActions.Default.Ability5.performed += Ability5_Performed;
        _inputActions.Default.Ability6.performed += Ability6_Performed;
        _inputActions.Default.Ability7.performed += Ability7_Performed;
        _inputActions.Default.Ability8.performed += Ability8_Performed;
        _inputActions.Default.Ability9.performed += Ability9_Performed;
        _inputActions.Default.OpenMenu.performed += OpenMenu_Performed;
        _inputActions.Default.OpenSettings.performed += OpenSettings_Performed;
        _inputActions.Default.Ability1.canceled += Ability1_Released;
        _inputActions.Default.Ability2.canceled += Ability2_Released;
        _inputActions.Default.Ability3.canceled += Ability3_Released;
        _inputActions.Default.Ability4.canceled += Ability4_Released;
        _inputActions.Default.Ability5.canceled += Ability5_Released;
        _inputActions.Default.Ability6.canceled += Ability6_Released;
        _inputActions.Default.Ability7.canceled += Ability7_Released;
        _inputActions.Default.Ability8.canceled += Ability8_Released;
        _inputActions.Default.Ability9.canceled += Ability9_Released;
        _inputActions.Default.Dash.performed += Dash_Performed;
        _inputActions.Default.Interaction.performed += Interaction_Performed;
        _inputActions.Default.ExitGame.performed += AppExit_Performed;
    }

    private void OpenSettings_Performed(InputAction.CallbackContext context)
    {
        OpenSettingsAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        _inputActions.Default.DrawWeapon.performed -= DrawWeapon_Perfordmed;
        _inputActions.Default.LeftClick.performed -= LeftClick_Perfordmed;
        _inputActions.Default.RightClick.performed -= RightClick_Performed;
        _inputActions.Default.LeftClick.canceled -= LeftClick_Released;
        _inputActions.Default.RightClick.canceled -= RightClick_Released;
        _inputActions.Default.Emote.performed -= Emote_Performed;
        _inputActions.Default.Ability1.performed -= Ability1_Performed;
        _inputActions.Default.Ability2.performed -= Ability2_Performed;
        _inputActions.Default.Ability3.performed -= Ability3_Performed;
        _inputActions.Default.Ability4.performed -= Ability4_Performed;
        _inputActions.Default.Ability5.performed -= Ability5_Performed;
        _inputActions.Default.Ability6.performed -= Ability6_Performed;
        _inputActions.Default.Ability7.performed -= Ability7_Performed;
        _inputActions.Default.Ability8.performed -= Ability8_Performed;
        _inputActions.Default.Ability9.performed -= Ability9_Performed;
        _inputActions.Default.OpenMenu.performed -= OpenMenu_Performed;
        _inputActions.Default.Ability1.canceled -= Ability1_Released;
        _inputActions.Default.Ability2.canceled -= Ability2_Released;
        _inputActions.Default.Ability3.canceled -= Ability3_Released;
        _inputActions.Default.Ability4.canceled -= Ability4_Released;
        _inputActions.Default.Ability5.canceled -= Ability5_Released;
        _inputActions.Default.Ability6.canceled -= Ability6_Released;
        _inputActions.Default.Ability7.canceled -= Ability7_Released;
        _inputActions.Default.Ability8.canceled -= Ability8_Released;
        _inputActions.Default.Ability9.canceled -= Ability9_Released;
        _inputActions.Default.Dash.performed -= Dash_Performed;
        _inputActions.Default.Interaction.performed -= Interaction_Performed;
        _inputActions.Disable();
    }
    #endregion

    #region EventHandlers
    private void Interaction_Performed(InputAction.CallbackContext context)
    {
        OnInteractionAction?.Invoke(this, EventArgs.Empty);
    }
    private void Dash_Performed(InputAction.CallbackContext context)
    {
        OnDashClickAction?.Invoke(this, EventArgs.Empty);
    }
    private void Ability9_Released(InputAction.CallbackContext context)
    {
        OnAbility9ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability9Pressed = false;
    }
    private void Ability8_Released(InputAction.CallbackContext context)
    {
        OnAbility8ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability8Pressed = false;
    }
    private void Ability7_Released(InputAction.CallbackContext context)
    {
        OnAbility7ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability7Pressed = false;
    }
    private void Ability6_Released(InputAction.CallbackContext context)
    {
        OnAbility6ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability6Pressed = false;
    }
    private void Ability5_Released(InputAction.CallbackContext context)
    {
        OnAbility5ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability5Pressed = false;
    }
    private void Ability4_Released(InputAction.CallbackContext context)
    {
        OnAbility4ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability4Pressed = false;
    }
    private void Ability3_Released(InputAction.CallbackContext context)
    {
        OnAbility3ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability3Pressed = false;
    }
    private void Ability2_Released(InputAction.CallbackContext context)
    {
        OnAbility2ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability2Pressed = false;
    }
    private void Ability1_Released(InputAction.CallbackContext context)
    {
        OnAbility1ReleaseAction?.Invoke(this, EventArgs.Empty);
        _ability1Pressed = false;
    }
    private void OpenMenu_Performed(InputAction.CallbackContext context)
    {
        OpenMenuAction?.Invoke(this, EventArgs.Empty);
        _ability1Pressed = true;
    }
    private void Ability1_Performed(InputAction.CallbackContext context)
    {
        OnAbility1Action?.Invoke(this, EventArgs.Empty);
        _ability1Pressed = true;
    }
    private void Ability2_Performed(InputAction.CallbackContext context)
    {
        OnAbility2Action?.Invoke(this, EventArgs.Empty);
        _ability2Pressed = true;
    }
    private void Ability3_Performed(InputAction.CallbackContext context)
    {
        OnAbility3Action?.Invoke(this, EventArgs.Empty);
        _ability3Pressed = true;
    }
    private void Ability4_Performed(InputAction.CallbackContext context)
    {
        OnAbility4Action?.Invoke(this, EventArgs.Empty);
        _ability4Pressed = true;
    }
    private void Ability5_Performed(InputAction.CallbackContext context)
    {
        OnAbility5Action?.Invoke(this, EventArgs.Empty);
        _ability5Pressed = true;
    }
    private void Ability6_Performed(InputAction.CallbackContext context)
    {
        OnAbility6Action?.Invoke(this, EventArgs.Empty);
        _ability6Pressed = true;
    }
    private void Ability7_Performed(InputAction.CallbackContext context)
    {
        OnAbility7Action?.Invoke(this, EventArgs.Empty);
        _ability7Pressed = true;
    }
    private void Ability8_Performed(InputAction.CallbackContext context)
    {
        OnAbility8Action?.Invoke(this, EventArgs.Empty);
        _ability6Pressed = true;
    }
    private void Ability9_Performed(InputAction.CallbackContext context)
    {
        OnAbility9Action?.Invoke(this, EventArgs.Empty);
        _ability7Pressed = true;
    }
    private void Emote_Performed(InputAction.CallbackContext context)
    {
        OnEmoteClickAction?.Invoke(this, EventArgs.Empty);
    }
    private void RightClick_Released(InputAction.CallbackContext context)
    {
        OnRightHoldReleaseAction?.Invoke(this, EventArgs.Empty);
    }
    private void LeftClick_Released(InputAction.CallbackContext context)
    {
        OnLeftHoldReleaseAction?.Invoke(this, EventArgs.Empty);
    }
    private void RightClick_Performed(InputAction.CallbackContext obj)
    {
        OnRightClickAction?.Invoke(this, EventArgs.Empty);
    }
    private void LeftClick_Perfordmed(InputAction.CallbackContext context)
    {
        OnLeftClickAction?.Invoke(this, EventArgs.Empty);
    }
    private void DrawWeapon_Perfordmed(InputAction.CallbackContext context)
    {
        OnDrawAction?.Invoke(this, EventArgs.Empty);
    }
    private void AppExit_Performed(InputAction.CallbackContext context)
    {
        OnAppExitAction?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Methods
    public Vector2 GetRunVector()
    {
        return _inputActions.Default.Run.ReadValue<Vector2>().normalized;
    }
    public bool GetAbilityPressed(int abilityNumber)
    {
        return abilityNumber switch
        {
            1 => _ability1Pressed,
            2 => _ability2Pressed,
            3 => _ability3Pressed,
            4 => _ability4Pressed,
            5 => _ability5Pressed,
            6 => _ability6Pressed,
            7 => _ability7Pressed,
            8 => _ability8Pressed,
            9 => _ability9Pressed,
            _ => false
        };
    }
    #endregion
}
