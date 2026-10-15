using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SFEtherTransformerAnimationBehaviour : MonoBehaviour
{
    private const float LEVER_WAIT_TIME = 0.5f;
    private const float LEVER_DRAG_TIME = 0.15f;
    private const float DOOR_OPENING_STAGE1 = 0.2f;
    private const float DOOR_OPENING_STAGE2 = 0.4f;
    private const float DOOR_OPENING_STAGE3 = 1f;

    #region Variables
    private SFEtherTransformerBehaviour _transformer;
    [Header("Middle Keyboard")]
    [SerializeField] private Image _kingPointer;
    [SerializeField] private Image _assassinPointer, _judgePointer, _sagePointer;
    [SerializeField] private SkinnedMeshRenderer _kingButton, _assassinButton, _judgeButton, _sageButton;
    [Header("Transform Button And Lever")]
    [SerializeField] private SkinnedMeshRenderer _transformButtonMesh;
    [SerializeField] private MeshRenderer _leverMesh;
    [SerializeField] private float _leverRotationRest, _leverRotationPressed;
    private float _leverWaitTimeLeft, _leverDragTimeLeft;
    private bool _isLeverDown;
    [Header("Door and Stand")]
    [SerializeField] private SkinnedMeshRenderer _doorMesh;
    [SerializeField] private SkinnedMeshRenderer _doorLockMesh1, _doorLockMesh2;
    [SerializeField] private MeshRenderer _standMesh;
    private enum DoorState { Closed, Opening, Open, Closing }
    private DoorState _currentDoorState;
    private enum DoorAnimatinoState { None, BlendOpen, RotateLocks, BlendLock }
    private DoorAnimatinoState _scurrentDoorAnimationStage = DoorAnimatinoState.None;
    private float _doorTimer;
    private bool _isOpening;
    [SerializeField] private Vector3 _doorLockRotationRestAngle, _doorLockRotationLockedAngle, _itemStandElevateMod;
    private Vector3 _itemStandInitialPosition, _upgradableSlotInitialPosition;
    [Header("LightMeshes")]
    [SerializeField] private MeshRenderer _machineMainMesh;
    [SerializeField] private MeshRenderer _transformSectionMesh;
    [Header("Items")]
    [SerializeField] private GUIInventorySlot _upgradableSlotPreview;
    [Header("Effects")]
    [SerializeField] private ParticleSystem _smokeBase;
    #endregion

    #region Mono
    private void Awake()
    {
        _transformer = GetComponent<SFEtherTransformerBehaviour>();
        if (_transformer == null)
            Debug.LogError("Ether Transformer Animation Behaviour missuse! Expect errors!");
        _currentDoorState = DoorState.Open;
        _scurrentDoorAnimationStage = DoorAnimatinoState.None;
        _itemStandInitialPosition = _standMesh.transform.position;
        _upgradableSlotInitialPosition = _upgradableSlotPreview.transform.position;
    }
    private void Update()
    {
        _kingPointer.fillAmount = _transformer.KingEnergyAmount / 100;
        _assassinPointer.fillAmount = _transformer.AssasinEnergyAmount / 100;
        _judgePointer.fillAmount = _transformer.JudgeEnergyAmount / 100;
        _sagePointer.fillAmount = _transformer.SageEnergyAmount / 100;
        if(_transformer.UpgradingOngoing)
        {
            _kingButton.SetBlendShapeWeight(1, 100 - (_transformer.KingEnergyOverload));
            _assassinButton.SetBlendShapeWeight(1, 100 - (_transformer.AssasinEnergyOverload));
            _judgeButton.SetBlendShapeWeight(1, 100 - (_transformer.JudgeEnergyOverload));
            _sageButton.SetBlendShapeWeight(1, 100 - (_transformer.SageEnergyOverload));
        }
        if(_transformer.KingEnergyAmount > 0 && _transformer.AssasinEnergyAmount > 0 && _transformer.JudgeEnergyAmount > 0 && _transformer.SageEnergyAmount > 0)
            SetTransformLight(true);
        else
            SetTransformLight(false);
        UpdateLeverRotation();
        UpdateDoorAnimation();
    }
    #endregion

    #region Methods
    private void UpdateLeverRotation()
    {
        if(_leverWaitTimeLeft<=0 && _leverDragTimeLeft<=0)
        {
            if(_transformer.LeverLocked && _isLeverDown == false)
                _leverDragTimeLeft = LEVER_DRAG_TIME;
            else if(!_transformer.LeverLocked && _isLeverDown == true)
                _leverWaitTimeLeft = LEVER_WAIT_TIME;
        }
        else if (_leverWaitTimeLeft > 0)
        {
            _leverWaitTimeLeft -= Time.deltaTime;
            var percent = _leverWaitTimeLeft / LEVER_WAIT_TIME;
            var maxAmount = _leverRotationPressed - _leverRotationRest;
            _leverMesh.transform.rotation = Quaternion.Euler(new Vector3(_leverRotationRest + (maxAmount * percent), 0, 0));
            if (_leverWaitTimeLeft <= 0)
            {
                _leverWaitTimeLeft = 0;
                _leverMesh.transform.rotation = Quaternion.Euler(new Vector3(_leverRotationRest, 0, 0));
                _isLeverDown = false;
            }
        }
        else if (_leverDragTimeLeft > 0)
        {
            _leverDragTimeLeft -= Time.deltaTime;
            var percent = 1 - (_leverDragTimeLeft / LEVER_DRAG_TIME);
            var maxAmount = _leverRotationPressed - _leverRotationRest;
            _leverMesh.transform.rotation = Quaternion.Euler(new Vector3(_leverRotationRest + (maxAmount * percent), 0, 0));
            if (_leverDragTimeLeft <= 0)
            {
                _leverDragTimeLeft = 0;
                _leverMesh.transform.rotation = Quaternion.Euler(new Vector3(_leverRotationPressed, 0, 0));
                _isLeverDown = true;
            }
        }
    }
    public IEnumerator CancelLeverRotation()
    {
        yield return new WaitForSeconds(LEVER_DRAG_TIME + 0.1f);
        _leverDragTimeLeft = 0;
        _leverWaitTimeLeft = LEVER_WAIT_TIME;
    }
    private void UpdateDoorAnimation()
    {
        if (_currentDoorState != DoorState.Opening && _currentDoorState != DoorState.Closing)
            return;
        _doorTimer += Time.deltaTime;
        if (_isOpening)
        {
            switch (_scurrentDoorAnimationStage)
            {
                case DoorAnimatinoState.BlendLock:
                    {
                        float percent = Mathf.Clamp01(_doorTimer / DOOR_OPENING_STAGE3);
                        _doorMesh.SetBlendShapeWeight(0, 100 - (percent * 100f));
                        _doorLockMesh1.SetBlendShapeWeight(0, 100 - (percent * 100f));
                        _doorLockMesh2.SetBlendShapeWeight(0, 100 - (percent * 100f));
                        _standMesh.transform.position = Vector3.Lerp(_itemStandInitialPosition, _itemStandInitialPosition + _itemStandElevateMod, 1 - percent);
                        _upgradableSlotPreview.transform.position = Vector3.Lerp(_upgradableSlotInitialPosition, _upgradableSlotInitialPosition + _itemStandElevateMod, 1 - percent);
                        if (_doorTimer >= DOOR_OPENING_STAGE3)
                        {
                            _currentDoorState = DoorState.Open;
                            _scurrentDoorAnimationStage = DoorAnimatinoState.None;
                        }
                        break;
                    }
                case DoorAnimatinoState.RotateLocks:
                    {
                        float percent = Mathf.Clamp01(_doorTimer / DOOR_OPENING_STAGE2);
                        Vector3 euler = Vector3.Lerp(_doorLockRotationLockedAngle, _doorLockRotationRestAngle, percent);
                        _doorLockMesh1.transform.localEulerAngles = euler;
                        _doorLockMesh2.transform.localEulerAngles = euler;
                        if (_doorTimer >= DOOR_OPENING_STAGE2)
                        {
                            _scurrentDoorAnimationStage = DoorAnimatinoState.BlendLock;
                            _doorTimer = 0f;
                        }
                        break;
                    }
                case DoorAnimatinoState.BlendOpen:
                    {
                        float percent = Mathf.Clamp01(_doorTimer / DOOR_OPENING_STAGE1);
                        _doorLockMesh2.SetBlendShapeWeight(1, percent * 100f);
                        if (_doorTimer >= DOOR_OPENING_STAGE1)
                        {
                            _scurrentDoorAnimationStage = DoorAnimatinoState.RotateLocks;
                            _doorTimer = 0f;
                        }
                        break;
                    }
            }
        }
        else
        {
            switch (_scurrentDoorAnimationStage)
            {
                case DoorAnimatinoState.BlendLock:
                    {
                        float percent = Mathf.Clamp01(_doorTimer / DOOR_OPENING_STAGE3);
                        _doorMesh.SetBlendShapeWeight(0, percent * 100f);
                        _doorLockMesh1.SetBlendShapeWeight(0, percent * 100f);
                        _doorLockMesh2.SetBlendShapeWeight(0, percent * 100f);
                        _standMesh.transform.position = Vector3.Lerp(_itemStandInitialPosition, _itemStandInitialPosition + _itemStandElevateMod, percent);
                        _upgradableSlotPreview.transform.position = Vector3.Lerp(_upgradableSlotInitialPosition, _upgradableSlotInitialPosition + _itemStandElevateMod, percent);
                        if (_doorTimer >= DOOR_OPENING_STAGE3)
                        {
                            _scurrentDoorAnimationStage = DoorAnimatinoState.RotateLocks;
                            _doorTimer = 0f;
                        }
                        break;
                    }
                case DoorAnimatinoState.RotateLocks:
                    {
                        float percent = Mathf.Clamp01(_doorTimer / DOOR_OPENING_STAGE2);
                        Vector3 euler = Vector3.Lerp(_doorLockRotationRestAngle, _doorLockRotationLockedAngle, percent);
                        _doorLockMesh1.transform.localEulerAngles = euler;
                        _doorLockMesh2.transform.localEulerAngles = euler;
                        if (_doorTimer >= DOOR_OPENING_STAGE2)
                        {
                            _scurrentDoorAnimationStage = DoorAnimatinoState.BlendOpen;
                            _doorTimer = 0f;
                        }
                        break;
                    }
                case DoorAnimatinoState.BlendOpen:
                    {
                        float percent = Mathf.Clamp01(_doorTimer / DOOR_OPENING_STAGE1);
                        _doorLockMesh2.SetBlendShapeWeight(1, 1 - (percent * 100f));
                        if (_doorTimer >= DOOR_OPENING_STAGE1)
                        {
                            _currentDoorState = DoorState.Closed;
                            _scurrentDoorAnimationStage = DoorAnimatinoState.None;
                        }
                        break;
                    }
            }
        }
    }
    public IEnumerator CancelPress(SkinnedMeshRenderer button, float timeAfter = 0.1f)
    {
        yield return new WaitForSeconds(timeAfter);
        button.SetBlendShapeWeight(0, 0);
    }
    public void OpenDoor()
    {
        if (_currentDoorState == DoorState.Open || _currentDoorState == DoorState.Opening)
            return;
        _isOpening = true;
        _currentDoorState = DoorState.Opening;
        _scurrentDoorAnimationStage = DoorAnimatinoState.BlendOpen;
        _doorTimer = 0f;
    }
    public void CloseDoor()
    {
        if (_currentDoorState == DoorState.Closed || _currentDoorState == DoorState.Closing)
            return;
        _isOpening = false;
        _currentDoorState = DoorState.Closing;
        _scurrentDoorAnimationStage = DoorAnimatinoState.BlendLock;
        _doorTimer = 0f;
    }
    public void PrepareInterfaceOnInteraction()
    {
        _kingButton.SetBlendShapeWeight(1, 100);
        _assassinButton.SetBlendShapeWeight(1, 100);
        _judgeButton.SetBlendShapeWeight(1, 100);
        _sageButton.SetBlendShapeWeight(1, 100);
    }
    public void SetMachineLight(bool lightOn)
    {
        int FadeID = Shader.PropertyToID("_Emission_Intensity");
        var mpb = new MaterialPropertyBlock();
        _machineMainMesh.GetPropertyBlock(mpb);
        if (lightOn)
            mpb.SetFloat(FadeID, 50000);
        else
            mpb.SetFloat(FadeID, 0);
        _machineMainMesh.SetPropertyBlock(mpb);
    }
    public void SetLockLight(bool lightOn)
    {
        int FadeID = Shader.PropertyToID("_Emission_Intensity");
        var mpb = new MaterialPropertyBlock();
        _doorLockMesh1.GetPropertyBlock(mpb);
        if (lightOn)
            mpb.SetFloat(FadeID, 50000);
        else
            mpb.SetFloat(FadeID, 0);
        _doorLockMesh1.SetPropertyBlock(mpb);
    }
    public void SetTransformLight(bool lightOn)
    {
        int FadeID = Shader.PropertyToID("_Emission_Intensity");
        var mpb = new MaterialPropertyBlock();
        _transformSectionMesh.GetPropertyBlock(mpb);
        if (lightOn)
            mpb.SetFloat(FadeID, 50000);
        else
            mpb.SetFloat(FadeID, 0);
        _transformSectionMesh.SetPropertyBlock(mpb);
    }
    public void ResetButtonFillDisplay()
    {
        _kingButton.SetBlendShapeWeight(1, 100);
        _assassinButton.SetBlendShapeWeight(1, 100);
        _judgeButton.SetBlendShapeWeight(1, 100);
        _sageButton.SetBlendShapeWeight(1, 100);
    }
    public void SpawnSmoke()
    {
        var smoke = Instantiate(_smokeBase, this.transform);
        smoke.gameObject.SetActive(true);
    }
    #endregion

    #region ButtonPressMethods
    public void PressKingButton()
    {
        _kingButton.SetBlendShapeWeight(0, 100);
        StartCoroutine(CancelPress(_kingButton));
    }
    public void PressAssassinButton()
    {
        _assassinButton.SetBlendShapeWeight(0, 100);
        StartCoroutine(CancelPress(_assassinButton));
    }
    public void PressJudgeButton()
    {
        _judgeButton.SetBlendShapeWeight(0, 100);
        StartCoroutine(CancelPress(_judgeButton));
    }
    public void PressSageButton()
    {
        _sageButton.SetBlendShapeWeight(0, 100);
        StartCoroutine(CancelPress(_sageButton));
    }
    public void PressTransformButton()
    {
        _transformButtonMesh.SetBlendShapeWeight(0, 100);
        StartCoroutine(CancelPress(_transformButtonMesh, 0.3f));
    }
    public void PressLeaver()
    {
        _leverDragTimeLeft = LEVER_DRAG_TIME;
    }
    #endregion
}
