using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class SFEtherTransformerBehaviour : GIItemDragBehaviour, IPlayerInteractable, ISpecialInterfaceProvider
{
    public const float MAX_ENERGY_AMOUNT = 100;
    public const float MAX_UPGRADING_TIME = 15;

    #region ReferenceVariables
    [Header("Interface Related Variables")]
    [Tooltip("Camera that is going to be moved towards the machine on interaction")]
    [SerializeField] private CinemachineCamera _interfacecamera;
    [Tooltip("Gameplay controls instance of the scene (should be only one)")]
    [SerializeField] private GUIGameplayControls _gameplayControls;
    [Tooltip("Parent of the menu referenced below. It's needed so that it can return to its initial position after the interaction is over")]
    [SerializeField] private RectTransform _menuParent;
    [Tooltip("Menu that will be displayed when the machine is interacted with")]
    [SerializeField] private RectTransform _menu;
    [Tooltip("Text object that will display machine's messages")]
    [SerializeField] private TextMeshProUGUI _console;
    [Tooltip("Default scene black fade object")]
    [SerializeField] private GUISceneBlackFade _black;
    [SerializeField] private PlayerInputBehaviour _inputBehaviour;
    private bool alreadyFading = false;
    private SFEtherTransformerAnimationBehaviour _animationHandler;
    private SoundBundleBehaviour _sounds;
    [Header("Middle Keyboard")]
    [SerializeField] private Image _kingPointer;
    [SerializeField] private Image _assassinPointer, _judgePointer, _sagePointer;
    [SerializeField] private Button _upButton, _leftButton, _bottomButton, _rightButton;
    [Header("Transform Button And Lever")]
    [SerializeField] private Button _leverButton;
    [SerializeField] private Button _transformButton;
    //items
    private List<InventorySlot> _registeredUpgradableSlots, _registeredResourceSlots;
    [SerializeField] private GUIItemPickView _upgradableView, _resourceView;
    private ScrollRect _upgradableScrollRect, _resourceScrollRect;
    [SerializeField] private GUIItemSlotConnectedButton _upgradableSlotButton, _resourceSlotButton;
    private InventorySlot _upgradableSlot, _resourceSlot;
    [SerializeField] private GUIInventorySlot _upgradableSlotPreview, _resourceSlotPreview;
    #endregion

    #region LogicVariables
    //Amount variables are the ones indicatinh how much energi is in the storage (bottom left side of the screen)
    public float KingEnergyAmount { get; set; }    
    public float AssasinEnergyAmount { get; set; }    
    public float JudgeEnergyAmount { get; set; }
    public float SageEnergyAmount { get; set; }
    //Overload variables are for indicationg how much cbarge is currently in the buttons (only during upgradiong minigame)
    public float KingEnergyOverload { get; set; }
    public float AssasinEnergyOverload { get; set; }
    public float JudgeEnergyOverload { get; set; }
    public float SageEnergyOverload { get; set; }
    //Gain variables indicate how much energyt has already been accumulated during current minigame. They indicate the type of output bonus or strength of Ether Fiber
    public float KingEnergyGain { get; set; }
    public float AssasinEnergyGain { get; set; }
    public float JudgeEnergyGain { get; set; }
    public float SageEnergyGain { get; set; }
    public bool UpgradingOngoing { get; set; }
    //while the player picks what fiber they want to redo
    public bool FiberPickMode { get; set; }
    public float CurrentUpgradingTime { get; set; }
    public bool LeverLocked { get; set; }
    //variables used to properly retue items to their previous slots in case player interrupting the process
    private GUIInventorySlot _transformItemPreviousSlot, _upgradedItemPreviousSlot;
    private short _currentlySelectedFiberNumber;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        _animationHandler = GetComponent<SFEtherTransformerAnimationBehaviour>();
        if (_animationHandler == null)
            Debug.LogError("Ether Transformer Animation Behaviour not found! Expect errors!");
        _sounds = GetComponent<SoundBundleBehaviour>();
        _menu.gameObject.SetActive(false);
        _upButton.onClick.AddListener(PressKingButton);
        _leftButton.onClick.AddListener(PressSageButton);
        _bottomButton.onClick.AddListener(PressJudgeButton);
        _rightButton.onClick.AddListener(PressAssassinButton);
        _leverButton.onClick.AddListener(PressLeaver);
        _transformButton.onClick.AddListener(PressTransformButton);
        _upgradableSlot = new(); 
        _resourceSlot = new();
        _upgradableSlotPreview.InventorySlot = _upgradableSlot;
        _resourceSlotPreview.InventorySlot = _resourceSlot;
        _upgradableSlotPreview.UpdateSlot();
        _resourceSlotPreview.UpdateSlot();
        AddSlot(_upgradableSlotPreview);
        AddSlot(_resourceSlotPreview);
        _upgradableScrollRect = _upgradableView.GetComponentInChildren<ScrollRect>();
        _resourceScrollRect = _resourceView.GetComponentInChildren<ScrollRect>();
        _currentSlots.Add(_upgradableSlotButton.InventorySlotWidget);
        _upgradableSlotButton.OnPointerEnterEvent += SlotPointerEnterAction;
        _upgradableSlotButton.OnPointerExitEvent += SlotPointerExitAction;
        _upgradableSlotButton.OnPointerUpEvent += SlotPointerUpAction;
        _upgradableSlotButton.OnPointerDownEvent += SlotPointerDownAction;
        _currentSlots.Add(_resourceSlotButton.InventorySlotWidget);
        _resourceSlotButton.OnPointerEnterEvent += SlotPointerEnterAction;
        _resourceSlotButton.OnPointerExitEvent += SlotPointerExitAction;
        _resourceSlotButton.OnPointerUpEvent += SlotPointerUpAction;
        _resourceSlotButton.OnPointerDownEvent += SlotPointerDownAction;
        UpgradingOngoing = false;
        _inputBehaviour.OnUpAction += UpButton_Pressed;
        _inputBehaviour.OnDownAction += DownButton_Pressed;
        _inputBehaviour.OnLeftAction += LeftButton_Pressed;
        _inputBehaviour.OnRightAction += RightButton_Pressed;
        LeverLocked = false;
        _upgradableView.SetTitle("Equipment");
        _resourceView.SetTitle("Resources");
    }
    protected override void Update()
    {
        base.Update();
        UpdateUpgrading();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateUpgrading();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _inputBehaviour.OnUpAction -= UpButton_Pressed;
        _inputBehaviour.OnDownAction -= DownButton_Pressed;
        _inputBehaviour.OnLeftAction -= LeftButton_Pressed;
        _inputBehaviour.OnRightAction -= RightButton_Pressed;
    }
    #endregion

    #region Methods
    private void UpdateUpgrading()
    {
        if (UpgradingOngoing)
        {
            CurrentUpgradingTime += Time.fixedDeltaTime;
            KingEnergyOverload += 0.1f + UnityEngine.Random.Range(0f, 0.2f);
            AssasinEnergyOverload += 0.1f + UnityEngine.Random.Range(0f, 0.2f);
            JudgeEnergyOverload += 0.1f + UnityEngine.Random.Range(0f, 0.2f);
            SageEnergyOverload += 0.1f + UnityEngine.Random.Range(0f, 0.2f);

            var kingIncrease = 0.001f * KingEnergyOverload;
            var assasinIncrease = 0.001f * AssasinEnergyOverload;
            var judgeIncrease = 0.001f * JudgeEnergyOverload;
            var sageIncrease = 0.001f * SageEnergyOverload;

            KingEnergyGain += kingIncrease * 2;
            AssasinEnergyGain += assasinIncrease * 2;
            JudgeEnergyGain += judgeIncrease * 2;
            SageEnergyGain += sageIncrease * 2;
            KingEnergyAmount -= kingIncrease;
            AssasinEnergyAmount -= assasinIncrease;
            JudgeEnergyAmount -= judgeIncrease;
            SageEnergyAmount -= sageIncrease;

            if (CurrentUpgradingTime >= MAX_UPGRADING_TIME || KingEnergyOverload < 0 || KingEnergyOverload > 100 || AssasinEnergyOverload < 0 || AssasinEnergyOverload > 100 ||
                JudgeEnergyOverload < 0 || JudgeEnergyOverload > 100 || SageEnergyOverload < 0 || SageEnergyOverload > 100 || KingEnergyAmount < 0 || AssasinEnergyAmount < 0
                || JudgeEnergyAmount < 0 || SageEnergyAmount < 0)
            {
                FinishUpgrading();
            }
        }
    }
    private void StartUpgrading()
    {
        _animationHandler.SetLockLight(true);
        _sounds.Playlong(this.gameObject, "EtherTransformer_Working");
        CurrentUpgradingTime = 0;
        UpgradingOngoing = true;
        KingEnergyOverload = 0;
        AssasinEnergyOverload = 0;
        JudgeEnergyOverload = 0;
        SageEnergyOverload = 0;
        KingEnergyGain = 0;
        AssasinEnergyGain = 0;
        JudgeEnergyGain = 0;
        SageEnergyGain = 0;
        LeverLocked = false;
    }
    private void PreUpgraeCheck()
    {
        if (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[0] == 0)
        {
            _console.text = "Warning! Upgrade type will now be decided!";
            StartUpgrading();
        }
        else
        {
            short i = 0;
            bool full = false;
            bool max = true;
            while (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[i] != 0)
            {
                i++;
                if (i > 19)
                {
                    full = true;
                    break;
                }
                else if (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[i] != 1)
                    max = false;
            }
            if (max)
            {
                _console.text = "This item can no longer be improved!";
                FinishUpgrading();
            }
            else if (full)
            {
                _console.text = "All Ether Fibers are already created! Use up(king) and down(judge) to pick the Fiber to re-create. Confirm by pressing Transform button.";
                FiberPickMode = true;
                _currentlySelectedFiberNumber = -1;
                PickNextFiber();
                LeverLocked = false;
            }
            else
            {
                _currentlySelectedFiberNumber = i;
                _console.text = "Upgrade is starting! Get ready!";
                StartUpgrading();
            }
        }
    }
    private void FinishUpgrading()
    {
        _animationHandler.SetLockLight(false);
        _animationHandler.OpenDoor();
        _sounds.PlaySound(this.gameObject, "EtherTransformer_DoorOpen");
        _sounds.Fadeout("EtherTransformer_Working");
        if (UpgradingOngoing)
            _animationHandler.SpawnSmoke();
        UpgradingOngoing = false;
        FiberPickMode = false;
        KingEnergyOverload = 0;
        AssasinEnergyOverload = 0;
        JudgeEnergyOverload = 0;
        SageEnergyOverload = 0;
        _upgradableSlotButton.gameObject.SetActive(true);
        _animationHandler.SetMachineLight(false);
        _animationHandler.ResetButtonFillDisplay();
        CheckResult();
        LeverLocked = false;
    }
    private void CheckResult()
    {
        float king = KingEnergyGain * 1.25f - AssasinEnergyGain - JudgeEnergyGain - SageEnergyGain;
        float kingHalf = KingEnergyGain * 0.75f + JudgeEnergyGain * 0.5f - AssasinEnergyGain - SageEnergyGain;
        float assasin = AssasinEnergyGain * 1.25f - KingEnergyGain - JudgeEnergyGain - SageEnergyGain;
        float assasinHalf = AssasinEnergyGain * 0.75f + SageEnergyGain * 0.5f - KingEnergyGain - JudgeEnergyGain;
        float judge = JudgeEnergyGain * 1.25f - AssasinEnergyGain - KingEnergyGain - SageEnergyGain;
        float judgeHalf = JudgeEnergyGain * 0.75f + KingEnergyGain * 0.5f - AssasinEnergyGain - SageEnergyGain;
        float sage = SageEnergyGain * 1.25f - AssasinEnergyGain - JudgeEnergyGain - KingEnergyGain;
        float sageHalf = SageEnergyGain * 0.75f + AssasinEnergyGain * 0.5f - JudgeEnergyGain - KingEnergyGain;
        List<float> numbers = new() { king, kingHalf, assasin, assasinHalf, judge, judgeHalf, sage, sageHalf };
        numbers.Sort();
        if (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[0] == 0)
        {
            _upgradableSlot.Item.ItemUpgradeState.EtherFibers[0] = 1;
            if (numbers[numbers.Count - 1] == king)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.King;
            }
            if (numbers[numbers.Count - 1] == kingHalf)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.KingHalf;
            }
            if (numbers[numbers.Count - 1] == assasin)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.Assassin;
            }
            if (numbers[numbers.Count - 1] == assasinHalf)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.AssassinHalf;
            }
            if (numbers[numbers.Count - 1] == judge)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.Judge;
            }
            if (numbers[numbers.Count - 1] == judgeHalf)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.JudgeHalf;
            }
            if (numbers[numbers.Count - 1] == sage)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.Sage;
            }
            if (numbers[numbers.Count - 1] == sageHalf)
            {
                _upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.SageHalf;
            }
            _console.text = "Upgrade type has been decided!";
        }
        else
        {
            float targetValue = 0;
            if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.King)
                targetValue = king;
            else if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.KingHalf)
                targetValue = kingHalf;
            else if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Assassin)
                targetValue = assasin;
            else if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.AssassinHalf)
                targetValue = assasinHalf;
            else if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Judge)
                targetValue = judge;
            else if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.JudgeHalf)
                targetValue = judgeHalf;
            else if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Sage)
                targetValue = sage;
            else if (_upgradableSlot.Item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.SageHalf)
                targetValue = sageHalf;
            {
                if (targetValue > 50)
                {
                    if (targetValue > 100)
                        targetValue = 100;
                    _console.text = "Upgrade successful! Percent of current Ether Fiber completion:\n" + (targetValue/100).ToString("0.00%");
                    _upgradableSlot.Item.ItemUpgradeState.EtherFibers[_currentlySelectedFiberNumber] = targetValue / 100;
                }
                else
                    _console.text = "Upgrade failed!";
            }
        }
    }
    public bool CheckIfReadyToStart()
    {
        if(UpgradingOngoing || FiberPickMode)
        {
            return false;
        }
        if(KingEnergyAmount<=0 || AssasinEnergyAmount<=0 || JudgeEnergyAmount<=0 || SageEnergyAmount<=0)
        {
            _console.text = "There must be some energy in each of the tanks!";
            return false;
        }
        else if(_upgradableSlotButton.InventorySlotWidget.InventorySlot.Item == null || _upgradableSlotButton.InventorySlotWidget.InventorySlot.Item?.ItemSO.IsUpgradable == false)
        {
            _console.text = "Place a correct item inside the machine!";
            return false;
        }
        _console.text = "The process is starting...";
        return true;
    }
    public string GetInteractionTitle()
    {
        return "Activate Ether Transformer";
    }
    public IPlayerInteractable.InteractionType GetInteractionType()
    {
        return IPlayerInteractable.InteractionType.DigitalInterface;
    }
    public void Interact(PlayerBehaviour player)
    {
        _ = ScheduleInterfaceTransition();
        PrepareInterfaceOnInteraction();
    }
    public async Task ScheduleInterfaceTransition()
    {
        if (!alreadyFading)
        {
            alreadyFading = true;
            await _black.StartFadeIn();
            _gameplayControls.SetSpecialWorldInterface(_menu, _menuParent, _interfacecamera, this);
            await _black.StartFadeOut();
            alreadyFading = false;
        }
    }
    public void CloseInterface()
    {
        if(_resourceSlotButton.InventorySlotWidget.InventorySlot.Item!=null)
        {
            _transformItemPreviousSlot.InventorySlot.Item = _resourceSlotButton.InventorySlotWidget.InventorySlot.Item;
            _resourceSlotButton.InventorySlotWidget.InventorySlot.Item = null;
        }
        if (_upgradableSlotButton.InventorySlotWidget.InventorySlot.Item != null)
        {
            _upgradedItemPreviousSlot.InventorySlot.Item = _upgradableSlotButton.InventorySlotWidget.InventorySlot.Item;
            _upgradableSlotButton.InventorySlotWidget.InventorySlot.Item = null;
        }
        _upgradableSlotButton.InventorySlotWidget.UpdateSlot();
        _resourceSlotButton.InventorySlotWidget.UpdateSlot();
        if(UpgradingOngoing || FiberPickMode)
        {
            FinishUpgrading();
        }
        SkyforgeLoader.EquipmentChanged = true;
    }
    private void PrepareInterfaceOnInteraction()
    {
        _animationHandler.PrepareInterfaceOnInteraction();
        _registeredUpgradableSlots = new();
        if (SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item != null)
            _registeredUpgradableSlots.Add(SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot);
        if (SkyforgeLoader.CurrentProfile.Equipment.ArmorSlot.Item != null)
            _registeredUpgradableSlots.Add(SkyforgeLoader.CurrentProfile.Equipment.ArmorSlot);
        if (SkyforgeLoader.CurrentProfile.Equipment.ArtifactSlot.Item != null)
            _registeredUpgradableSlots.Add(SkyforgeLoader.CurrentProfile.Equipment.ArtifactSlot);
        foreach (var slot in SkyforgeLoader.CurrentProfile.Inventory.Slots.Where(s=>s.Item?.ItemSO.IsUpgradable == true))
        {
            _registeredUpgradableSlots.Add(slot);
        }
        _registeredResourceSlots = new();
        foreach (var slot in SkyforgeLoader.CurrentProfile.Inventory.Slots.Where(s => s.Item?.ItemSO.ID == "Base_Resource_MarkOfKing" 
            || s.Item?.ItemSO.ID == "Base_Resource_MarkOfAssasin"
            || s.Item?.ItemSO.ID == "Base_Resource_MarkOfJudge" 
            || s.Item?.ItemSO.ID == "Base_Resource_MarkOfSage"))
        {
            _registeredResourceSlots.Add(slot);
        }
        var toRemoveList = _currentSlots.ToList();
        foreach (var slot in toRemoveList)
        {
            RemoveSlot(slot);
        }
        _upgradableView.AssignSlots(_registeredUpgradableSlots);
        _resourceView.AssignSlots(_registeredResourceSlots);
        foreach (var slot in _upgradableView.GetSlots())
        {
            AddSlot(slot);
        }
        foreach (var slot in _resourceView.GetSlots())
        {
            AddSlot(slot);
        }
        _console.text = "Drag resources into the slot\nover the Transform button";
    }
    private void PickNextFiber()
    {
        var initValue = _currentlySelectedFiberNumber;
        if (_currentlySelectedFiberNumber < 19)
            _currentlySelectedFiberNumber++;
        while (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[_currentlySelectedFiberNumber]==1)
        {
            _currentlySelectedFiberNumber++;
            if(_currentlySelectedFiberNumber>19)
            {
                _currentlySelectedFiberNumber = initValue;
                break;
            }
        }
    }
    private void PickPreviousFiber()
    {
        var initValue = _currentlySelectedFiberNumber;
        if (_currentlySelectedFiberNumber > 0)
            _currentlySelectedFiberNumber--;
        while (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[_currentlySelectedFiberNumber] == 1)
        {
            _currentlySelectedFiberNumber--;
            if (_currentlySelectedFiberNumber < 0)
            {
                _currentlySelectedFiberNumber = initValue;
                break;
            }
        }
    }
    #endregion

    #region EventHandlers
    private void PressTransformButton()
    {
        _animationHandler.PressTransformButton();
        if(FiberPickMode)
        {
            FiberPickMode = false;
            StartUpgrading();
        }
        else
        {
            var item = _resourceSlotButton.InventorySlotWidget.InventorySlot.Item;
            if (item != null)
            {
                if (item.ItemSO.ID == "Base_Resource_MarkOfKing")
                {
                    if (KingEnergyAmount < MAX_ENERGY_AMOUNT)
                    {
                        item.Amount -= 1;
                        if (item.Amount <= 0)
                            _resourceSlotButton.InventorySlotWidget.InventorySlot.Item = null;
                        KingEnergyAmount += 10;
                        if (KingEnergyAmount > MAX_ENERGY_AMOUNT)
                            KingEnergyAmount = MAX_ENERGY_AMOUNT;
                        _console.text = "Resources transformed successfully!";
                    }
                    else
                        _console.text = "King energy already full!";
                }
                else if (item.ItemSO.ID == "Base_Resource_MarkOfAssasin")
                {
                    if (AssasinEnergyAmount < MAX_ENERGY_AMOUNT)
                    {
                        item.Amount -= 1;
                        if (item.Amount <= 0)
                            _resourceSlotButton.InventorySlotWidget.InventorySlot.Item = null;
                        AssasinEnergyAmount += 10;
                        if (AssasinEnergyAmount > MAX_ENERGY_AMOUNT)
                            AssasinEnergyAmount = MAX_ENERGY_AMOUNT;
                        _console.text = "Resources transformed successfully!";
                    }
                    else
                        _console.text = "Assasin energy already full!";
                }
                else if (item.ItemSO.ID == "Base_Resource_MarkOfJudge")
                {
                    if (JudgeEnergyAmount < MAX_ENERGY_AMOUNT)
                    {
                        item.Amount -= 1;
                        if (item.Amount <= 0)
                            _resourceSlotButton.InventorySlotWidget.InventorySlot.Item = null;
                        JudgeEnergyAmount += 10;
                        if (JudgeEnergyAmount > MAX_ENERGY_AMOUNT)
                            JudgeEnergyAmount = MAX_ENERGY_AMOUNT;
                        _console.text = "Resources transformed successfully!";
                    }
                    else
                        _console.text = "Judge energy already full!";
                }
                else if (item.ItemSO.ID == "Base_Resource_MarkOfSage")
                {
                    if (SageEnergyAmount < MAX_ENERGY_AMOUNT)
                    {
                        item.Amount -= 1;
                        if (item.Amount <= 0)
                            _resourceSlotButton.InventorySlotWidget.InventorySlot.Item = null;
                        SageEnergyAmount += 10;
                        if (SageEnergyAmount > MAX_ENERGY_AMOUNT)
                            SageEnergyAmount = MAX_ENERGY_AMOUNT;
                        _console.text = "Resources transformed successfully!";
                    }
                    else
                        _console.text = "Sage energy already full!";
                }
                else
                    _console.text = "Warning!\nWrong item in the resource slot!";
            }
            else
                _console.text = "Warning!\nResource slot is empty!";
            _resourceSlotButton.InventorySlotWidget.UpdateSlot();
        }        
    }
    private void PressLeaver()
    {
        if(!LeverLocked)
        {
            _animationHandler.PressLeaver();
            _sounds.PlaySound(this.gameObject, "EtherTransformer_Lever");
            if (UpgradingOngoing || FiberPickMode)
            {
                FinishUpgrading();
            }
            else if( CheckIfReadyToStart())
            {
                _animationHandler.CloseDoor();
                _sounds.PlaySound(this.gameObject, "EtherTransformer_DoorClose");
                _animationHandler.SetMachineLight(true);
                _upgradableSlotButton.gameObject.SetActive(false);
                StartCoroutine(ScheduleUpgradingStart(2f));
                LeverLocked = true;
            }
            else
            {
                //StartCoroutine(_animationHandler.CancelLeverRotation());
            }
        }
    }
    public IEnumerator ScheduleUpgradingStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        PreUpgraeCheck();
    }
    private void PressKingButton()
    {
        _animationHandler.PressKingButton();
        if (UpgradingOngoing)
        {
            KingEnergyOverload -= 5f;
            if(KingEnergyOverload<0)
            {
                KingEnergyOverload = 0;
            }
        }
        else if(FiberPickMode)
        {
            PickPreviousFiber();
            _console.text = "Fiber number " + (_currentlySelectedFiberNumber+1) + "  [ " + (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[_currentlySelectedFiberNumber]).ToString("0.00%") + "]";
        }
    }
    private void PressAssassinButton()
    {
        _animationHandler.PressAssassinButton();
        if (UpgradingOngoing)
        {
            AssasinEnergyOverload -= 5f;
            if (AssasinEnergyOverload < 0)
            {
                AssasinEnergyOverload = 0;
            }
        }
    }
    private void PressJudgeButton()
    {
        _animationHandler.PressJudgeButton();
        JudgeEnergyOverload -= 5f;
        if (UpgradingOngoing)
        {
            if (JudgeEnergyOverload < 0)
            {
                JudgeEnergyOverload = 0;
            }
        }
        else if (FiberPickMode)
        {
            PickNextFiber();
            _console.text = "Fiber number " + (_currentlySelectedFiberNumber + 1) + "  [ " + (_upgradableSlot.Item.ItemUpgradeState.EtherFibers[_currentlySelectedFiberNumber]).ToString("0.00%") + "]";
        }
    }
    private void PressSageButton()
    {
        _animationHandler.PressSageButton();
        if (UpgradingOngoing)
        {
            SageEnergyOverload -= 5f;
            if (SageEnergyOverload < 0)
            {
                SageEnergyOverload = 0;
            }
        }
    }
    private void UpButton_Pressed(object sender, EventArgs e)
    {
        PressKingButton();
    }
    private void DownButton_Pressed(object sender, EventArgs e)
    {
        PressJudgeButton();
    }
    private void LeftButton_Pressed(object sender, EventArgs e)
    {
        PressSageButton();
    }
    private void RightButton_Pressed(object sender, EventArgs e)
    {
        PressAssassinButton();
    }
    protected override void SlotPointerDownAction(object sender, EventArgs e)
    {
        _upgradableScrollRect.enabled = false;
        _resourceScrollRect.enabled = false;
        base.SlotPointerDownAction(sender, e);
    }
    protected override void SlotPointerUpAction(object sender, EventArgs e)
    {
        if (_currentlyDraggedSlot != null && _currentDragTargetSlot != null)
        {
            if (_currentlyDraggedSlot != _currentDragTargetSlot)
            {
                bool canMove = false;
                //Check if dragging from the upgrade slot back into the inventory
                if (_currentlyDraggedSlot == _upgradableSlotButton.InventorySlotWidget && _currentDragTargetSlot == _upgradedItemPreviousSlot)
                {
                    canMove = true;
                    _upgradedItemPreviousSlot = null;
                }
                //Check if dragging from the transform slot back into the inventory
                else if (_currentlyDraggedSlot == _resourceSlotButton.InventorySlotWidget && _currentDragTargetSlot == _transformItemPreviousSlot)
                {
                    canMove = true;
                    _transformItemPreviousSlot = null;
                }
                //if not, check if the destination slot is one of the machine's slots
                else
                {
                    if (_currentDragTargetSlot == _resourceSlotButton.InventorySlotWidget)
                    {
                        //if something is already in the slot, it has to first return to its original slot
                        if (_resourceSlotButton.InventorySlotWidget.InventorySlot.Item != null)
                        {
                            var tempItem = _resourceSlotButton.InventorySlotWidget.InventorySlot.Item;
                            _resourceSlotButton.InventorySlotWidget.InventorySlot.Item = _transformItemPreviousSlot.InventorySlot.Item;
                            _transformItemPreviousSlot.InventorySlot.Item = tempItem;
                            _transformItemPreviousSlot.UpdateSlot();
                        }
                        _transformItemPreviousSlot = _currentlyDraggedSlot;
                        if (_currentlyDraggedSlot != _upgradableSlotButton.InventorySlotWidget)
                            _transformItemPreviousSlot = _currentlyDraggedSlot;
                        canMove = true;
                    }
                    else if (_currentDragTargetSlot == _upgradableSlotButton.InventorySlotWidget)
                    {
                        //if something is already in the slot, it has to first return to its original slot
                        if (_upgradableSlotButton.InventorySlotWidget.InventorySlot.Item != null)
                        {
                            var tempItem = _upgradableSlotButton.InventorySlotWidget.InventorySlot.Item;
                            _upgradableSlotButton.InventorySlotWidget.InventorySlot.Item = _upgradedItemPreviousSlot.InventorySlot.Item;
                            _upgradedItemPreviousSlot.InventorySlot.Item = tempItem;
                            _upgradedItemPreviousSlot.UpdateSlot();
                        }
                        if(_currentlyDraggedSlot != _resourceSlotButton.InventorySlotWidget)
                            _upgradedItemPreviousSlot = _currentlyDraggedSlot;
                        canMove = true;
                    }
                }
                if (canMove)
                {
                    var tempItem = _currentDragTargetSlot.InventorySlot.Item;
                    _currentDragTargetSlot.InventorySlot.Item = _currentlyDraggedSlot.InventorySlot.Item;
                    _currentlyDraggedSlot.InventorySlot.Item = tempItem;
                    _currentDragTargetSlot.UpdateSlot();
                    _currentlyDraggedSlot.UpdateSlot();
                    _currentDragTargetSlot = null;
                    SoundManager.UIInstance.PlayGlobalSFX(_itemMoveSound);
                }
            }
        }
        _currentlyDraggedSlot = null;
        _visualItemGhost.gameObject.SetActive(false);
        _upgradableScrollRect.enabled = true;
        _resourceScrollRect.enabled = true;
        base.SlotPointerUpAction(sender, e);
    }
    #endregion
}
