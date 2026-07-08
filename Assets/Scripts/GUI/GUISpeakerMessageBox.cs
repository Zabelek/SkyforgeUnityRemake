using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUISpeakerMessageBox : MonoBehaviour
{
    #region Variables
    [SerializeField] private Image _profileImage;
    [SerializeField] private TextMeshProUGUI _nameTextBox;
    [SerializeField] private TextMeshProUGUI _messageTextBox;
    private float _fadeInTimer, _fadeOutTimer, _displayTimer;
    private float _defaultXPos, _defaulYYRotation;
    #endregion

    #region Mono
    private void Awake()
    {
        var rect = GetComponent<RectTransform>();
        _defaultXPos = rect.anchoredPosition.x;
        _defaulYYRotation = rect.localEulerAngles.y;
        GetComponent<CanvasGroup>().alpha = 0;
    }
    private void Update()
    {
        //based on timers, fade in or fade out
        if(_displayTimer>0)
        {
            if(_fadeInTimer > 0)
            {
                GetComponent<CanvasGroup>().alpha = 1;
                _fadeInTimer -= Time.deltaTime;
                var rect = GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector3(_defaultXPos - (300 * _fadeInTimer * 2), rect.anchoredPosition.y);
                rect.localEulerAngles = new Vector3(rect.localEulerAngles.x, _defaulYYRotation - (90 * _fadeInTimer * 2), rect.localEulerAngles.z);
                if(_fadeInTimer<=0)
                {
                    _fadeInTimer = 0;
                    rect.anchoredPosition = new Vector3(_defaultXPos, rect.anchoredPosition.y);
                    rect.localEulerAngles = new Vector3(rect.localEulerAngles.x, _defaulYYRotation, rect.localEulerAngles.z);
                }
            }
            _displayTimer -= Time.deltaTime;
            if(_displayTimer<=0)
            {
                HideWindow();
            }
        }
        if(_fadeOutTimer>0)
        {
            _fadeOutTimer -= Time.deltaTime;
            GetComponent<CanvasGroup>().alpha = 2 * _fadeOutTimer;
            if (_fadeOutTimer<=0)
            {
                gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Methods
    public void SetMessage(VoicelineSO voiceline)
    {
        _nameTextBox.text = voiceline.Speaker.Name;
        _messageTextBox.text = voiceline.Text;
        _profileImage.sprite = voiceline.Speaker.Image;
        _displayTimer = voiceline.Time;
        Globals.Instance.SoundManager.PlayGlobalVoice(voiceline.Voice, voiceline.Volume);
        ShowWindow();
    }
    private void ShowWindow()
    {
        _fadeInTimer = 0.5f;
        gameObject.SetActive(true);
    }
    private void HideWindow()
    {
        _fadeOutTimer = 0.5f;
    }
    #endregion
}
