using UnityEngine;

public class DOTTriffidBossAIHandler : AIHandlerBehaviour
{
    #region Variables
    public float TimeAfterLastDeadPillarToResummon;
    private float _timeSinceLastPillarDown;
    private PYTHTriffidBossBehaviour _triffid;
    #endregion

    #region Mono
    protected override void Start()
    {
        base.Start();
        if(_character != null && _character is PYTHTriffidBossBehaviour)
        {
            _triffid = _character as PYTHTriffidBossBehaviour;
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(_triffid != null && _timeSinceLastPillarDown==-100 && _triffid.GetPillarAmount()==0)
        {
            _timeSinceLastPillarDown = TimeAfterLastDeadPillarToResummon;
        }
        if (_timeSinceLastPillarDown != -100 && _triffid.GetPillarAmount() > 0)
            _timeSinceLastPillarDown = -100;
        if (_timeSinceLastPillarDown > 0)
            _timeSinceLastPillarDown -= Time.fixedDeltaTime;
    }
    #endregion

    #region Methods
    public override bool TryPerformAttack()
    {
        if (_triffid.CanAct())
        {
            if (_triffid.SecondSpecialAbility != null && _timeSinceLastPillarDown <= 0 && _timeSinceLastPillarDown !=-100 && _triffid.TryPerformAbility(_triffid.SecondSpecialAbility))
                return true;
            else if (_triffid.ThirdSpecialAbility != null && _triffid.TryPerformAbility(_triffid.ThirdSpecialAbility))
                return true;
        }
        return base.TryPerformAttack();
    }
    #endregion

    #region EventHandlers
    #endregion
}
