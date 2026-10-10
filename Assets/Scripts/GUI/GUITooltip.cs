using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUITooltip : MonoBehaviour
{
    public const float FADE_IN_TIME = 0.5f;
    public const float APPEAR_DELAY = 0.1f;

    #region Variables
    [SerializeField] protected Image _titleImage, _progressBarFill, _Upgradebackground;
    [SerializeField] protected TextMeshProUGUI _titleText, _descriptionText, _specialDescriptionText, _addInfoTopText;
    [SerializeField] Transform _costPanel, _costLayout, _costOpsitionTemplate, _statBonusPanel, _specialDescriptionPanel, _statLineBase, _markedStatBonusPanel, _progressBarTransform;
    private float _fadeInTimer, _waitUntilAppearTimer;
    private CanvasGroup _group;
    [SerializeField] private RectTransform _tooltip;
    [SerializeField] private Canvas _canvas;
    #endregion

    #region Mono
    private void Awake()
    {
        if(_costPanel!= null)
            _costPanel.gameObject.SetActive(false);
        if (_specialDescriptionPanel != null)
            _specialDescriptionPanel.gameObject.SetActive(false);
        if (_statLineBase != null)
            _statLineBase.gameObject.SetActive(false);
        if (_statBonusPanel != null)
            _statBonusPanel.gameObject.SetActive(false);
        if (_markedStatBonusPanel != null)
            _markedStatBonusPanel.gameObject.SetActive(false);
        if (_progressBarTransform != null)
            _progressBarTransform.gameObject.SetActive(false);        
        if (_Upgradebackground != null)
            _Upgradebackground.gameObject.SetActive(false);
        _fadeInTimer = 0;
        _waitUntilAppearTimer = APPEAR_DELAY;
        _group = GetComponent<CanvasGroup>();
        if (_group != null)
            _group.alpha = 0;
        _addInfoTopText.gameObject.SetActive(false);

    }
    private void Update()
    {
        //waiting 0.1 seconds after canvas init to avoid display bugs
        if (_waitUntilAppearTimer>0)
        {
            _waitUntilAppearTimer -= Time.deltaTime;
            if(_waitUntilAppearTimer<=0)
            {
                _fadeInTimer = FADE_IN_TIME;
            }
        }
        if(_fadeInTimer > 0 && _group != null)
        {
            _fadeInTimer -= Time.deltaTime;
            _group.alpha = Mathf.Lerp(1, 0, _fadeInTimer / FADE_IN_TIME);
        }
    }
    private void LateUpdate()
    {
        PositionTooltip();
    } 
    #endregion

    #region Methods
    public void SetTitle(string title)
    {
        _titleText.text = title;
    }
    public void SetDescription(string description)
    {
        _descriptionText.text = description;
        GetComponent<VerticalLayoutGroup>().reverseArrangement = true;
        StartCoroutine(RebuildLayout());
    }
    public void SetSpecialDescription(string description)
    {
        _specialDescriptionPanel.gameObject.SetActive(true);
        _specialDescriptionText.text = description;
        GetComponent<VerticalLayoutGroup>().reverseArrangement = true;
        StartCoroutine(RebuildLayout());
    }
    private IEnumerator RebuildLayout()
    {
        //For reasons unknown to me, this must be done to rebuild the layout, as it goes bonkers every time a text is set. It works in the editor, but doesn't work at runtime. No idea why.
        //This function may change when a better solution is discovered.
        yield return new WaitForSeconds(0.05f);
        GetComponent<VerticalLayoutGroup>().reverseArrangement = false;
    }
    public void SetTitleImage(Sprite image)
    {
        _titleImage.sprite = image;
    }
    public void SetCanvas(Canvas canvas)
    {
        _canvas = canvas;
    }
    public void AddStatBonus(string statBonusName, float statBonus, bool isPercent)
    {
        if(statBonus != 0)
        {
            _statBonusPanel.gameObject.SetActive(true);
            var newLine = Instantiate(_statLineBase, _statBonusPanel);
            newLine.gameObject.SetActive(true);
            newLine.GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "StatName").text = statBonusName;
            if (isPercent)
                newLine.GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "StatValue").text = (statBonus * 100).ToString("0.00") + "%";
            else
                newLine.GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "StatValue").text = statBonus.ToString();
        }
    }
    public void AddMarkedStatBonus(string statBonusName, float statBonus, bool isPercent)
    {
        if (statBonus != 0)
        {
            _markedStatBonusPanel.gameObject.SetActive(true);
            var newLine = Instantiate(_statLineBase, _markedStatBonusPanel);
            newLine.gameObject.SetActive(true);
            newLine.GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "StatName").text = statBonusName;
            if (isPercent)
                newLine.GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "StatValue").text = (statBonus * 100).ToString("0.00") + "%";
            else
                newLine.GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "StatValue").text = statBonus.ToString();
        }
    }
    public void AddCost(Sprite costIcon, int costAmount)
    {
        _costPanel.gameObject.SetActive(true);
        var newPosition = Instantiate(_costOpsitionTemplate, _costLayout);
        newPosition.gameObject.SetActive(true);
        var costText = newPosition.GetComponentInChildren<TextMeshProUGUI>();
        if (costText != null)
        {
            costText.text = costAmount.ToString();
        }
        var costImage = newPosition.GetComponentInChildren<Image>();
        if(costImage != null)
        {
            costImage.sprite = costIcon;
        }
    }
    private void PositionTooltip()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x/Screen.width*_canvas.renderingDisplaySize.x, Input.mousePosition.y/Screen.height*_canvas.renderingDisplaySize.y);
        Vector2 size = _tooltip.rect.size;
        float canvasWidth = _canvas.pixelRect.width;
        float canvasHeight = _canvas.pixelRect.height;

        Vector2 defaultPosition = mousePos;

        float topOffset = 0;
        if (size.y + defaultPosition.y > _canvas.renderingDisplaySize.y)
            topOffset -= size.y;
        float leftOffset = 0;
        if (defaultPosition.x - size.x < 0)
            leftOffset += size.x;
        _tooltip.anchoredPosition = new Vector2(defaultPosition.x + leftOffset, defaultPosition.y + topOffset);
    }
    public void SetForItem(Item item)
    {
        SetTitle(item.ItemSO.Name);
        SetDescription(item.ItemSO.Description);
        SetTitleImage(item.ItemSO.InterfaceSprite);
        GearBonus markedBonus = null;
        if (item.ItemSO is WeaponSO)
        {
            var baseStats = WeaponBehaviour.GetGeneralBaseStats(item);
            markedBonus = WeaponBehaviour.GetGeneralMarkedStats(item);
            AddStatBonus("Damage Bonus: ", baseStats.DamageBonus, false);
            SetDescription(((WeaponSO)item.ItemSO).Type.Name);
            if (item.ItemSO.Rarity == ItemSO.RarityLevel.Legendary)
            {
                SetSpecialDescription(item.ItemSO.Description);
            }
            else
            {
                SetDescription(((WeaponSO)item.ItemSO).Type.Name + "\n" + item.ItemSO.Description);
            }
        }
        else if (item.ItemSO is ArmorSO)
        {
            var baseStats = ArmorBehaviour.GetGeneralBaseStats(item);
            markedBonus = ArmorBehaviour.GetGeneralMarkedStats(item);
            AddStatBonus("Armor Bonus: ", baseStats.Armor, false);
        }
        else if(item.ItemSO is ArtifactSO)
        {
            var baseStats = ArtifactBehaviour.GetGeneralBaseStats(item);
            markedBonus = ArtifactBehaviour.GetGeneralMarkedStats(item);
            AddStatBonus("Health Bonus: ", baseStats.HealthPercentBonus, true);
            AddStatBonus("Damage Bonus: ", baseStats.DamagePercentBonus, true);
            if(item.ItemSO.Rarity == ItemSO.RarityLevel.Legendary)
            {
                SetDescription("");
                SetSpecialDescription(item.ItemSO.Description);
            }
        }
        if (item.ItemSO.IsUpgradable)
        {
            _addInfoTopText.gameObject.SetActive(true);
            _addInfoTopText.text = "Generation " + item.ItemUpgradeState.Generation;
            if(markedBonus != null)
            {
                if (markedBonus.HealthPercentBonus > 0)
                    AddMarkedStatBonus("Health Bonus: ", markedBonus.HealthPercentBonus, true);
                if (markedBonus.ArmorPercentBonus > 0)
                    AddMarkedStatBonus("Armor Bonus: ", markedBonus.ArmorPercentBonus, true);
                if (markedBonus.DamagePercentBonus > 0)
                    AddMarkedStatBonus("Damage Bonus: ", markedBonus.DamagePercentBonus, true);
                if (markedBonus.CriticalDamagePercentBonus > 0)
                    AddMarkedStatBonus("Critical Damage Bonus: ", markedBonus.CriticalDamagePercentBonus, true);
                if (markedBonus.CompanionDamagePercentBonus > 0)
                    AddMarkedStatBonus("Companion Damage Bonus: ", markedBonus.CompanionDamagePercentBonus, true);
                if (markedBonus.VampirismBonus > 0)
                    AddMarkedStatBonus("Vampirism Bonus: ", markedBonus.VampirismBonus, true);
                if (markedBonus.MaxDamagePercentBonus > 0)
                    AddMarkedStatBonus("Max Damage Bonus: ", markedBonus.MaxDamagePercentBonus, true);
                if (markedBonus.AttackSpeedBonus > 0)
                    AddMarkedStatBonus("Attack Speed Bonus: ", markedBonus.AttackSpeedBonus, true);
                if (markedBonus.StabilityBonus > 0)
                    AddMarkedStatBonus("Stability Bonus: ", markedBonus.StabilityBonus, true);
                if (markedBonus.DashChargesBonusPercent > 0)
                    AddMarkedStatBonus("Dash Charges Bonus: ", markedBonus.DashChargesBonusPercent, true);
                if (markedBonus.CompanionChargesBonusPercent > 0)
                    AddMarkedStatBonus("Companion Charges Bonus: ", markedBonus.CompanionChargesBonusPercent, true);
                if (markedBonus.MovementSpeedPercentBonus > 0)
                    AddMarkedStatBonus("Movement Speed Bonus: ", markedBonus.MovementSpeedPercentBonus, true);
                if (markedBonus.MoneyReceivedBonus > 0)
                    AddMarkedStatBonus("Credits Received Bonus: ", markedBonus.MoneyReceivedBonus, true);
            }
            _Upgradebackground.gameObject.SetActive(item.ItemUpgradeState.CanGenUp());
            _progressBarTransform.gameObject.SetActive(true);
            _progressBarFill.fillAmount = (float)((decimal)item.ItemUpgradeState.CurrentExp / (decimal)item.ItemUpgradeState.GetMaxExp());
        }
        else
            _addInfoTopText.gameObject.SetActive(false);
    }
    #endregion
}
