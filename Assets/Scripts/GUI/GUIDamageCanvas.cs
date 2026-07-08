using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIDamageCanvas : MonoBehaviour
{
    private const float POSITION_VARIANCE = 0.2f;

    #region Variables
    [Tooltip("When the damage is critical, additional background will be displayed")]
    [SerializeField] private Image _criticalBackground;
    [Tooltip("Text box that displayes the amound of damage")]
    [SerializeField] private TextMeshProUGUI _damageText;
    [Tooltip("The whole damage display group, with Animator etc")]
    [SerializeField] private CanvasGroup _group;
    private bool _alreadyVisible, _scheduledForFadeout;
    private float _fadeoutTimer;
    private Damage _previousDamage;
    //in case the same damage displays, buc changes its value
    private int _previousDisplayValue;
    private int _previousAmountsIndex;
    private int _targetDisplayValue;
    private Vector3 _initialPosition;
    #endregion

    #region Mono
    private void Awake()
    {
        Reset();
        _initialPosition = transform.localPosition;
    }
    private void Reset()
    {
        _alreadyVisible = false;
        _scheduledForFadeout = false;
        _fadeoutTimer = 0.5f;
        _previousDamage = null;
        this.gameObject.SetActive(false);
        _previousDisplayValue = 0;
        _previousAmountsIndex = 0;
        _targetDisplayValue = 0;
    }
    #endregion

    #region Methods
    public void UpdateDamage(Damage damage)
    {
        //new Damage was added
        if (_previousDamage != null && damage!= null && _previousDamage!= damage)
        {
            AddNewDamageOverPrevious(damage);
        }
        //Multishot Damage Handling
        else if(_previousDamage == damage && damage != null && _previousAmountsIndex < damage.PreviousAmounts.Count)
        {
            AddToMultishot(damage);
        }
        //Normal Damage Handling
        else if(damage != null)
        {
            if (_alreadyVisible == false)
            {
                AddNewDamage(damage);
            }
            else
            {
                if (damage.Critical)
                {
                    _criticalBackground.gameObject.SetActive(true);
                }
                else
                {
                    _criticalBackground.gameObject.SetActive(false);
                }
            }          
        }
        //handle already shown damage
        else if (_alreadyVisible == true)
        {
            if (_scheduledForFadeout == false)
            {
                _scheduledForFadeout = true;
                _group.GetComponent<Animator>().SetBool("IsVisible", false);
            }
            else if (_scheduledForFadeout == true)
            {
                if (_fadeoutTimer <= 0)
                {
                    Reset();
                }
                _fadeoutTimer -= Time.deltaTime;
            }
        }
        //damage increase animation in case multishot
        if (_previousDisplayValue != _targetDisplayValue)
        {
            _previousDisplayValue += (_previousDisplayValue*(int)(Time.deltaTime * 200))/10;
            {
                if (_previousDisplayValue > _targetDisplayValue)
                {
                    _previousDisplayValue = _targetDisplayValue;
                }
                _damageText.text = _previousDisplayValue.ToString();
            }
        }
        else if(damage!=null && damage.PreviousAmounts.Count == 0)
        {
            _damageText.text = damage.Amount.ToString();
        }
        float distance = Vector3.Distance(Globals.Instance.ViewportCamera.transform.position, transform.position);
        transform.localScale = Vector3.one * distance * 0.08f;
        transform.forward = -Globals.Instance.ViewportCamera.transform.forward;
    }

    private void AddNewDamage(Damage damage)
    {
        Reset();
        _previousDamage = damage;
        _alreadyVisible = true;
        this.gameObject.SetActive(true);
        _group.GetComponent<Animator>().SetBool("IsVisible", true);
        PickRandomDamageDisplaySpot();
    }

    private void AddNewDamageOverPrevious(Damage damage)
    {
        _group.GetComponent<Animator>().SetBool("IsVisible", false);
        Reset();
        _previousDamage = damage;
        PickRandomDamageDisplaySpot();
    }
    private void AddToMultishot(Damage damage)
    {
        //next line is for setting damage to be previous damage value, so the increase animation doesn't go from 0
        if (_previousAmountsIndex == 0)
            _previousDisplayValue = damage.Amount;
        _previousAmountsIndex++;
        _targetDisplayValue = damage.Amount;
        foreach (var subdamage in damage.PreviousAmounts)
        {
            _targetDisplayValue += subdamage;
        }
        if (_scheduledForFadeout == true)
        {
            _scheduledForFadeout = false;
            _fadeoutTimer = 0.5f;
        }
    }
    private void PickRandomDamageDisplaySpot()
    {
        transform.localPosition = new Vector3(_initialPosition.x + UnityEngine.Random.Range(-POSITION_VARIANCE, POSITION_VARIANCE), 
            _initialPosition.y + UnityEngine.Random.Range(-POSITION_VARIANCE, POSITION_VARIANCE),
            _initialPosition.z + UnityEngine.Random.Range(-POSITION_VARIANCE, POSITION_VARIANCE));
    }
    #endregion
}
