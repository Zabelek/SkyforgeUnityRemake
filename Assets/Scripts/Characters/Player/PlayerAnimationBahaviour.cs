using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationBehaviour : HeroAnimationBehaviour
{
    private class AnimatorSnapshot
    {
        public RuntimeAnimatorController Controller;
        public Dictionary<int, bool> Bools = new();
        public Dictionary<int, int> Ints = new();
        public Dictionary<int, float> Floats = new();
        public AnimatorSnapshot(Animator controller)
        {
            Controller = controller.runtimeAnimatorController;
            foreach (var p in controller.parameters)
            {
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        Bools[p.nameHash] = controller.GetBool(p.nameHash);
                        break;
                    case AnimatorControllerParameterType.Int:
                        Ints[p.nameHash] = controller.GetInteger(p.nameHash);
                        break;
                    case AnimatorControllerParameterType.Float:
                        Floats[p.nameHash] = controller.GetFloat(p.nameHash);
                        break;
                }
            }
        }
        public void RestoreAnimator(Animator controller)
        {
            controller.runtimeAnimatorController = Controller;
            foreach (var pair in Bools)
                controller.SetBool(pair.Key, pair.Value);
            foreach (var pair in Ints)
                controller.SetInteger(pair.Key, pair.Value);
            foreach (var pair in Floats)
                controller.SetFloat(pair.Key, pair.Value);
        }
    }

    #region Variables
    PlayerBehaviour _player;
    private bool _playingEmote;  
    private AnimatorSnapshot _animationSnapshot;
    #endregion

    #region Mono
    protected override void Start()
    {
        base.Start();
        if (_character is PlayerBehaviour)
            _player = (PlayerBehaviour)_character;
        _player.OnHurt += Hurt_Performed;
        _player.OnDeath += Death_Performed;
        _player.OnResurrect += Resurrect_Performed;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_player != null && _animator != null)
        {
            _animator.SetBool("IsMoving", _player.IsRunning);
            _animator.SetFloat("Forward", _player.GetRunVector().x);
            _animator.SetFloat("Right", _player.GetRunVector().y);
            _animator.SetFloat("Attack_Speed", _player.GetEffectiveAttackSpeed());
            _animator.SetFloat("Speed", _player.GetMovementSpeedModifiers());
            if (_playingEmote)
            {
                var parameter = _animator.GetCurrentAnimatorStateInfo(0);
                if (_animator.GetBool("Interrupted") && parameter.normalizedTime >= 1 && parameter.loop == false)
                {
                    RestorePreviousController();
                }
            }
        }

    }
    #endregion

    #region EventHandlers
    private void Resurrect_Performed(object sender, EventArgs e)
    {
        _animator?.SetBool("IsDead", false);
    }
    private void Death_Performed(object sender, EventArgs e)
    {
        _animator?.SetBool("IsDead", true);
        _animator?.SetTrigger("Death");
    }
    private void Hurt_Performed(object sender, EventArgs e)
    {
        _animator?.SetTrigger("Hurt");
        _animator?.SetFloat("Hurt_Variance", UnityEngine.Random.Range(0f, 3f));
    }
    public override void PerformEmote(EmoteSO emoteSO)
    {
        _animationSnapshot = new AnimatorSnapshot(_animator);
        SetController(emoteSO.AnimatorController);
        _animator.ResetControllerState();
        _playingEmote = true;
    }
    public override void StopEmote()
    {
        if(_playingEmote)
        {
            if (_animator.GetBool("Interrupted"))
            {
                RestorePreviousController();
            }
            else
                _animator.SetBool("Interrupted", true);
        }
    }
    private void RestorePreviousController()
    {
        _animationSnapshot.RestoreAnimator(_animator);
        _animationSnapshot = null;
        _playingEmote = false;
        _animator.SetBool("IsWeaponOut", _player.CombatStance);
    }
    public override void SetController(RuntimeAnimatorController controller)
    {
        base.SetController(controller);
    }
    #endregion
}