using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AscensionAtlasBehaviour : MonoBehaviour
{
    #region Variables
    public HeroClassSO HeroClassSO;
    [Header("Sprites")]
    [SerializeField] private Sprite _aelionEidosSprite;
    public Sprite IconBoxSprite, IconCrossSprite, IconBallSprite;
    public Sprite IconBoxSpriteSelected, IconCrossSpriteSelected, IconBallSpriteSelected;
    public AtlasNodeSelection NodeSelectionSprite;
    private List<AtlasNodeBehaviour> _currentlyDisplayedNodes;
    private AtlasNodeBehaviour _previouslyHoveredNode;
    [Header("Scene")]
    [SerializeField] private Camera _atlasCamera;
    [SerializeField] private Transform _tooltipLayer, _connectionsLayer;
    [SerializeField] private Canvas _tooltipCanvas;
    private float _untilTooltipDisplaytimer;
    [Tooltip("The atlas camera can move only inside this collider. Make sure the collider is not on the Default layer, as it may block mouse collision with nodes")]
    public Collider AtlasBounds;
    [Tooltip("The point at which the canvas camera will reset. Best is to put it near the start noe of the atlas")]
    public Transform OriginPoint;
    [Header("Base Prefabs")]
    [Tooltip("Prefab used to spawn GUI tooltips on node hover")]
    [SerializeField] private GUITooltip _tooltipBase;
    [Tooltip("Prefab used to spawn new node connections")]
    [SerializeField] private AtlasNodeConnectionBehaviour _connectionBase;
    private GUITooltip _currentTooltip;
    private List<AtlasNodeConnectionBehaviour> _currentlyDisplayedConnections;
    #endregion

    #region Mono
    public void Awake()
    {
        NodeSelectionSprite.gameObject.SetActive(false);
        _currentlyDisplayedNodes = GetComponentsInChildren<AtlasNodeBehaviour>().ToList();
        _untilTooltipDisplaytimer = 0;
        _currentlyDisplayedConnections = new();
        SetUpConnections();
    }
    protected void Update()
    {
        UpdateMouseCollision();
        if(_untilTooltipDisplaytimer>0 && _previouslyHoveredNode != null)
        {
            _untilTooltipDisplaytimer -= Time.deltaTime;
            if(_untilTooltipDisplaytimer<=0)
            {
                _untilTooltipDisplaytimer = 0;
                _currentTooltip = Instantiate(_tooltipBase, _tooltipLayer);
                _currentTooltip.SetCanvas(_tooltipCanvas);
                _currentTooltip.SetTitle(_previouslyHoveredNode.PerkSO.Name);
                _currentTooltip.SetDescription(FormatPerkDesription(_previouslyHoveredNode.PerkSO));
                _currentTooltip.SetTitleImage(IconBallSprite);
                if(!_previouslyHoveredNode.IsActive)
                {
                    _currentTooltip.AddCost(_aelionEidosSprite, _previouslyHoveredNode.PerkSO.EidosCost);
                }
            }
        }
    }
    private void OnDisable()
    {
        ClearActiveConnections();
    }
    private void OnEnable()
    {
        SetUpConnections();
    }
    #endregion

    #region Methods
    public string FormatPerkDesription(PerkSO perk)
    {
        string ret = "";
        if (perk.Description.Length > 0)
            ret = perk.Description;
        else if (perk.Functional == false)
        {
            if (perk.Value > 0)
                ret += "Increases ";
            else
                ret += "Decreases ";
            if (perk.Stat == PerkSO.StatType.AttackSpeed)
                ret += "attack speed by ";
            else if (perk.Stat == PerkSO.StatType.BaseDamage)
                ret += "base damage by ";
            else if (perk.Stat == PerkSO.StatType.CombatManaRegen)
                ret += "mana regeneration during combat by ";
            else if (perk.Stat == PerkSO.StatType.CriticalChance)
                ret += "critical hit chance by ";
            else if (perk.Stat == PerkSO.StatType.MaxHP)
                ret += "max HP by ";
            else if (perk.Stat == PerkSO.StatType.CompanionCharges)
                ret += "max Companion attack charges amount by ";
            else if (perk.Stat == PerkSO.StatType.DashCharges)
                ret += "max dash charges amount by ";
            else if (perk.Stat == PerkSO.StatType.Defense)
                ret += "defense by ";
            else if (perk.Stat == PerkSO.StatType.MaxDamage)
                ret += "max damage by ";
            else if (perk.Stat == PerkSO.StatType.Stability)
                ret += "Stability by ";
            else if (perk.Stat == PerkSO.StatType.Vampirism)
                ret += "Vampirism by ";
            if (perk.IsPercent)
                ret += (perk.Value*100).ToString("0.#") + "%";
            else
                ret += perk.Value;
            ret += ".";
            if (perk.Stat == PerkSO.StatType.CompanionCharges)
                ret += " You need 10 charges to launch a Companion attack.";
            else if (perk.Stat == PerkSO.StatType.DashCharges)
                ret += " You need 10 charges to dash.";
            if (perk.HeroClass != null)
            {
                ret += " Applies only to the " + perk.HeroClass.Name + " class.";
            }
        }
        else ret = " ";
        return ret;
    }
    public void UpdateMouseCollision()
    {
        Vector2 mouse = Mouse.current.position.ReadValue();
        Ray ray = _atlasCamera.ScreenPointToRay(mouse);
        if (Physics.Raycast(ray, out RaycastHit hit, 50, LayerMask.GetMask("Default")) && hit.collider.TryGetComponent<AtlasNodeBehaviour>(out var node) == true)
        {
            var currentNode = _currentlyDisplayedNodes.FirstOrDefault(n => n == node);
            if (currentNode != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentNode.OnPointerUp();
                }
                if (_previouslyHoveredNode != null && _previouslyHoveredNode != currentNode)
                {
                    if (_previouslyHoveredNode != currentNode)
                    {
                        ClearHoveredNode(currentNode);
                    }
                }
                if (_previouslyHoveredNode != currentNode)
                {
                    ClearHoveredNode(currentNode);
                }
            }
        }
        else if (_previouslyHoveredNode != null)
        {
            ClearHoveredNode(null);
        }
    }
    private void ClearHoveredNode(AtlasNodeBehaviour replacementNode)
    {
        if(_previouslyHoveredNode!= null)
            _previouslyHoveredNode.OnPointerExit();
        _previouslyHoveredNode = null;
        if(_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
        if (replacementNode != null)
        {
            replacementNode.OnPointerEnter();
            _previouslyHoveredNode = replacementNode;
        }
        _untilTooltipDisplaytimer = 0.5f;
    }
    public void UpdateUnlockedNodes()
    {
        foreach(var node in _currentlyDisplayedNodes)
        {
            if(SkyforgeLoader.CurrentProfile.AcquiredPerks.Any(p=>p.PerkID == node.PerkSO.ID))
            {
                node.SetToActive(true);
            }
        }
    }
    public void SetUpConnections()
    {
        var alreadyCheckedList = new List<AtlasNodeBehaviour>();
        ClearActiveConnections();
        foreach (var node in _currentlyDisplayedNodes)
        {
            if(!alreadyCheckedList.Contains(node))
            {
                alreadyCheckedList.Add(node);
                foreach(var childNode in node.ChildNodes)
                {
                    var connection = Instantiate(_connectionBase, _connectionsLayer);
                    connection.SetNodes(node, childNode);
                    connection.SetActivated(node.IsActive && childNode.IsActive);
                    _currentlyDisplayedConnections.Add(connection);
                }
            }
        }
    }
    private void ClearActiveConnections()
    {
        var toDestroyList = _currentlyDisplayedConnections.ToList();
        foreach (var connection in toDestroyList)
        {
            _currentlyDisplayedConnections.Remove(connection);
            Destroy(connection.gameObject);
        }
    }
    public bool CheckChainUnlockCondition(AtlasNodeBehaviour atlasNodeBehaviour)
    {
        bool atLeastOne = false;
        if (atlasNodeBehaviour.ChildNodes.Any(n => n.IsActive))
            return true;
        foreach(var node in _currentlyDisplayedNodes)
        {
            if (node.ChildNodes.Contains(atlasNodeBehaviour))
            {
                atLeastOne = true;
                if (node.IsActive)
                    return true;
            }
        }
        if (atLeastOne)
            return false;
        else
            return true;
    }
    #endregion
}
