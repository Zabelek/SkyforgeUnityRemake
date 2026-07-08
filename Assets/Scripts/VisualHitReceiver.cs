using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class VisualHitReceiver : MonoBehaviour
{
    #region Variables
    [Tooltip("Effect that would spawn when the character gets meelee hit")]
    [SerializeField] private GameObject _hitEffectMeelee;
    [Tooltip("Effect that would spawn when the character gets ranged hit")]
    [SerializeField] private GameObject _hitEffectRanged;
    [Tooltip("Current character reference")]
    [SerializeField] private CharacterBehaviour _character;
    private float _lightenTimer = 0;
    private float _darkenTimer = 0;
    private float _heallLightenTimer = 0;
    private float _healDarkenTimer = 0;
    #endregion

    #region Mono
    private void Start()
    {
        _character = GetComponent<CharacterBehaviour>();
    }
    private void Update()
    {
        //timers related to glow animatinos
        if (_lightenTimer > 0)
        {
            _lightenTimer -= Time.deltaTime * 4;
            if (_lightenTimer < 0)
                _lightenTimer = 0;
            _character.SetDamageGlow((0.3f - _lightenTimer) / 2, true);
        }
        else if (_darkenTimer > 0)
        {
            _darkenTimer -= Time.deltaTime * 4;
            if (_darkenTimer < 0)
                _darkenTimer = 0;
            _character.SetDamageGlow(_darkenTimer / 4, false);
        }
        if (_heallLightenTimer > 0)
        {
            {
                _heallLightenTimer -= Time.deltaTime * 4;
                if (_heallLightenTimer < 0)
                    _heallLightenTimer = 0;
                _character.SetGlow((0.6f - _heallLightenTimer) / 4, Color.lightGreen);
            }
        }
        else if (_healDarkenTimer > 0)
        {
            _healDarkenTimer -= Time.deltaTime * 4;
            if (_healDarkenTimer < 0)
                _healDarkenTimer = 0;
            _character.SetGlow(_healDarkenTimer / 8, Color.lightGreen);
        }
    }
    #endregion

    #region Methods
    public void GetHit()
    {
        Instantiate(_hitEffectRanged, transform);
        _lightenTimer = 0.3f;
        _darkenTimer = 0.6f;
    }
    public void GetHit(bool ranged, Vector3 source)
    {
        //for now it just spawns a sprite, it will be replace by a proper 2d animation in the future
        if (!ranged)
        {
            var effect = Instantiate(_hitEffectMeelee, transform);
            //the meelee hit receive "animation" will be displayed slightly towards the damage source direction, not directly in the center of the character
            effect.transform.position += source/2;
        }
        else
        {
            Instantiate(_hitEffectRanged, transform);
        }
        _lightenTimer = 0.3f;
        _darkenTimer = 0.6f;
    }
    public void GetHeal()
    {
        _heallLightenTimer = 0.6f;
        _healDarkenTimer = 1f;
    }
    #endregion

    #region GLowSetters
    public void SetGlow(float alpha)
    {
        //usually the character's model is made from more than one mesh rentderers, that's why list
        List<SkinnedMeshRenderer> renderers = GetComponentsInChildren<SkinnedMeshRenderer>().Where(r => r.gameObject.layer == LayerMask.NameToLayer("Characters")).ToList();
        foreach (var r in renderers)
        {
            if (!r.enabled) continue;
            //if the custom shader pass is enabled in the scene, all meshes in Characters layer have custom material attached to them.
            //the material does nothing except if the specific variable is set, it glows, mking the character highlighted in a specific color.
            var mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);
            mpb.SetFloat("_Custom_Glow_Alpha_Value", alpha);
            r.SetPropertyBlock(mpb);
        }
    }
    public void SetGlow(float alpha, Color color)
    {
        List<SkinnedMeshRenderer> renderers = GetComponentsInChildren<SkinnedMeshRenderer>().Where(r => r.gameObject.layer == LayerMask.NameToLayer("Characters") && r.IsDestroyed() == false).ToList();
        foreach (var r in renderers)
        {
            if (!r.enabled) continue;
            var mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);
            mpb.SetFloat("_Custom_Glow_Alpha_Value", alpha);
            mpb.SetColor("_Custom_Glow_Color", color);
            r.SetPropertyBlock(mpb);
        }
    }
    public void SetGlow(float alpha, Color color, float value)
    {
        List<SkinnedMeshRenderer> renderers = GetComponentsInChildren<SkinnedMeshRenderer>().Where(r => r.gameObject.layer == LayerMask.NameToLayer("Characters")).ToList();
        foreach (var r in renderers)
        {
            if (!r.enabled) continue;
            var mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);
            mpb.SetFloat("_Custom_Glow_Alpha_Value", alpha);
            mpb.SetFloat("_Custom_Glow_Strength", value);
            mpb.SetColor("_Custom_Glow_Color", color);
            r.SetPropertyBlock(mpb);
        }
    }
    public void SetDamageGlow(float alpha, bool add)
    {
        List<SkinnedMeshRenderer> renderers = GetComponentsInChildren<SkinnedMeshRenderer>().Where(r => r.gameObject.layer == LayerMask.NameToLayer("Characters")).ToList();
        foreach (var r in renderers)
        {
            if (!r.enabled) continue;
            //Damage glow is a separate material so efen if the character currently glows due to some effect, damage glow will be visible
            var mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);
            if (add && mpb.GetFloat("_Damage_Glow_Alpha_Value") < alpha)
                mpb.SetFloat("_Damage_Glow_Alpha_Value", alpha);
            else if (!add)
                mpb.SetFloat("_Damage_Glow_Alpha_Value", alpha);
            r.SetPropertyBlock(mpb);
        }
    }
    #endregion
}
