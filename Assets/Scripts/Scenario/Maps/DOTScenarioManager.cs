using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DOTScenarioManager : ScenarioManager
{
    #region Variables
    [Header("Divine Observatory Test Scenario")]
    [SerializeField] private CharacterBehaviour[] _group1;
    [SerializeField] private CharacterBehaviour[] _group2;
    [SerializeField] private CharacterBehaviour _boss1;
    [SerializeField] private CharacterBehaviour _boss2;
    [SerializeField] private DOTSlidingUpDestroyableGenerator[] _generators;
    [SerializeField] private DOTSlidingUpDestroyableGenerator _laserGun;
    [SerializeField] private ParticleSystem _particlesLaser;
    [SerializeField] private ParticleSystem _particlesLaserExp;
    [SerializeField] private Transform _laserDirGuide;
    [SerializeField] private WeaponBehaviour _lastSceneErrai;
    #endregion

    #region Mono
    protected override void Update()
    {
        base.Update();
        //story
        if (Stage == 0)
        {
            _cutsceneBlack.gameObject.SetActive(true);
            Globals.Instance.IsCutscenePlaying = false;
            _player.PerformEmote(this, EventArgs.Empty);
            Globals.Instance.IsCutscenePlaying = true;
            Stage = 1;
        }
        if (Stage == 2)
        {
            _interface.ShowCharacterMessage(_voicelines[0]);
            Stage = 3;
        }
        else if (Stage == 3)
        {
            if (IsGroupDead(_group1))
            {
                Stage = 4;
                _interface.ShowCharacterMessage(_voicelines[1]);
            }
        }
        else if (Stage == 4)
        {
            if (IsGroupDead(_group2))
            {
                Stage = 5;
                _interface.ShowCharacterMessage(_voicelines[2]);
            }
        }
        else if (Stage == 5)
        {
            _currentCutscene = _cutscenes[1];
            if (_boss1.IsDead)
            {
                Stage = 6;
                _interface.ShowCharacterMessage(_voicelines[3]);
                _currentCutscene = _cutscenes[2];
                StartCoroutine(DelayedCharacterRemoval(_boss1, GUISceneBlackFade.FADE_TIME));
            }
        }
        else if (Stage == 6)
        {
            if (_boss2.gameObject.activeSelf == true && _boss2.Stats.CurrentHP < (_boss2.Stats.MaxHP / 10) * 9)
            {
                Stage = 7;
                _interface.ShowCharacterMessage(_voicelines[4]);
                foreach (var generator in _generators)
                {
                    generator.RaiseUp();
                }
            }
        }
        else if (Stage == 7)
        {
            if (_generators.Any(gen => gen.IsDead == true))
            {
                Stage = 8;
                _interface.ShowCharacterMessage(_voicelines[5]);
            }
        }
        else if (Stage == 8)
        {
            if (!_generators.Any(gen => gen.IsDead == false))
            {
                Stage = 9;
                _boss2.StopMovement();
                _interface.ShowCharacterMessage(_voicelines[6]);
                if (_boss2.TryGetComponent<AIHandlerBehaviour>(out var handler))
                {
                    handler.enabled = false;
                }
                _laserGun.RaiseUp();
                _laserGun.CanBeDamaged = false;
                _laserGun.Selectable = false;
                var gunFwdVec = (_boss2.transform.position - _laserGun.transform.position).normalized;
                _laserGun.FaceTheTarget(new Vector3(gunFwdVec.x, 0, gunFwdVec.z));
                var bossFwdVec = (_laserGun.transform.position - _boss2.transform.position).normalized;
                _boss2.FaceTheTarget(new Vector3(bossFwdVec.x, 0, bossFwdVec.z));
                _currentCutscene = _cutscenes[3];
                _laserDirGuide.transform.forward = _laserGun.transform.forward;
                _particlesLaserExp.transform.SetParent(_boss2.transform);
                _particlesLaserExp.transform.localPosition = Vector3.zero;
                StartCoroutine(DelayedScene4Actions());
                SkyforgeLoader.GameBeaten = true;
                if (SkyforgeLoader.CurrentProfile.Difficulty.Name == "Average Coffin Enjoyer")
                    SkyforgeLoader.HardestDiffBeaten = true;
            }
        }
        else if (Stage == 9)
        {
            if (_boss2.IsDead)
            {
                Stage = 10;
                _currentCutscene = _cutscenes[4];
                StartCoroutine(DelayedCharacterRemoval(_boss2, GUISceneBlackFade.FADE_TIME));
                StartCoroutine(DelayedScene5Actions());
                StartCoroutine(DelayedBackToMenu());
            }
        }
    }
    private IEnumerator DelayedBackToMenu()
    {
        yield return new WaitForSeconds(21);
        _ = SkyforgeLoader.LoadScene("DivineObservatoryScene", "MainMenuScene");
    }

    protected override IEnumerator DelayedInitSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentCutscene = _cutscenes[0];
        Stage = 2;
    }
    private IEnumerator DelayedScene4Actions()
    {
        yield return new WaitForSeconds(10);
        if (_boss2.Stats.CurrentHP > (int)(_boss2.Stats.MaxHP / 10))
        {
            _boss2.TakeDamage(new Damage(_laserGun, (_boss2.Stats.CurrentHP - (int)(_boss2.Stats.MaxHP / 10))), true);
        }
    }
    private IEnumerator DelayedScene5Actions()
    {
        yield return new WaitForSeconds(1);
        _player.EquipWeapon(_lastSceneErrai);
    }
    #endregion
}
