using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUIMenuRebirthMode : MonoBehaviour
{
    #region Variables
    [SerializeField] private Button _openButton, _closeButton, _weaponSlotButton, _armorSlotButton, _artifactSlotButton;
    [SerializeField] private GUIInventorySlot _rebirthSlot;
    [SerializeField] private CanvasGroup _rebirthControls;
    [SerializeField] private float _fadingTime, _buttonDespawnTime, _soundDelay;
    private bool _isFading, _isOpen;
    private float _currentFadeTime;
    [SerializeField] private GUIGameMenuEquipmentControls _equipmentControls;
    [SerializeField] private Transform _rebirthEffect;
    [SerializeField] private AudioClip _openClip, _closeClip, _generationUpClip;
    private bool _weaponAnimationPerformed, _armorAnimationPerformed, _artifactAnimationPerformed;
    #endregion

    #region Mono
    private void Awake()
    {
        _openButton.onClick.AddListener(OpenButton_Clicked);
        _closeButton.onClick.AddListener(CloseButton_Clicked);
        _weaponSlotButton.onClick.AddListener(WeaponButton_Clicked);
        _armorSlotButton.onClick.AddListener(ArmorButton_Clicked);
        _artifactSlotButton.onClick.AddListener(ArtifactButton_Clicked);
    }
    private void Update()
    {
        if(_isFading)
        {
            if (_isOpen)
                _currentFadeTime += Time.deltaTime;
            else
                _currentFadeTime -= Time.deltaTime;
            _rebirthControls.alpha = _currentFadeTime / _fadingTime;
            if (_isOpen == false && _currentFadeTime < 0)
            {
                _rebirthControls.alpha = 0;
                _rebirthControls.gameObject.SetActive(false);
                _isFading = false;
            }
            else if(_isOpen == true && _currentFadeTime > _fadingTime)
            {
                _rebirthControls.alpha = 1;
                _isFading = false;
            }
        }
    }
    #endregion

    #region Methods
    public void OpenRebirthView(bool immediate)
    {
        _isOpen = true;
        if (immediate)
        {
            _isFading = false;
        }
        else
        {
            _isFading = true;
            _currentFadeTime = 0;
        }
        _rebirthControls.gameObject.SetActive(true);
        _openButton.gameObject.SetActive(false);
        _weaponAnimationPerformed = false;
        _armorAnimationPerformed = false;
        _artifactAnimationPerformed = false;
        SoundManager.UIInstance.PlayGlobalSFX(_openClip);
        UpdateState();
    }
    public void CloseRebirthView(bool immediate, bool playSound)
    {
        _isOpen = false;
        if (immediate)
        {
            _isFading = false;
            _rebirthControls.gameObject.SetActive(false);
        }
        else
        {
            _isFading = true;
            _currentFadeTime = _fadingTime;
        }
        _openButton.gameObject.SetActive(true);
        if(playSound)
            SoundManager.UIInstance.PlayGlobalSFX(_closeClip);
    }
    private void UpdateState()
    {
        SetDisplayAny();
        if (SkyforgeLoader.CurrentProfile.Inventory.Slots.Any(s => s.Item != null && s.Item.ItemSO.ID == "Base_Resource_StarOfRebirth") &&
            SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item?.ItemUpgradeState.CanGenUp() == true)
        {
            _weaponSlotButton.gameObject.SetActive(true);
        }
        else
            _weaponSlotButton.gameObject.SetActive(false);
        if (SkyforgeLoader.CurrentProfile.Inventory.Slots.Any(s => s.Item != null && s.Item.ItemSO.ID == "Base_Resource_SealOfRebirth") &&
            SkyforgeLoader.CurrentProfile.Equipment.ArmorSlot.Item?.ItemUpgradeState.CanGenUp() == true)
        {
            _armorSlotButton.gameObject.SetActive(true);
        }
        else
            _armorSlotButton.gameObject.SetActive(false);
        if (SkyforgeLoader.CurrentProfile.Inventory.Slots.Any(s => s.Item != null && s.Item.ItemSO.ID == "Base_Resource_SparkleOfRebirth") &&
            SkyforgeLoader.CurrentProfile.Equipment.ArtifactSlot.Item?.ItemUpgradeState.CanGenUp() == true)
        {
            _artifactSlotButton.gameObject.SetActive(true);
        }
        else
            _artifactSlotButton.gameObject.SetActive(false);
    }
    #endregion

    #region DisplayMethods
    private void SetDisplay(string itemID)
    {
        var slot = SkyforgeLoader.CurrentProfile.Inventory.Slots.FirstOrDefault(s => s.Item!= null && s.Item.ItemSO.ID == itemID);
        if (slot != null)
            _rebirthSlot.InventorySlot = slot;
        else
            _rebirthSlot.InventorySlot = new InventorySlot();
        _rebirthSlot.UpdateSlot();
    }
    public void SetDisplayAny()
    {
        var slot = SkyforgeLoader.CurrentProfile.Inventory.Slots.FirstOrDefault(s => s.Item != null && ( s.Item.ItemSO.ID == "Base_Resource_StarOfRebirth"
        || s.Item.ItemSO.ID == "Base_Resource_SealOfRebirth" || s.Item.ItemSO.ID == "Base_Resource_SparkleOfRebirth" || s.Item.ItemSO.ID == "Base_Resource_FlameOfRebirth"
        || s.Item.ItemSO.ID == "Base_Resource_TearOfRebirth" || s.Item.ItemSO.ID == "Base_Resource_BudOfRebirth" || s.Item.ItemSO.ID == "Base_Resource_BreezeOfRebirth"));
        if (slot != null)
        {
            _rebirthSlot.InventorySlot = slot;
            _rebirthSlot.UpdateSlot();
        }
        else
        {
            _rebirthSlot.InventorySlot = new InventorySlot();
            _rebirthSlot.UpdateSlot();
        }
    }
    public void SetDisplayStarOfRebirth()
    {
        SetDisplay("Base_Resource_StarOfRebirth");
    }
    public void SetDisplaySealOfRebirth()
    {
        SetDisplay("Base_Resource_SealOfRebirth");
    }
    public void SetDisplaySparkleOfRebirth()
    {
        SetDisplay("Base_Resource_SparkleOfRebirth");
    }
    public void SetDisplayFlaneOfRebirth()
    {
        SetDisplay("Base_Resource_FlameOfRebirth");
    }
    public void SetDisplayTearOfRebirth()
    {
        SetDisplay("Base_Resource_TearOfRebirth");
    }
    public void SetDisplayBudOfRebirth()
    {
        SetDisplay("Base_Resource_BudOfRebirth");
    }
    public void SetDisplayBreezeOfRebirth()
    {
        SetDisplay("Base_Resource_BreezeOfRebirth");
    }
    #endregion

    #region EventHandlers
    private void OpenButton_Clicked()
    {
        OpenRebirthView(false);
    }
    private void CloseButton_Clicked()
    {
        CloseRebirthView(false, true);
    }
    private void SpawnEfefctsOnGenerationUp(Transform transformParent)
    {
        var effect = Instantiate(_rebirthEffect, this.transform);
        effect.transform.position = transformParent.position;
        effect.gameObject.SetActive(true);
        StartCoroutine(LateGenerationUpSoundPlay());
    }
    private IEnumerator SpawnLateEfefctsOnGenerationUp(GameObject gameObjectToHide)
    {
        yield return new WaitForSeconds(_buttonDespawnTime);
        gameObjectToHide.SetActive(false);
    }
    private IEnumerator LateGenerationUpSoundPlay()
    {
        yield return new WaitForSeconds(_soundDelay);
        SoundManager.UIInstance.PlayGlobalSFX(_generationUpClip);
    }
    private void WeaponButton_Clicked()
    {
        if(!_weaponAnimationPerformed)
        {
            _weaponAnimationPerformed = true;
            SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item.ItemUpgradeState.GenerationUp();
            SkyforgeLoader.CurrentProfile.Inventory.RemoveItem("Base_Resource_StarOfRebirth");
            SkyforgeLoader.EquipmentChanged = true;
            _equipmentControls.UpdateValues();
            _rebirthSlot.UpdateSlot();
            StartCoroutine(SpawnLateEfefctsOnGenerationUp(_weaponSlotButton.gameObject));
            SpawnEfefctsOnGenerationUp(_weaponSlotButton.transform);
        }
    }
    private void ArmorButton_Clicked()
    {
        if (!_armorAnimationPerformed)
        {
            _armorAnimationPerformed = true;
            SkyforgeLoader.CurrentProfile.Equipment.ArmorSlot.Item.ItemUpgradeState.GenerationUp();
            SkyforgeLoader.CurrentProfile.Inventory.RemoveItem("Base_Resource_SealOfRebirth");
            SkyforgeLoader.EquipmentChanged = true;
            _equipmentControls.UpdateValues();
            _rebirthSlot.UpdateSlot();
            StartCoroutine(SpawnLateEfefctsOnGenerationUp(_armorSlotButton.gameObject));
            SpawnEfefctsOnGenerationUp(_armorSlotButton.transform);
        }
    }
    private void ArtifactButton_Clicked()
    {
        if (!_artifactAnimationPerformed)
        {
            _artifactAnimationPerformed = true;
            SkyforgeLoader.CurrentProfile.Equipment.ArtifactSlot.Item.ItemUpgradeState.GenerationUp();
            SkyforgeLoader.CurrentProfile.Inventory.RemoveItem("Base_Resource_SparkleOfRebirth");
            SkyforgeLoader.EquipmentChanged = true;
            _equipmentControls.UpdateValues();
            _rebirthSlot.UpdateSlot();
            StartCoroutine(SpawnLateEfefctsOnGenerationUp(_artifactSlotButton.gameObject));
            SpawnEfefctsOnGenerationUp(_artifactSlotButton.transform);
        }
    }
    #endregion
}
