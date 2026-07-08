using UnityEngine;

public class DOTSlidingUpDestroyableGeneratorAnimationBehaviour : MonoBehaviour
{
    #region Variables
    private Animator _animator;
    [SerializeField] DOTSlidingUpDestroyableGenerator _generator;
    #endregion

    #region Mono
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(_generator.SlideAlreadyActivated == false && _generator.IsUp)
        {
            _generator.SlideAlreadyActivated = true;
            _animator.SetTrigger("SlideUp");
            _animator.SetBool("IsUp", true);
        }
        else if(_generator.IsUp == false && _generator.SlideAlreadyActivated == true)
        {
            _animator.SetBool("IsUp", false);
            _generator.SlideAlreadyActivated = false;
        }
        else if(_generator.DeadMeshAppeared)
        {
            _generator.DeadMeshAppeared = false;
            _animator.SetBool("IsDead", true);
            _animator.SetBool("IsUp", true);
        }
        if(_generator.IsDead && !_animator.GetBool("IsDead"))
        {
            _animator.SetBool("IsDead", true);
            _animator.SetBool("IsUp", true);
        }
    }
    #endregion
}
