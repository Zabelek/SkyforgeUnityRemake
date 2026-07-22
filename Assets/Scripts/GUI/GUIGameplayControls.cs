using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class GUIGameplayControls : MonoBehaviour
{
    public const float MAX_INTERACTABLE_RANGE = 3;

    #region Variables
    [Header("GUI Base References")]
    [Tooltip("Drag here Player from the current scene")]
    [SerializeField] private PlayerBehaviour _player;
    [Tooltip("Player Input Behaviour should be a child of System object on the scene")]
    [SerializeField] private PlayerInputBehaviour _inputBehaviour;
    [Header("Effects and Stat Bars")]
    [Tooltip("The whole HP bar (parent of the other components)")]
    [SerializeField] private StatBarBehaviour _hpBar;
    [Tooltip("The whole Mana bar (parent of the other components)")]
    [SerializeField] private StatBarBehaviour _manaBar;
    [Tooltip("The panel displaying all active effects. May not be visible on the scene preview, as it's empty by default, but it should be inside the interface.")]
    [SerializeField] private GUIEffectsGroup _playerEffects;
    [Header("Enemy Selection")]
    [Tooltip("Normal version of the bar ar the top (when the character is selected by the player)")]
    [SerializeField] private GUICharacterTopBar _characterTopBar;
    [Tooltip("Boss version of the bar at the top. It's displayed when the character has more than 1 HP bar")]
    [SerializeField] private GUICharacterTopBar _bossTopBar;
    private List<CharacterBehaviour> _charactersInRange;
    private CharacterBehaviour _selectedCharacter;
    [Tooltip("When the game decides which character is selected by the player, it chechs how close they are to the center of the screen and also how close they are to the player. This weight determines how important is staying near the center of the screen for these calculations")]
    [SerializeField] private float _selectionScreenWeight = 1.0f;
    [Tooltip("When the game decides which character is selected by the player, it chechs how close they are to the center of the screen and also how close they are to the player. This weight determines how important is staying near the player for these calculations")]
    [SerializeField] private float _selectionDistanceWeight = 0.5f;
    [Tooltip("Max distance (to the player) at which the character will be selectable for the player")]
    [SerializeField] private float _selectionMaxTargetDistance = 20f;
    [Tooltip("Max distance (to the center of the screen) at which the character will be selectable for the player")]
    [SerializeField] private float _selectionMaxAngleTolerance = 0.2f;
    [Header("Abilities")]
    [Tooltip("Small bar above the HP that shows ability charging. Invisible by default")]
    [SerializeField] private CanvasGroup _abilityChargingGroup;
    [Tooltip("Fill image of the ability charging group")]
    [SerializeField] private Image _abilityChargingFill;
    [Tooltip("All the sounds played by the interface such as notifications, that are not referenced anywhere else in the interface components")]
    public SoundEffectSO[] SoundEffects;
    private GUIAbilitiesPanel _abilitiesPanel;
    [Header("Dash and Companion")]
    [SerializeField] private Image _dashFillImage;
    [SerializeField] private TextMeshProUGUI _dashNumberText;
    [SerializeField] private Image _dashIconImage;
    [SerializeField] private Image _companionFillImage;
    [SerializeField] private TextMeshProUGUI _companionNumberText;
    [SerializeField] private Image _companionIconImage;
    [Tooltip("Sprite dislplayed on the Dash Image when the dash is active")]
    [SerializeField] private Sprite _dashActiveSprite;
    [Tooltip("Sprite dislplayed on the Dash Image when the dash is locked")]
    [SerializeField] private Sprite _dashLockedSprite;
    [Tooltip("Sprite dislplayed on the Companion Image when the comapnion attack is active")]
    [SerializeField] private Sprite _companionActiveSprite;
    [Tooltip("Sprite dislplayed on the Companion Image when the comapnion attack is locked")]
    [SerializeField] private Sprite _companionLockedSprite;
    [Tooltip("A group comtaining all the dash and companion interface components")]
    [SerializeField] private Transform _comapnionAndDashGroup;
    [Header("Opportunity Buttons")]
    [Tooltip("Button that pops up when the opportunity ability is ready to use")]
    [SerializeField] Transform _opportunityButtonLeft;
    [Tooltip("Button that pops up when the opportunity ability is ready to use")]
    [SerializeField] Transform _opportunityButtonRight;
    [Tooltip("Image inside opportunity button that displays ability's image")]
    [SerializeField] Image _opportunityButtonLeftImage, _opportunityButtonRightImage;
    [Tooltip("Text box inside the opportunity button that displays keyboard key of the ability")]
    [SerializeField] TextMeshProUGUI _opportunityButtonLeftText, _opportunityButtonRightText;
    //Camera freeze on menu open
    private float _camFreezeHorizontalAxisValue, _camFreezeVerticalAxisValue, _camFreezeRadiusValue;
    [Header("Message Box")]
    [Tooltip("Box that pops up to the left of the screen when the character speaks to the player")]
    [SerializeField] private GUISpeakerMessageBox _characterMessageBox;
    [Header("Death Screen")]
    [Tooltip("Volume Control (should be in the System transform in the scene) used to modify color scheme when the player is near death")]
    [SerializeField] private Volume _volume;
    private ColorAdjustments _colorAdjustments;
    private Vignette _vignette;
    [Tooltip("The screen displayed when the player is, surprise surprise, dead")]
    [SerializeField] private Transform _deathScreen;
    [Tooltip("The camera that is set as an output camera in the GraphicsCompositor")]
    [SerializeField] private Transform _sceneRoot;
    [SerializeField] private SettingsManager _settingsManager;
    [Header("Interactable")]
    [Tooltip("Interactable widget is a small button that appears when E interaction is available")]
    [SerializeField] private Transform _interactableWidget;
    [Tooltip("Interactable widget is a small button that appears when E interaction is available")]
    [SerializeField] private TextMeshProUGUI _interactableWidgetText;
    private IPlayerInteractable _currentlySelectedInteractable;
    [Header("Menu Black Fade")]
    [Tooltip("Different black fade used for transition to menu")]
    [SerializeField] private GUISceneBlackFade _menuBlackFade;
    //so that the player can't open/close menu too fast
    private float _menuOpenDelay = 0.5f;
    #endregion

    #region Mono
    private void Awake()
    {
        SkyforgeLoader.GUIGameplayControls = this;
    }
    private void Start()
    {
        _charactersInRange = new();
        _player.OnCombatStartEvent += StartCombat;
        _player.OnCombatEndEvent += EndCombat;
        if (_inputBehaviour != null)
        {
            _inputBehaviour.OpenMenuAction += MenuOpenClose;
            _inputBehaviour.OpenSettingsAction += SettingsOpenClose;
            _inputBehaviour.OnAppExitAction += AppExit_Performed;
            _inputBehaviour.OnInteractionAction += Interaction_Performed;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _playerEffects.SetCharacter(_player);
        if(_volume != null)
        {
            _volume.profile.TryGet(out _colorAdjustments);
            _volume.profile.TryGet(out _vignette);
        }
        SkyforgeLoader.LoadedSceneReady = true;
    }
    private void Update()
    {
        UpdatePlayerStatus();
        UpdateSelectedCharacters();
        UpdateAbilityCharging();
        UpdateDashAndCompanion();
        UpdateOpportunityButtons();
        UpdateVolumeAndDeathScreen();
        if (_menuOpenDelay > 0)
            _menuOpenDelay -= Time.deltaTime;
    }
    private void OnDestroy()
    {
        SkyforgeLoader.GUIGameplayControls = null;
        if (_inputBehaviour != null)
        {
            _inputBehaviour.OpenMenuAction -= MenuOpenClose;
            _inputBehaviour.OpenSettingsAction -= SettingsOpenClose;
            _inputBehaviour.OnAppExitAction -= AppExit_Performed;
            _inputBehaviour.OnInteractionAction -= Interaction_Performed;
            _inputBehaviour.OnInteractionAction -= Interaction_Performed;
        }
    }
    #endregion

    #region Methods
    public async Task CloseMenu()
    {
        _sceneRoot.gameObject.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Globals.Instance.IsMenuOpen = false;
        FreezeCam(false);
        if (SkyforgeLoader.SettingsChanged)
        {
            _settingsManager.ApplySceneSettings();
        }
        if (SkyforgeLoader.PerksChanged)
        {
            _player.SyncPerks(true);
        }
        await _menuBlackFade.StartFadeOut();
    }
    public async Task OpenMenu()
    {
        _menuOpenDelay = 0.5f;
        FreezeCam(true);
        await _menuBlackFade.StartFadeIn();
        Globals.Instance.IsMenuOpen = true;
        _sceneRoot.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ShowCharacterMessage(VoicelineSO voiceline)
    {
        _characterMessageBox.SetMessage(voiceline);
    }
    private void UpdateVolumeAndDeathScreen()
    {
        if (_player != null && _colorAdjustments != null && _vignette != null)
        {
            if (_player.Stats.CurrentHP < _player.Stats.MaxHP / 4)
            {
                _colorAdjustments.saturation.value = -40 + (40 * ((float)_player.Stats.CurrentHP / (float)_player.Stats.MaxHP) * 4);
                _vignette.intensity.value = 0.58f - (0.58f * ((float)_player.Stats.CurrentHP / (float)_player.Stats.MaxHP) * 4);
                if (_player.Stats.CurrentHP <= 0)
                {
                    _colorAdjustments.saturation.value = -100;
                }
            }
            else
            {
                _colorAdjustments.saturation.value = 0;
                _vignette.intensity.value = 0;
            }
        }
        if (!_player.IsDead)
        {
            _deathScreen.gameObject.SetActive(false);
            _abilitiesPanel.gameObject.SetActive(true);
            _playerEffects.gameObject.SetActive(true);
        }
        else
        {
            _deathScreen.gameObject.SetActive(true);
            _abilitiesPanel.gameObject.SetActive(false);
            _playerEffects.gameObject.SetActive(false);
        }
    }
    private void UpdateOpportunityButtons()
    {
        if (_player.GetOpportunityAbilities(out var leftAbility, out var rightAbility, out var leftKey, out var rightKey))
        {
            SetRightOpportunityAbility(rightAbility, rightKey);
            SetLeftOpportunityAbility(leftAbility, leftKey);
        }
        else
        {
            _opportunityButtonLeft.gameObject.SetActive(false);
            _opportunityButtonRight.gameObject.SetActive(false);
        }
    }
    public void SetLeftOpportunityAbility(AbilityBehaviour ability, string key)
    {
        if (ability == null)
        {
            _opportunityButtonLeft.gameObject.SetActive(false);
        }
        else
        {
            _opportunityButtonLeft.gameObject.SetActive(true);
            if (ability is EscapeAbilityBehaviour)
                _opportunityButtonLeftImage.sprite = ability.AbilitySO.Icon;
            else
                _opportunityButtonLeftImage.sprite = ability.AbilitySO.Icon;
            _opportunityButtonLeftText.text = key;
        }
    }
    public void SetRightOpportunityAbility(AbilityBehaviour ability, string key)
    {
        if (ability == null)
        {
            _opportunityButtonRight.gameObject.SetActive(false);
        }
        else
        {
            _opportunityButtonRight.gameObject.SetActive(true);
            if(ability is EscapeAbilityBehaviour)
                _opportunityButtonRightImage.sprite = ability.AbilitySO.EscapeIcon;
            else
                _opportunityButtonRightImage.sprite = ability.AbilitySO.Icon;
            _opportunityButtonRightText.text = key;
        }
    }
    private void UpdateDashAndCompanion()
    {
        if (!_player.IsDead)
        {
            if (_comapnionAndDashGroup.gameObject.activeSelf == false)
            {
                _comapnionAndDashGroup.gameObject.SetActive(true);
            }
            if (_player.CompanionChargeMax > 0)
            {
                int availableAttacks = (int)(_player.CompanionCharge / 10);
                if (availableAttacks > 0)
                {
                    _companionIconImage.sprite = _companionActiveSprite;
                }
                else
                {
                    _companionIconImage.sprite = _companionLockedSprite;
                }
                _companionNumberText.text = availableAttacks.ToString();
                _companionFillImage.fillAmount = _player.CompanionCharge / _player.CompanionChargeMax;
            }
            if (_player.DashChargeMax > 0)
            {
                int availableDashes = (int)(_player.DashCharge / 10);
                if (availableDashes > 0)
                {
                    _dashIconImage.sprite = _dashActiveSprite;
                }
                else
                {
                    _dashIconImage.sprite = _dashLockedSprite;
                }
                _dashNumberText.text = availableDashes.ToString();
                _dashFillImage.fillAmount = _player.DashCharge / _player.DashChargeMax;
            }
        }
        else
        {
            _comapnionAndDashGroup.gameObject.SetActive(false);
        }
    }
    private void UpdateAbilityCharging()
    {
        if (!_player.IsDead)
        {
            if (_player.GetHeroClass().AbilityChargeMax != 0)
            {
                _abilityChargingGroup.gameObject.SetActive(true);
                _abilityChargingFill.fillAmount = _player.GetHeroClass().AbilityCharge / _player.GetHeroClass().AbilityChargeMax;
            }
            else
            {
                _abilityChargingGroup.gameObject.SetActive(false);
            }
        }
        else
        {
            _abilityChargingGroup.gameObject.SetActive(false);
        }
    }
    private void UpdatePlayerStatus()
    {
        if (!_player.IsDead)
        {
            if (_hpBar.gameObject.activeSelf == false)
                _hpBar.gameObject.SetActive(true);
            if (_manaBar.gameObject.activeSelf == false)
                _manaBar.gameObject.SetActive(true);
            _hpBar.SetValue(_player.Stats.CurrentHP, _player.Stats.MaxHP);
            _manaBar.SetValue(_player.Stats.CurrentMana, _player.Stats.MaxMana);
        }
        else
        {
            _hpBar.gameObject.SetActive(false);
            _manaBar.gameObject.SetActive(false);
        }
    }
    private void UpdateSelectedCharacters()
    {
        ClearCharactersInRange();
        AutoDeselectCharacter();
        if (!_player.IsDead)
        {
            //Code below determines which character is closest to the center of the screen to be selected
            Camera cam = Globals.Instance.ViewportCamera;
            Vector2 screenPoint = new Vector2(cam.pixelRect.x + cam.pixelRect.width * 0.5f, cam.pixelRect.y + cam.pixelRect.height * 0.55f);
            float maxScreenDistance = Vector2.Distance(screenPoint, new Vector2(cam.pixelRect.x, cam.pixelRect.y));
            Collider[] hits = Physics.OverlapSphere(_player.transform.position, _selectionMaxTargetDistance);
            float bestScoreForCharacters = float.MaxValue;
            CharacterBehaviour bestTargetForCharacters = null;
            float bestScoreForInteractables = float.MaxValue;
            IPlayerInteractable bestInteractable = null;
            float playerZCameraPos = Globals.Instance.ViewportCamera.WorldToScreenPoint(_player.transform.position).z;
            foreach (var hit in hits)
            {
                Vector3 screenPos = Globals.Instance.ViewportCamera.WorldToScreenPoint(hit.transform.position);
                // Ignore enemies behind the player
                if (screenPos.z < playerZCameraPos)
                    continue;
                Vector2 enemyScreenPos = new Vector2(screenPos.x, screenPos.y);
                float screenDistance = Vector2.Distance(enemyScreenPos, screenPoint);
                float screenDistanceNormalized = screenDistance / maxScreenDistance;
                float worldDistance = Vector3.Distance(_player.transform.position, hit.transform.position);
                float worldDistanceNormalized = Mathf.Clamp01(worldDistance / _selectionMaxTargetDistance);
                if (CharacterBehaviour.FindCharacterInCollider(hit, out CharacterBehaviour character) && character.Selectable)
                {
                    AddCharacterInRange(character);
                    if (character != _player)
                    {
                        var currentScore = (screenDistanceNormalized * _selectionScreenWeight) + (worldDistanceNormalized * _selectionDistanceWeight);
                        if (currentScore < bestScoreForCharacters && screenDistanceNormalized < _selectionMaxAngleTolerance)
                        {
                            bestScoreForCharacters = currentScore;
                            bestTargetForCharacters = character;
                        }
                    }
                }
                //if no luck with the character, try to look for IPlayerInteractable instead, if close enough
                else if(bestTargetForCharacters == null && (hit.transform.position - _player.transform.position).magnitude < MAX_INTERACTABLE_RANGE && hit.TryGetComponent<IPlayerInteractable>(out var interactable))
                {
                    var currentScore = (screenDistanceNormalized * _selectionScreenWeight) + (worldDistanceNormalized * _selectionDistanceWeight);
                    if (currentScore < bestScoreForInteractables && screenDistanceNormalized < _selectionMaxAngleTolerance)
                    {
                        bestScoreForInteractables = currentScore;
                        bestInteractable = interactable;
                    }
                }
            }
            if (bestTargetForCharacters != null)
            {
                AutoSelectCharacter(bestTargetForCharacters);
                if (bestTargetForCharacters.CharacterSO.Category.HealthBarsAmount == 1)
                {
                    _characterTopBar.gameObject.SetActive(true);
                    _bossTopBar.gameObject.SetActive(false);
                }
                else
                {
                    _characterTopBar.gameObject.SetActive(false);
                    _bossTopBar.gameObject.SetActive(true);
                }
            }
            else
            {
                _characterTopBar.gameObject.SetActive(false);
                _bossTopBar.gameObject.SetActive(false);
            }
            //if any interactoble found, display the widget
            if (bestInteractable != null)
            {
                _currentlySelectedInteractable = bestInteractable;
                _interactableWidget.gameObject.SetActive(true);
                _interactableWidgetText.text = bestInteractable.GetInteractionTitle();
            }
            else
            {
                _currentlySelectedInteractable = null;
                _interactableWidget.gameObject.SetActive(false);
            }
        }
        else
        {
            _characterTopBar.gameObject.SetActive(false);
            _bossTopBar.gameObject.SetActive(false);
        }
    }
    public void AddCharacterInRange(CharacterBehaviour character)
    {
        _charactersInRange.Add(character);
        if (character.TryGetComponent<GUICharacterBars>(out GUICharacterBars characterg))
        {
            characterg.IsInRange = true;
            characterg.IsHealthBarVisible = character.Stats.CurrentHP != character.Stats.MaxHP;
        }
    }
    public void RemoveCharacterInRange(CharacterBehaviour character)
    {
        _charactersInRange.Remove(character);
        if (character.TryGetComponent<GUICharacterBars>(out GUICharacterBars characterg))
        {
            characterg.IsInRange = false;
            characterg.IsHealthBarVisible = character.Stats.CurrentHP != character.Stats.MaxHP;
        }
    }
    public void ClearCharactersInRange()
    {
        foreach (var character in _charactersInRange)
        {
            if (character.TryGetComponent<GUICharacterBars>(out GUICharacterBars characterg))
            {
                characterg.IsInRange = false;
                characterg.IsHealthBarVisible = character.Stats.CurrentHP != character.Stats.MaxHP;
            }
        }
        _charactersInRange.Clear();
    }
    public void AutoSelectCharacter(CharacterBehaviour character)
    {
        _selectedCharacter = character;
        if (character.TryGetComponent<GUICharacterBars>(out GUICharacterBars characterg))
        {
            characterg.IsInRange = true;
            characterg.IsHealthBarVisible = true;
            characterg.IsAutoSelected = true;
            characterg.DisplayedDistance = (int)((_player.transform.position - character.transform.position).magnitude);
        }
        if (character.CharacterSO.Category.HealthBarsAmount == 1)
        {
            _characterTopBar.SetCharacter(character);
            _bossTopBar.SetCharacter(null);
        }
        else
        {
            _characterTopBar.SetCharacter(null);
            _bossTopBar.SetCharacter(character);
        }
        _player.SelectedCharacter = character;
    }
    public void AutoDeselectCharacter()
    {
        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<GUICharacterBars>(out GUICharacterBars characterg))
        {
            characterg.IsInRange = false;
            characterg.IsHealthBarVisible = false;
            characterg.IsAutoSelected = false;
        }
        _selectedCharacter = null;
        _player.SelectedCharacter = null;
        _characterTopBar.SetCharacter(null);
        _bossTopBar.SetCharacter(null);
    }
    #endregion

    #region EventHandlers
    private void MenuOpenClose(object sender, EventArgs e)
    {
        if (gameObject.activeSelf && !Globals.Instance.IsCutscenePlaying && _menuOpenDelay<=0)
        {
            if (Globals.Instance.IsMenuOpen == false)
            {
                _ = OpenMenuAndShowSystemView();
            }
        }
    }
    private async Task OpenMenuAndShowSystemView()
    {
        await SkyforgeLoader.SetMenuOpen(true);
        SkyforgeLoader.GUIGameMenu.ShowSystemView();
    }
    private void SettingsOpenClose(object sender, EventArgs e)
    {
        if (gameObject.activeSelf && !Globals.Instance.IsCutscenePlaying && _menuOpenDelay <= 0)
        {
            if (Globals.Instance.IsMenuOpen == false)
            {
                _ = OpenMenuAndShowSettingsView();
            }
        }
    }
    private async Task OpenMenuAndShowSettingsView()
    {
        await SkyforgeLoader.SetMenuOpen(true);
        SkyforgeLoader.GUIGameMenu.ShowSettingsView();
    }
    private void FreezeCam(bool freeze)
    {
        if(freeze)
        {
            var cinemachineCam = Globals.Instance.CurrentCinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
            if (cinemachineCam != null)
            {
                cinemachineCam.enabled = false;
                _camFreezeHorizontalAxisValue = cinemachineCam.HorizontalAxis.Value;
                _camFreezeVerticalAxisValue = cinemachineCam.VerticalAxis.Value;
                _camFreezeRadiusValue = cinemachineCam.Radius;
            }
        }
        else
        {
            var cinemachineCam = Globals.Instance.CurrentCinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
            var cinemachineZoom = Globals.Instance.CurrentCinemachineCamera.GetComponent<CinemachineCustomZoom>();
            if (cinemachineCam != null)
            {
                cinemachineCam.enabled = true;
                cinemachineCam.HorizontalAxis.Value = _camFreezeHorizontalAxisValue;
                cinemachineCam.VerticalAxis.Value = _camFreezeVerticalAxisValue;
                cinemachineCam.Radius = _camFreezeRadiusValue;
                cinemachineZoom.SetRadius(_camFreezeRadiusValue);
            }
        }
    }
    private void EndCombat(object sender, EventArgs e)
    {
        var sound = SoundEffects.FirstOrDefault(s => s.Name == "Combat_End");
        if(sound!=null)
        {
            Globals.Instance.SoundManager.PlayGlobalSFX(sound.AudioClips[UnityEngine.Random.Range(0, sound.AudioClips.Length)], sound.VolumeModifier);
        }
    }
    private void StartCombat(object sender, EventArgs e)
    {
        var sound = SoundEffects.FirstOrDefault(s => s.Name == "Combat_Start");
        if (sound != null)
        {
            Globals.Instance.SoundManager.PlayGlobalSFX(sound.AudioClips[UnityEngine.Random.Range(0, sound.AudioClips.Length)], sound.VolumeModifier);
        }
    }
    public void SetAbilitiesPanel(GUIAbilitiesPanel panel)
    {
        if (_abilitiesPanel != null)
            Destroy(_abilitiesPanel.gameObject);
        if(panel != null)
        {
            _abilitiesPanel = Instantiate(panel, this.transform);
            _abilitiesPanel.gameObject.SetActive(true);
            _abilitiesPanel.SetPlayer(_player);
        }
        else
            _abilitiesPanel = null;
    }
    private void AppExit_Performed(object sender, EventArgs e)
    {
        Application.Quit();
    }
    private void Interaction_Performed(object sender, EventArgs e)
    {
        if(!_player.IsDead && !Globals.Instance.IsCutscenePlaying && !Globals.Instance.IsMenuOpen && _currentlySelectedInteractable != null)
        {
            _currentlySelectedInteractable.Interact(_player);
            _player.PlayAnimation("Interaction");
        }
    }
    #endregion
}
