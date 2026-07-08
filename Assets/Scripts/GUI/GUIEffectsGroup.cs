using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIEffectsGroup : MonoBehaviour
{
    #region Variables
    [Tooltip("Template used to create all the effect icons")]
    [SerializeField] private GUIEffectMenuIcon _iconTemplate;
    [Tooltip("All new effect icons will be placed here")]
    [SerializeField] private LayoutGroup _layout;
    private List<GUIEffectMenuIcon> _effectIcons;
    private CharacterBehaviour _character;
    public bool TopVersion { get; set; }
    #endregion

    #region Mono
    private void Awake()
    {
        _effectIcons = new();
    }
    private void Update()
    {
        if(_character!=null)
        {
            var effectsToAdd = new List<GameplayEffectBehaviour>();
            foreach (var effect in _character.GetActiveEffects())
            {
                bool isPresent = false;
                foreach (var icon in _effectIcons)
                {
                    if (icon.GetEffect() == effect)
                    {
                        isPresent = true;
                        break;
                    }
                }
                if (!isPresent && !effect.EffectSO.IsHidden)
                {
                    effectsToAdd.Add(effect);
                }
            }
            foreach (var effect in effectsToAdd)
            {
                var newIcon = Instantiate(_iconTemplate, _layout.transform);
                newIcon.gameObject.SetActive(true);
                newIcon.Init(effect, this);
                newIcon.TopVersion = TopVersion;
                _effectIcons.Add(newIcon);
            }
        }      
    }
    #endregion

    #region Methods
    public void Remove(GUIEffectMenuIcon icon)
    {
        _effectIcons.Remove(icon);
        Destroy(icon.gameObject);
    }
    public void SetCharacter(CharacterBehaviour character)
    {
        _character = character;
        foreach(var icon in _effectIcons)
        {
            Destroy(icon.gameObject);
        }
        _effectIcons.Clear();
    }
    #endregion
}
