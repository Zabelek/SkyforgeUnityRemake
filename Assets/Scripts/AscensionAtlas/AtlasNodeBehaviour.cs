using System;
using System.Linq;
using UnityEngine;

public class AtlasNodeBehaviour : MonoBehaviour
{
    public const float DOUBLE_CLICK_TIME = 0.3f;
    public static Color ACTIVE_COLOR = new Color(1, 0.988f, 0.694f, 1);
    public static Color INACTIVE_COLOR = new Color(0.792f, 0.792f, 0.792f, 1);

    #region Variables
    public EventHandler OnActivation;
    [SerializeField] protected AscensionAtlasBehaviour _atlas;
    [SerializeField] protected SpriteRenderer _displayIcon, _activeBorderIcon;
    [SerializeField] protected Transform _activeBorderMesh;
    [SerializeField] protected AtlasNodeUnlockVisualEffect _unlockEffectBase;
    [SerializeField] protected SoundEffectSO _unlockSound, _lockedClickSound;
    private Sprite _regularSprite, _activatedSprite;
    public PerkSO PerkSO;
    private bool _isMouseHovered;
    public bool IsActive { get; protected set; }
    private float _doubleClickTimer;
    public AtlasNodeBehaviour[] ChildNodes;
    #endregion

    #region Mono
    protected void Awake()
    {
        if(PerkSO != null)
        {
            if(PerkSO.CustomAtlasIcon != null)
            {
                _regularSprite = PerkSO.CustomAtlasIcon;
            }
            else if (PerkSO.AtlasIcon == PerkSO.AtlasIconType.Ball)
            {
                _regularSprite = _atlas.IconBallSprite;
                _activatedSprite = _atlas.IconBallSpriteSelected;
            }
            else if (PerkSO.AtlasIcon == PerkSO.AtlasIconType.Cross)
            {
                _regularSprite = _atlas.IconCrossSprite;
                _activatedSprite = _atlas.IconCrossSpriteSelected;
            }
            else
            {
                _regularSprite = _atlas.IconBoxSprite;
                _activatedSprite = _atlas.IconBoxSpriteSelected;
            }
        }
        else
        {
            _regularSprite = _atlas.IconBoxSprite;
            _activatedSprite = _atlas.IconBoxSpriteSelected;
        }
        SetToActive(false);
        SetDisplayIconSprite();
        _isMouseHovered = false;
        _doubleClickTimer = 0;
    }
    private void FixedUpdate()
    {
        if(_doubleClickTimer>0)
        {
            _doubleClickTimer -= Time.fixedDeltaTime;
            if (_doubleClickTimer < 0)
                _doubleClickTimer = 0;
        }
    }
    #endregion

    #region Methods
    private void SetDisplayIconSprite()
    {
        //Sprite Renderers have different sizes based on the image resolution. It needs to be rescaled to stay in bounds of the node
        Quaternion oldRotation = _displayIcon.transform.rotation;
        _displayIcon.transform.rotation = Quaternion.Euler(0, 0, 0);
        Vector3 oldSize = _displayIcon.bounds.size;
        _displayIcon.sprite = _regularSprite;
        var newSprite = _displayIcon.sprite;
        Vector3 newSize = _displayIcon.bounds.size;
        _displayIcon.transform.localScale = new Vector3(_displayIcon.transform.localScale.x * oldSize.x / newSize.x, _displayIcon.transform.localScale.y * oldSize.y / newSize.y,
            _displayIcon.transform.localScale.z * oldSize.z / newSize.z);
        _displayIcon.transform.rotation = oldRotation;
    }
    public virtual void SetToActive(bool active)
    {
        if(active)
        {
            IsActive = true;
            if (PerkSO.CustomAtlasIcon != null)
                _displayIcon.color = ACTIVE_COLOR;
            else
                _displayIcon.sprite = _activatedSprite;
            if (_activeBorderIcon != null)
                _activeBorderIcon.gameObject.SetActive(true);
            if (_activeBorderMesh != null)
                _activeBorderMesh.gameObject.SetActive(true);
        }
        else
        {
            IsActive = false;
            if (PerkSO.CustomAtlasIcon != null)
                _displayIcon.color = INACTIVE_COLOR;
            else
                _displayIcon.sprite = _regularSprite;
            if (_activeBorderIcon != null)
                _activeBorderIcon.gameObject.SetActive(false);
            if (_activeBorderMesh != null)
                _activeBorderMesh.gameObject.SetActive(false);
        }
        OnActivation?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region EventHandlers
    public virtual void OnPointerEnter()
    {
        _isMouseHovered = true;
        _atlas.NodeSelectionSprite.transform.position = this.transform.position;
        _atlas.NodeSelectionSprite.transform.localScale = new Vector3(1,1,1);
        _atlas.NodeSelectionSprite.SettActivated(IsActive);
        _atlas.NodeSelectionSprite.gameObject.SetActive(true);
    }
    public void OnPointerExit()
    {
        _isMouseHovered = false;
        _atlas.NodeSelectionSprite.gameObject.SetActive(false);
    }
    public void OnPointerUp()
    {
        if(_isMouseHovered && !IsActive)
        {
            if(_doubleClickTimer<=0)
            {
                _doubleClickTimer = DOUBLE_CLICK_TIME;
            }
            else
            {
                if (UnlockPerk())
                    SetToActive(true);
            }
        }
    }
    private bool UnlockPerk()
    {
        if (PerkSO != null)
        {
            if (SkyforgeLoader.CurrentProfile.GameplayResources.AelionEidoses >= PerkSO.EidosCost && !SkyforgeLoader.CurrentProfile.AcquiredPerks.Any(p => p.PerkID == PerkSO.ID)
                && _atlas.CheckChainUnlockCondition(this))
            {
                var effect = Instantiate(_unlockEffectBase, _atlas.transform);
                effect.transform.position = this.transform.position;
                var perkState = new UserProfile.PerkState() { PerkID = PerkSO.ID, Enabled = true };
                SoundManager.UIInstance.PlayGlobalSFX(_unlockSound);
                SkyforgeLoader.CurrentProfile.AcquiredPerks.Add(perkState);
                SkyforgeLoader.CurrentProfile.GameplayResources.AelionEidoses -= PerkSO.EidosCost;
                SkyforgeLoader.EnablePerk(PerkSO);
                SkyforgeLoader.PerksChanged = true;
                return true;
            }
            else
            {
                SoundManager.UIInstance.PlayGlobalSFX(_lockedClickSound);
            }
        }
        return false;
    }
    #endregion
}
