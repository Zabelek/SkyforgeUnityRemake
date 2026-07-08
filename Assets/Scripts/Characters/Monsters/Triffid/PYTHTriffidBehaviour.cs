using UnityEngine;

public class PYTHTriffidBehaviour : MonsterBehaviour
{
    #region Variables
    [Header("Triffid Related Variables")]
    [SerializeField] private ParticleSystem _deathParticles;
    #endregion

    #region Methods
    public override void Kill(CharacterBehaviour killer)
    {
        base.Kill(killer);
        Instantiate(_deathParticles, this.transform).gameObject.SetActive(true);
    }
    public override void LeaveCombat()
    {
        base.LeaveCombat();
        //If triffids other than healing one will be added, the healing triffid has to have it's own class, where tese below line should go
        if (!IsDead)
        {
            Kill(this);
        }
    }
    #endregion
}
